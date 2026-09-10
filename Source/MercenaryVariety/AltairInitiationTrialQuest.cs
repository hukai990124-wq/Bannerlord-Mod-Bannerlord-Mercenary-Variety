using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace MercenaryVariety
{
    public sealed class AltairInitiationTrialQuest : QuestBase
    {
        private static readonly string[] RewardItemIds =
        {
            "improved_assassin_hood_q5",
            "improved_assassin_shoulder_q5",
            "improved_assassin_armor_q5",
            "improved_assassin_boot_q5"
        };

        [SaveableField(1)]
        private bool _hasWon;

        public bool HasWon => _hasWon;
        public override TextObject Title => new TextObject("{=MVAltairInitiationTitle}Hashashin Initiation - The Shadow's Children");
        public override bool IsRemainingTimeHidden => true;

        public AltairInitiationTrialQuest(Hero questGiver) : base("mv_altair_initiation_trial", questGiver, CampaignTime.Never, 0) { }
        protected override void SetDialogs() { }
        protected override void InitializeQuestOnGameLoad() { }

        protected override void OnStartQuest()
        {
            AddLog(new TextObject("{=MVAltairInitiationLog}Join the Hashashin operation against the Shadow's Children in Quyaz. Fight alongside three masters, three Hashashins, and two apprentices."), false);
            AddDiscreteLog(new TextObject("{=MVAltairInitiationObjective}Destroy the Shadow's Children guard detail"),
                new TextObject("{=MVAltairInitiationProgress}The guard detail has been defeated"), 0, 1, null, false);
        }

        internal void RecordVictory()
        {
            if (!IsOngoing || _hasWon)
                return;
            _hasWon = true;
            foreach (JournalLog log in JournalEntries)
                if (log.TaskName?.GetID() == "MVAltairInitiationProgress")
                    log.UpdateCurrentProgress(1);
            AddLog(new TextObject("{=MVAltairInitiationReturn}The Shadow's Children have been routed. Return to Altair."), false);
        }

        protected override void OnCompleteWithSuccess()
        {
            GrantRewards();
            AltairInitiationTrialBehavior.Instance?.MarkCompleted();
        }

        private static void GrantRewards()
        {
            if (MobileParty.MainParty == null)
                return;
            foreach (string itemId in RewardItemIds)
            {
                ItemObject item = MBObjectManager.Instance.GetObject<ItemObject>(itemId);
                if (item != null)
                    MobileParty.MainParty.ItemRoster.AddToCounts(item, 1);
            }
            InformationManager.DisplayMessage(new InformationMessage(new TextObject(
                "{=MVAltairInitiationReward}Altair has granted you a high-defense Black assassin set. The prestigious bracers are not included.").ToString()));
        }
    }

    public sealed class AltairInitiationTrialSaveDefiner : SaveableTypeDefiner
    {
        public AltairInitiationTrialSaveDefiner() : base(914260300) { }
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(AltairInitiationTrialQuest), 1);
        }
    }
}
