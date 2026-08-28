using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public sealed class OldVaegirGovernorQuest : QuestBase
    {
        public const string QuestId = "mv_old_vaegir_governor_quest";

        private readonly MobileParty _targetParty;

        public OldVaegirGovernorQuest(Hero vasevolod, MobileParty targetParty)
            : base(QuestId, vasevolod, CampaignTime.Never, 0)
        {
            _targetParty = targetParty;
        }

        public override TextObject Title =>
            new TextObject("{=MVOldVaegirGovernorQuestTitle}The Governor's Judgment");

        public override bool IsRemainingTimeHidden => true;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkGovernorQuestStarted(
                _targetParty?.StringId);

            if (_targetParty != null)
            {
                AddTrackedObject(_targetParty);
            }

            AddLog(
                new TextObject(
                    "{=MVOldVaegirGovernorQuestLog}Find the Imperial governor's cavalry near Amprela and bring the man who condemned the Vaegir Guard to justice."),
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkGovernorQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 40);
            Clan.PlayerClan.AddRenown(40f);
        }
    }
}
