using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace MercenaryVariety
{
    public sealed class AltairMartialTrialBehavior : CampaignBehaviorBase
    {
        internal const string ApprenticeId = "mv_assassin_apprentice_t4";
        private bool _completed;
        private bool _launchRequested;
        private bool _leaveTavernRequested;
        private Settlement _trialSettlement;

        public static AltairMartialTrialBehavior Instance { get; private set; }
        public bool IsCompleted => _completed;

        public override void RegisterEvents()
        {
            Instance = this;
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, OnMissionTick);
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnCampaignTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("mv_altair_martial_trial_completed", ref _completed);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            _launchRequested = false;
            _leaveTavernRequested = false;
            _trialSettlement = null;
            starter.AddPlayerLine("mv_altair_martial_offer", "mv_altair_completed_greeting", "mv_altair_martial_explanation",
                "{=MVAltairMartialOffer}I believe my fighting skills are worthy of a Hashashin.", CanOfferTrial, null, 205);
            starter.AddDialogLine("mv_altair_martial_explanation", "mv_altair_martial_explanation", "mv_altair_martial_choice",
                "{=MVAltairMartialExplanation}Only a practical test can tell. A veteran Hashashin can usually defeat at least two apprentices. If you are willing, I can arrange a trial right away.", null, null, 200);
            starter.AddPlayerLine("mv_altair_martial_accept", "mv_altair_martial_choice", "mv_altair_martial_accepted",
                "{=MVAltairMartialAccept}Then let me take part.", CanOfferTrial, AcceptTrial, 200);
            starter.AddDialogLine("mv_altair_martial_accepted", "mv_altair_martial_accepted", "close_window",
                "{=MVAltairMartialAccepted}Tell me whenever you are ready.", null, null, 200);
            starter.AddPlayerLine("mv_altair_martial_decline", "mv_altair_martial_choice", "close_window",
                "{=MVAltairMartialDecline}I still need time to hone my blade.", null, null, 100);

            starter.AddPlayerLine("mv_altair_martial_ready", "mv_altair_completed_greeting", "mv_altair_martial_begin",
                "{=MVAltairMartialReady}I am ready for the trial of martial skill.", CanChallenge, null, 210);
            starter.AddDialogLine("mv_altair_martial_begin", "mv_altair_martial_begin", "close_window",
                "{=MVAltairMartialBegin}Then let us begin!", null, ScheduleFight, 200);
            starter.AddPlayerLine("mv_altair_martial_report", "mv_altair_completed_greeting", "mv_altair_martial_success",
                "{=MVAltairMartialReport}I have proven my personal martial skill.", CanReportVictory, null, 220);
            starter.AddDialogLine("mv_altair_martial_success", "mv_altair_martial_success", "close_window",
                "{=MVAltairMartialSuccess}You do possess greater martial skill than the other apprentices. You have passed this trial.", null, CompleteTrial, 200);
        }

        private static bool IsTalkingToAltair() => CharacterObject.OneToOneConversationCharacter?.StringId == "mv_altair";

        internal static AltairMartialTrialQuest FindActiveQuest()
        {
            return Campaign.Current?.QuestManager?.Quests.OfType<AltairMartialTrialQuest>().FirstOrDefault(q => q.IsOngoing);
        }

        private bool CanOfferTrial() => IsTalkingToAltair() && AltairTavernBehavior.Instance?.IsTrialCompleted == true && !_completed && FindActiveQuest() == null;
        private bool CanChallenge() => IsTalkingToAltair() && !_completed && !_launchRequested && FindActiveQuest()?.HasWon == false;
        private bool CanReportVictory() => IsTalkingToAltair() && !_completed && FindActiveQuest()?.HasWon == true;

        private void AcceptTrial()
        {
            if (CanOfferTrial())
                new AltairMartialTrialQuest(Hero.MainHero).StartQuest();
        }

        private void CompleteTrial()
        {
            if (CanReportVictory())
                FindActiveQuest().CompleteQuestWithSuccess();
        }

        internal void MarkCompleted() => _completed = true;

        private void ScheduleFight()
        {
            if (!CanChallenge())
                return;
            _trialSettlement = Settlement.CurrentSettlement;
            if (!TryGetArena(out _, out _))
            {
                ShowUnavailable();
                return;
            }
            _launchRequested = true;
            Campaign.Current.ConversationManager.ConversationEndOneShot += () => _leaveTavernRequested = true;
        }

        private void OnMissionTick(float dt)
        {
            // Wait until conversation cleanup finishes before ending the tavern mission.
            if (_leaveTavernRequested && !Campaign.Current.ConversationManager.IsConversationInProgress)
            {
                _leaveTavernRequested = false;
                Mission.Current?.EndMission();
            }
        }

        private void OnCampaignTick(float dt)
        {
            if (!_launchRequested || _leaveTavernRequested || Mission.Current != null || !(Game.Current.GameStateManager.ActiveState is MapState))
                return;
            _launchRequested = false;
            var quest = FindActiveQuest();
            if (quest == null || quest.HasWon || !TryGetArena(out Location arena, out string scene))
            {
                ShowUnavailable();
                return;
            }
            try { AltairMartialTrialMission.Open(scene, arena, quest); }
            catch (Exception ex)
            {
                Debug.Print("[MercenaryVariety] Could not open martial trial: " + ex);
                ShowUnavailable();
            }
        }

        private bool TryGetArena(out Location arena, out string scene)
        {
            arena = null;
            scene = null;
            if (_trialSettlement == null || Settlement.CurrentSettlement != _trialSettlement || !_trialSettlement.IsTown || CharacterObject.Find(ApprenticeId) == null)
                return false;
            arena = _trialSettlement.LocationComplex?.GetLocationWithId("arena");
            scene = arena?.GetSceneName(_trialSettlement.Town.GetWallLevel());
            return !string.IsNullOrEmpty(scene);
        }

        private static void ShowUnavailable()
        {
            InformationManager.DisplayMessage(new InformationMessage(new TextObject(
                "{=MVAltairMartialUnavailable}The trial could not begin. Your quest is unchanged; speak to Altair to try again.").ToString()));
        }
    }
}
