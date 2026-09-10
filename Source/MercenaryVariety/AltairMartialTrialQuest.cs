using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace MercenaryVariety
{
    public sealed class AltairMartialTrialQuest : QuestBase
    {
        [SaveableField(1)]
        private bool _hasWon;

        public bool HasWon => _hasWon;
        public override TextObject Title => new TextObject("{=MVAltairMartialTitle}Hashashin Initiate Trial - Martial Skill");
        public override bool IsRemainingTimeHidden => true;

        // Altair is a non-hero tavern character; follow the first trial's quest-giver convention.
        public AltairMartialTrialQuest(Hero questGiver) : base("mv_altair_martial_trial", questGiver, CampaignTime.Never, 0) { }
        protected override void SetDialogs() { }
        protected override void InitializeQuestOnGameLoad() { }

        protected override void OnStartQuest()
        {
            AddLog(new TextObject("{=MVAltairMartialLog}Altair has agreed to test your martial skill. Prepare your equipment, then tell him you are ready. Defeat two Assassin Apprentices at the same time in the arena and return to Altair. You may retry if you lose."), false);
            AddDiscreteLog(new TextObject("{=MVAltairMartialObjective}Win the one-against-two arena trial"),
                new TextObject("{=MVAltairMartialProgress}Arena trial won"), 0, 1, null, false);
        }

        internal void RecordVictory()
        {
            if (!IsOngoing || _hasWon)
                return;
            _hasWon = true;
            foreach (JournalLog log in JournalEntries)
                if (log.TaskName?.GetID() == "MVAltairMartialProgress")
                    log.UpdateCurrentProgress(1);
            AddLog(new TextObject("{=MVAltairMartialReturn}You defeated both apprentices. Return to Altair and tell him you have proven your personal martial skill."), false);
        }

        protected override void OnCompleteWithSuccess()
        {
            AltairMartialTrialBehavior.Instance?.MarkCompleted();
        }
    }

    public sealed class AltairMartialTrialSaveDefiner : SaveableTypeDefiner
    {
        public AltairMartialTrialSaveDefiner() : base(914260000) { }
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(AltairMartialTrialQuest), 1);
            AddClassDefinition(typeof(AltairWisdomTrialQuest), 2);
        }
    }
}
