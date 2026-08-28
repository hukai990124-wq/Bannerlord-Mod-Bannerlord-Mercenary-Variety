using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace MercenaryVariety
{
    public sealed class OldVaegirRecordsQuest : QuestBase
    {
        public const string QuestId = "mv_old_vaegir_records_quest";

        private readonly MobileParty _targetParty;

        public OldVaegirRecordsQuest(Hero vasevolod, MobileParty targetParty)
            : base(QuestId, vasevolod, CampaignTime.Never, 0)
        {
            _targetParty = targetParty;
        }

        public override TextObject Title =>
            new TextObject("{=MVOldVaegirRecordsQuestTitle}The Empire's Old Debt");

        public override bool IsRemainingTimeHidden => true;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkRecordsQuestStarted(
                _targetParty?.StringId);

            if (_targetParty != null)
            {
                AddTrackedObject(_targetParty);
            }

            AddLog(
                new TextObject(
                    "{=MVOldVaegirRecordsQuestLog}Find the former Imperial deserters near Diathma and recover the Vaegir Guard's muster rolls and unpaid wage ledgers."),
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkRecordsQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 30);
            Clan.PlayerClan.AddRenown(30f);

            ItemObject commanderHelmet = MBObjectManager.Instance.GetObject<ItemObject>(
                "mv_vaegir_guard_commander_helmet");
            if (commanderHelmet != null && MobileParty.MainParty != null)
            {
                MobileParty.MainParty.ItemRoster.AddToCounts(commanderHelmet, 1);
                InformationManager.DisplayMessage(
                    new InformationMessage(
                        new TextObject(
                            "{=MVVaegirCommanderHelmetRewardMessage}Vasevolod entrusts the Vaegir Guard Commander's Helmet to you as proof that the Guard's service has been restored to the record.")
                        .ToString()));
            }
        }

        public void ResolveByPayment()
        {
            ResolveAndComplete();
        }

        public void ResolveByNegotiation()
        {
            ResolveAndComplete();
        }

        private void ResolveAndComplete()
        {
            OldVaegirGuardsProgressBehavior.Instance?.MarkRecordsPartyDefeated();
            if (_targetParty != null)
            {
                DestroyPartyAction.Apply(null, _targetParty);
            }

            CompleteQuestWithSuccess();
        }
    }
}
