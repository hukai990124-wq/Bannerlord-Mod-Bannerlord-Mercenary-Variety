using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace MercenaryVariety
{
    public sealed class AltairInitiationTrialBehavior : CampaignBehaviorBase
    {
        internal const string MasterId = "mv_assassin_master_t6";
        internal const string AssassinId = "mv_assassin_t5";
        internal const string ApprenticeId = "mv_assassin_apprentice_t4";
        internal const string EnemyId = "hidden_hand_tier_3";
        private const string DesertBanditId = "desert_bandits_bandit";
        private const string SeaRaiderId = "sea_raiders_bandit";
        private const string SeaRaiderChiefId = "sea_raiders_chief";
        private const float ManpowerRequestCooldownDays = 10f;

        private bool _completed;
        private bool _launchRequested;
        private bool _leaveTavernRequested;
        private Settlement _trialSettlement;
        private CampaignTime _nextManpowerRequestTime = CampaignTime.Zero;
        private int _pendingManpowerOffer = -1;

        public static AltairInitiationTrialBehavior Instance { get; private set; }
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
            dataStore.SyncData("mv_altair_initiation_trial_completed", ref _completed);
            dataStore.SyncData("mv_altair_next_manpower_request_time", ref _nextManpowerRequestTime);
            dataStore.SyncData("mv_altair_pending_manpower_offer", ref _pendingManpowerOffer);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            _launchRequested = false;
            _leaveTavernRequested = false;
            _trialSettlement = null;
            starter.AddPlayerLine("mv_altair_initiation_offer", "mv_altair_completed_greeting", "mv_altair_initiation_explanation",
                "{=MVAltairInitiationOffer}What remains before I may be counted among the Hashashin?", CanOffer, null, 214);
            starter.AddDialogLine("mv_altair_initiation_explanation", "mv_altair_initiation_explanation", "mv_altair_initiation_choice",
                "{=MVAltairInitiationExplanation}The Shadow's Children hold a base in Quyaz. Join us in breaking it, and you will stand among our brothers and sisters.", null, null, 200);
            starter.AddPlayerLine("mv_altair_initiation_accept", "mv_altair_initiation_choice", "mv_altair_initiation_accepted",
                "{=MVAltairInitiationAccept}I will join the operation.", CanOffer, AcceptQuest, 200);
            starter.AddPlayerLine("mv_altair_initiation_decline", "mv_altair_initiation_choice", "close_window",
                "{=MVAltairInitiationDecline}I need time to prepare.", null, null, 100);
            starter.AddDialogLine("mv_altair_initiation_accepted", "mv_altair_initiation_accepted", "close_window",
                "{=MVAltairInitiationAccepted}Prepare yourself. When you are ready, tell me and we will enter the base.", null, null, 200);
            starter.AddPlayerLine("mv_altair_initiation_ready", "mv_altair_completed_greeting", "mv_altair_initiation_begin",
                "{=MVAltairInitiationReady}I am ready to take part in the operation against the Shadow's Children.", CanChallenge, null, 212);
            starter.AddDialogLine("mv_altair_initiation_begin", "mv_altair_initiation_begin", "close_window",
                "{=MVAltairInitiationBegin}Then let us put their hidden kingdom to the sword.", null, ScheduleFight, 200);
            starter.AddPlayerLine("mv_altair_initiation_report", "mv_altair_completed_greeting", "mv_altair_initiation_success",
                "{=MVAltairInitiationReport}The Shadow's Children have been defeated.", CanReportVictory, null, 220);
            starter.AddDialogLine("mv_altair_initiation_success", "mv_altair_initiation_success", "close_window",
                "{=MVAltairInitiationSuccess}You have fought beside us and held your ground. From this day forward, you are one of the Hashashin.", null, CompleteTrial, 200);

            starter.AddPlayerLine("mv_altair_more_power_request", "mv_altair_completed_greeting", "mv_altair_more_power_reply",
                "{=MVAltairMorePowerRequest}I need more strength from the Hashashin.", CanRequestMorePower, null, 190);
            starter.AddDialogLine("mv_altair_more_power_reply", "mv_altair_more_power_reply", "mv_altair_more_power_choice",
                "{=MVAltairMorePowerReply}A veteran member is worthy of receiving more of our resources. Tell me, what do you need?", null, null, 200);
            starter.AddPlayerLine("mv_altair_more_hands_request", "mv_altair_more_power_choice", "mv_altair_manpower_offer",
                "{=MVAltairMoreHandsRequest}My operations require more hands.", CanRequestManpower, RequestManpower, 200);
            starter.AddPlayerLine("mv_altair_lead_novices", "mv_altair_more_power_choice", "mv_altair_more_power_choice",
                "{=MVAltairLeadNovices}I am willing to lead new recruits through their training.", null, null, 190);
            starter.AddPlayerLine("mv_altair_more_power_leave", "mv_altair_more_power_choice", "close_window",
                "{=MVAltairMorePowerLeave}Nothing for now.", null, null, 100);

            starter.AddDialogLine("mv_altair_manpower_offer_exiles", "mv_altair_manpower_offer", "mv_altair_manpower_choice",
                "{=MVAltairManpowerOfferExiles}Hmm. You should understand that our people are exceedingly valuable. For now, I fear I can only give you some exiles who have sought our protection.", IsExileOffer, null, 200);
            starter.AddDialogLine("mv_altair_manpower_offer_apprentices", "mv_altair_manpower_offer", "mv_altair_manpower_choice",
                "{=MVAltairManpowerOfferApprentices}A group of new recruits happens to need an experienced hand to guide them. Shall I assign them to your company?", IsApprenticeManpowerOffer, null, 200);
            starter.AddPlayerLine("mv_altair_manpower_accept", "mv_altair_manpower_choice", "close_window",
                "{=MVAltairManpowerAccept}I agree.", CanAcceptManpower, AcceptManpower, 200);
            starter.AddPlayerLine("mv_altair_manpower_decline", "mv_altair_manpower_choice", "close_window",
                "{=MVAltairManpowerDecline}Not this time.", null, DeclineManpower, 100);
        }

        private static bool IsTalkingToAltair() => CharacterObject.OneToOneConversationCharacter?.StringId == "mv_altair";

        private static AltairInitiationTrialQuest FindActiveQuest() =>
            Campaign.Current?.QuestManager?.Quests.OfType<AltairInitiationTrialQuest>().FirstOrDefault(q => q.IsOngoing);

        private bool CanOffer() => IsTalkingToAltair() && AltairWisdomTrialBehavior.Instance?.IsCompleted == true && !_completed && FindActiveQuest() == null;
        private bool CanChallenge() => IsTalkingToAltair() && !_completed && !_launchRequested && FindActiveQuest()?.HasWon == false;
        private bool CanReportVictory() => IsTalkingToAltair() && !_completed && FindActiveQuest()?.HasWon == true;
        private bool CanRequestMorePower() => IsTalkingToAltair() && _completed;
        private bool CanRequestManpower() => CanRequestMorePower() && _nextManpowerRequestTime.IsPast;
        private bool IsExileOffer() => _pendingManpowerOffer == 0;
        private bool IsApprenticeManpowerOffer() => _pendingManpowerOffer == 1;
        private bool CanAcceptManpower() => IsTalkingToAltair() && _completed && _pendingManpowerOffer >= 0;

        private void AcceptQuest()
        {
            if (CanOffer())
                new AltairInitiationTrialQuest(Hero.MainHero).StartQuest();
        }

        private void CompleteTrial()
        {
            AltairInitiationTrialQuest quest = FindActiveQuest();
            if (CanReportVictory())
            {
                quest.RecordVictory();
                quest.CompleteQuestWithSuccess();
            }
        }

        internal void MarkCompleted() => _completed = true;

        private void RequestManpower()
        {
            if (!CanRequestManpower())
                return;

            _pendingManpowerOffer = MBRandom.RandomInt(2);
            _nextManpowerRequestTime = CampaignTime.DaysFromNow(ManpowerRequestCooldownDays);
        }

        private void AcceptManpower()
        {
            if (!CanAcceptManpower() || MobileParty.MainParty == null)
                return;

            if (_pendingManpowerOffer == 0)
            {
                AddTroops(DesertBanditId, 10);
                AddTroops(SeaRaiderId, 10);
                AddTroops(SeaRaiderChiefId, 5);
                InformationManager.DisplayMessage(new InformationMessage(new TextObject(
                    "{=MVAltairExilesJoined}Ten desert bandits, ten sea raiders, and five sea raider chiefs have joined your party.").ToString()));
            }
            else
            {
                AddTroops(ApprenticeId, 10);
                InformationManager.DisplayMessage(new InformationMessage(new TextObject(
                    "{=MVAltairApprenticesJoined}Ten Assassin apprentices have joined your party.").ToString()));
            }

            _pendingManpowerOffer = -1;
        }

        private void DeclineManpower()
        {
            _pendingManpowerOffer = -1;
        }

        private static void AddTroops(string characterId, int count)
        {
            CharacterObject troop = CharacterObject.Find(characterId);
            if (troop != null)
                MobileParty.MainParty.MemberRoster.AddToCounts(troop, count, false, 0, 0, false, 0);
        }

        private void ScheduleFight()
        {
            if (!CanChallenge())
                return;
            _trialSettlement = Settlement.CurrentSettlement;
            if (!TryGetTavern(out _, out _))
            {
                ShowUnavailable();
                return;
            }
            _launchRequested = true;
            Campaign.Current.ConversationManager.ConversationEndOneShot += () => _leaveTavernRequested = true;
        }

        private void OnMissionTick(float dt)
        {
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
            AltairInitiationTrialQuest quest = FindActiveQuest();
            if (quest == null || quest.HasWon || !TryGetTavern(out Location tavern, out string scene))
            {
                ShowUnavailable();
                return;
            }
            try { AltairInitiationTrialMission.Open(scene, tavern, quest); }
            catch (Exception ex)
            {
                Debug.Print("[MercenaryVariety] Could not open initiation trial: " + ex);
                ShowUnavailable();
            }
        }

        private bool TryGetTavern(out Location tavern, out string scene)
        {
            tavern = null;
            scene = null;
            if (_trialSettlement == null || Settlement.CurrentSettlement != _trialSettlement || !_trialSettlement.IsTown || _trialSettlement.StringId != "town_A1")
                return false;
            tavern = _trialSettlement.LocationComplex?.GetLocationWithId("tavern");
            scene = tavern?.GetSceneName(_trialSettlement.Town.GetWallLevel());
            return !string.IsNullOrEmpty(scene);
        }

        private static void ShowUnavailable()
        {
            InformationManager.DisplayMessage(new InformationMessage(new TextObject(
                "{=MVAltairInitiationUnavailable}The operation could not begin. Your quest is unchanged; speak to Altair to try again.").ToString()));
        }
    }
}
