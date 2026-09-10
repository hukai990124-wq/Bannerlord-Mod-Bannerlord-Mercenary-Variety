using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace MercenaryVariety
{
    public sealed class AltairTrialQuest : QuestBase
    {
        public const string QuestId = "mv_altair_trial";

        private const int RequiredEnemyDefeats = 50;
        private const int RequiredClanRenown = 30;

        private const string EnemyDefeatsTaskId = "MVAltairTrialTaskEnemies";
        private const string RenownTaskId = "MVAltairTrialTaskRenown";

        private static readonly string[] TrialRewardItemIds =
        {
            "assassin_hood",
            "assassin_armor",
            "assassin_shoulder",
            "assassin_boot"
        };

        public AltairTrialQuest(Hero questGiver)
            : base(QuestId, questGiver, CampaignTime.Never, 0)
        {
        }

        public override TextObject Title =>
            new TextObject("{=MVAltairTrialTitle}Trial of the Assassins");

        public override bool IsRemainingTimeHidden => true;

        public bool IsReadyToTurnIn =>
            AltairTavernBehavior.Instance != null &&
            AltairTavernBehavior.Instance.TrialEnemyDefeats >= RequiredEnemyDefeats &&
            AltairTavernBehavior.Instance.TrialRenownGained >= RequiredClanRenown;

        protected override void SetDialogs()
        {
        }

        protected override void InitializeQuestOnGameLoad()
        {
            RegisterBattleEndListener();
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        }

        protected override void OnStartQuest()
        {
            RegisterBattleEndListener();
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);

            AddLog(
                new TextObject(
                    "{=MVAltairTrialLog}Prove your worth to Altair: defeat 50 enemies and gain 30 clan renown."),
                false);

            AddDiscreteLog(
                new TextObject("{=MVAltairTrialProgressEnemies}Defeat 50 enemies"),
                new TextObject("{=MVAltairTrialTaskEnemies}Enemies defeated"),
                0,
                RequiredEnemyDefeats,
                null,
                false);

            AddDiscreteLog(
                new TextObject("{=MVAltairTrialProgressRenown}Gain 30 clan renown"),
                new TextObject("{=MVAltairTrialTaskRenown}Clan renown"),
                0,
                RequiredClanRenown,
                null,
                false);
        }

        protected override void OnCompleteWithSuccess()
        {
            GrantTrialRewards();
            AltairTavernBehavior.Instance?.MarkTrialCompleted();
        }

        private static void GrantTrialRewards()
        {
            if (MobileParty.MainParty == null)
            {
                return;
            }

            foreach (string itemId in TrialRewardItemIds)
            {
                ItemObject item = MBObjectManager.Instance.GetObject<ItemObject>(itemId);
                if (item != null)
                {
                    MobileParty.MainParty.ItemRoster.AddToCounts(item, 1);
                }
            }
        }

        private void RegisterBattleEndListener()
        {
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(
                this,
                OnPlayerBattleEnd);
        }

        private void OnPlayerBattleEnd(MapEvent mapEvent)
        {
            if (mapEvent == null || !mapEvent.IsPlayerMapEvent)
            {
                return;
            }

            BattleSideEnum playerSide = mapEvent.PlayerSide;
            BattleSideEnum enemySide = playerSide == BattleSideEnum.Attacker
                ? BattleSideEnum.Defender
                : BattleSideEnum.Attacker;

            MapEventSide enemySideData = mapEvent.GetMapEventSide(enemySide);
            if (enemySideData != null)
            {
                AltairTavernBehavior.Instance?.RecordEnemyDefeats(enemySideData.TroopCasualties);
            }

            RefreshQuestProgress();
        }

        private void OnHourlyTick()
        {
            RefreshQuestProgress();
        }

        private void RefreshQuestProgress()
        {
            if (AltairTavernBehavior.Instance == null)
            {
                return;
            }

            int enemies = AltairTavernBehavior.Instance.TrialEnemyDefeats;
            int renown = AltairTavernBehavior.Instance.TrialRenownGained;

            foreach (JournalLog log in JournalEntries)
            {
                if (log.TaskName == null)
                {
                    continue;
                }

                string taskId = log.TaskName.GetID();
                if (taskId == EnemyDefeatsTaskId)
                {
                    log.UpdateCurrentProgress(enemies);
                }
                else if (taskId == RenownTaskId)
                {
                    log.UpdateCurrentProgress(renown);
                }
                else if (string.IsNullOrEmpty(taskId) && log.Range > 0)
                {
                    if (log.Range == RequiredEnemyDefeats)
                    {
                        log.UpdateCurrentProgress(enemies);
                    }
                    else if (log.Range == RequiredClanRenown)
                    {
                        log.UpdateCurrentProgress(renown);
                    }
                }
            }
        }
    }
}
