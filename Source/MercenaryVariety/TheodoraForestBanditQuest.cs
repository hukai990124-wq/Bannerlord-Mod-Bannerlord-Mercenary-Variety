using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public sealed class TheodoraForestBanditQuest : QuestBase
    {
        public const string QuestId = "mv_theodora_forest_bandit_quest";

        private readonly Settlement _targetHideout;

        public TheodoraForestBanditQuest(Hero theodora, Settlement targetHideout)
            : base(QuestId, theodora, CampaignTime.Never, 0)
        {
            _targetHideout = targetHideout;
        }

        public override TextObject Title =>
            new TextObject("{=MVTheodoraForestBanditQuestTitle}The Forest Bandit Menace");

        public override bool IsRemainingTimeHidden => true;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraForestBanditQuestStarted(
                _targetHideout?.StringId);

            if (_targetHideout != null)
            {
                AddTrackedObject(_targetHideout);
            }

            AddLog(
                new TextObject(
                    "{=MVTheodoraForestBanditQuestLog}Find and clear the forest bandit hideout that has been attacking the Hodophylakes patrols near Danustica."),
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraForestBanditQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 20);
            Clan.PlayerClan.AddRenown(20f);
        }
    }
}
