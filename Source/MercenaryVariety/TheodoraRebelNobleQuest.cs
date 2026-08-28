using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace MercenaryVariety
{
    public sealed class TheodoraRebelNobleQuest : QuestBase
    {
        public const string QuestId = "mv_theodora_rebel_noble_quest";

        private readonly MobileParty _targetParty;

        public TheodoraRebelNobleQuest(Hero theodora, MobileParty targetParty)
            : base(QuestId, theodora, CampaignTime.Never, 0)
        {
            _targetParty = targetParty;
        }

        public override TextObject Title =>
            new TextObject("{=MVTheodoraRebelNobleQuestTitle}The Lycaron Revolt");

        public override bool IsRemainingTimeHidden => true;

        public MobileParty TargetParty => _targetParty;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
        }

        protected override void OnStartQuest()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraRebelQuestStarted(
                _targetParty?.StringId);

            if (_targetParty != null)
            {
                AddTrackedObject(_targetParty);
            }

            AddLog(
                new TextObject(
                    "{=MVTheodoraRebelNobleQuestLog}Defeat the rebel noble party operating near Lycaron and end its attacks on the roads.") ,
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraRebelQuestCompleted();
            ChangeRelationAction.ApplyPlayerRelation(QuestGiver, 20);
            Clan.PlayerClan.AddRenown(20f);

            if (MobileParty.MainParty == null)
            {
                return;
            }

            ItemObject tribune = MBObjectManager.Instance.GetObject<ItemObject>("mv_the_tribune");
            ItemObject unification = MBObjectManager.Instance.GetObject<ItemObject>("mv_the_unification");

            if (tribune != null)
            {
                MobileParty.MainParty.ItemRoster.AddToCounts(tribune, 1);
            }

            if (unification != null)
            {
                MobileParty.MainParty.ItemRoster.AddToCounts(unification, 1);
            }

            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVTheodoraTribuneRewardMessage}Your courage and justice have earned the people's admiration. They call you the Tribune and believe you are the one who can unite the Empire once more and bring this age of chaos to an end.")
                    .ToString()));
        }

        public void ResolveByDisbanding()
        {
            HodophylakesProgressBehavior.Instance?.MarkTheodoraRebelPartyDefeated();
            if (_targetParty != null)
            {
                DestroyPartyAction.Apply(null, _targetParty);
            }
            CompleteQuestWithSuccess();
        }

        public void ResolveByRecruiting()
        {
            if (_targetParty != null && MobileParty.MainParty != null)
            {
                foreach (TroopRosterElement element in _targetParty.MemberRoster.GetTroopRoster())
                {
                    if (element.Character != null && element.Number > 0)
                    {
                        MobileParty.MainParty.MemberRoster.AddToCounts(
                            element.Character,
                            element.Number,
                            false,
                            element.WoundedNumber,
                            0,
                            true,
                            -1);
                    }
                }
            }

            HodophylakesProgressBehavior.Instance?.MarkTheodoraRebelPartyDefeated();
            if (_targetParty != null)
            {
                DestroyPartyAction.Apply(null, _targetParty);
            }
            CompleteQuestWithSuccess();
        }
    }
}
