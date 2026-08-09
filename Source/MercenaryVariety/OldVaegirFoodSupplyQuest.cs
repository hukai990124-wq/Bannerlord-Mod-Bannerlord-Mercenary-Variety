using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public sealed class OldVaegirFoodSupplyQuest : QuestBase
    {
        public const string QuestId = "mv_old_vaegir_food_supply_quest";

        public OldVaegirFoodSupplyQuest(Hero vasevolod)
            : base(QuestId, vasevolod, CampaignTime.Never, 0)
        {
        }

        public override TextObject Title =>
            new TextObject("{=MVOldVaegirFoodQuestTitle}Meat for the Old Vaegir Guards");

        public override bool IsRemainingTimeHidden => true;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkFoodQuestStarted();
            AddLog(
                new TextObject("{=MVOldVaegirFoodQuestLog}Bring 20 units of meat to Vasevolod so the Old Vaegir Guards can smoke it into dried meat."),
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkFoodQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 10);
            Clan.PlayerClan.AddRenown(10f);
        }
    }
}
