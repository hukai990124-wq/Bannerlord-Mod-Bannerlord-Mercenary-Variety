using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public sealed class TheodoraFoodSupplyQuest : QuestBase
    {
        public const string QuestId = "mv_theodora_food_supply_quest";

        public TheodoraFoodSupplyQuest(Hero theodora)
            : base(QuestId, theodora, CampaignTime.Never, 0)
        {
        }

        public override TextObject Title =>
            new TextObject("{=MVTheodoraFoodSupplyQuestTitle}Food Supplies for the Hodophylakes");

        public override bool IsRemainingTimeHidden => true;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraFoodQuestStarted();
            AddLog(
                new TextObject("{=MVTheodoraFoodSupplyQuestLog}Bring 40 units of grain to Theodora for the Hodophylakes patrols."),
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraFoodQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 20);
            Clan.PlayerClan.AddRenown(10f);
        }
    }
}
