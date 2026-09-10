using System.Linq;
using TaleWorlds.CampaignSystem;

namespace MercenaryVariety
{
    public sealed class AltairWisdomTrialBehavior : CampaignBehaviorBase
    {
        private bool _completed;
        public static AltairWisdomTrialBehavior Instance { get; private set; }
        public bool IsCompleted => _completed;

        public override void RegisterEvents()
        {
            Instance = this;
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("mv_altair_wisdom_trial_completed", ref _completed);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("mv_altair_wisdom_offer", "mv_altair_completed_greeting", "mv_altair_wisdom_explanation",
                "{=MVAltairWisdomOffer}Surely I am no longer an apprentice now?", CanOfferTrial, null, 205);
            starter.AddDialogLine("mv_altair_wisdom_explanation", "mv_altair_wisdom_explanation", "close_window",
                "{=MVAltairWisdomExplanation}Usually, besides martial prowess, many of our members have skills of their own. And you... you seem to be a person of great ambition. Show us how you would lead a company.", null, AcceptTrial, 200);
            starter.AddPlayerLine("mv_altair_wisdom_report", "mv_altair_completed_greeting", "mv_altair_wisdom_success",
                "{=MVAltairWisdomReport}I think I can prove it.", CanReport, null, 220);
            starter.AddDialogLine("mv_altair_wisdom_success", "mv_altair_wisdom_success", "close_window",
                "{=MVAltairWisdomSuccess}Hmm. This ability shows you are a capable leader. We need someone like you to represent us in the light.", null, CompleteTrial, 200);
        }

        private static bool IsTalkingToAltair() => CharacterObject.OneToOneConversationCharacter?.StringId == "mv_altair";

        internal static AltairWisdomTrialQuest FindActiveQuest()
        {
            return Campaign.Current?.QuestManager?.Quests.OfType<AltairWisdomTrialQuest>().FirstOrDefault(q => q.IsOngoing);
        }

        private bool CanOfferTrial() => IsTalkingToAltair() && AltairMartialTrialBehavior.Instance?.IsCompleted == true && !_completed && FindActiveQuest() == null;
        private bool CanReport() => IsTalkingToAltair() && !_completed && FindActiveQuest()?.IsReadyToTurnIn == true;

        private void AcceptTrial()
        {
            if (CanOfferTrial())
                new AltairWisdomTrialQuest(Hero.MainHero).StartQuest();
        }

        private void CompleteTrial()
        {
            if (!CanReport())
                return;
            AltairWisdomTrialQuest quest = FindActiveQuest();
            quest.RefreshObjective();
            quest.CompleteQuestWithSuccess();
        }

        internal void MarkCompleted() => _completed = true;
    }
}
