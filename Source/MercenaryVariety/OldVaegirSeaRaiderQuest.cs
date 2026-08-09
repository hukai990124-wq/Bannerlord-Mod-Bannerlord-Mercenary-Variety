using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public sealed class OldVaegirSeaRaiderQuest : QuestBase
    {
        public const string QuestId = "mv_old_vaegir_sea_raider_quest";

        private readonly Settlement _targetHideout;

        public OldVaegirSeaRaiderQuest(Hero vasevolod, Settlement targetHideout)
            : base(QuestId, vasevolod, CampaignTime.Never, 0)
        {
            _targetHideout = targetHideout;
        }

        public override TextObject Title =>
            new TextObject("{=MVOldVaegirSeaRaiderQuestTitle}The Old Comrades' Camp");

        public override bool IsRemainingTimeHidden => true;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkSeaRaiderQuestStarted(
                _targetHideout?.StringId);

            if (_targetHideout != null)
            {
                AddTrackedObject(_targetHideout);
            }

            AddLog(
                new TextObject(
                    "{=MVOldVaegirSeaRaiderQuestLog}Find and clear the Sea Raider hideout near Diathma where former Vaegir Guards have taken up piracy."),
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkSeaRaiderQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 20);
            Clan.PlayerClan.AddRenown(20f);
        }
    }
}
