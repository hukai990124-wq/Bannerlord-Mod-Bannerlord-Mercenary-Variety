using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MercenaryVariety
{
    public class OldVaegirGuardsProgressBehavior : CampaignBehaviorBase
    {
        public static OldVaegirGuardsProgressBehavior Instance { get; private set; }

        private bool _foodQuestStarted;
        private bool _foodQuestCompleted;
        private bool _seaRaiderQuestStarted;
        private bool _seaRaiderHideoutCleared;
        private bool _seaRaiderQuestCompleted;
        private string _seaRaiderHideoutId;

        public bool IsFoodQuestStarted => _foodQuestStarted;
        public bool IsFoodQuestCompleted => _foodQuestCompleted;
        public bool IsSeaRaiderQuestStarted => _seaRaiderQuestStarted;
        public bool IsSeaRaiderHideoutCleared => _seaRaiderHideoutCleared;
        public bool IsSeaRaiderQuestCompleted => _seaRaiderQuestCompleted;
        public bool IsT4VaegirRecruitmentUnlocked => _seaRaiderQuestCompleted;
        public string SeaRaiderHideoutId => _seaRaiderHideoutId;

        public OldVaegirGuardsProgressBehavior()
        {
            Instance = this;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnHideoutBattleCompletedEvent.AddNonSerializedListener(
                this,
                OnHideoutBattleCompleted);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(
                "mv_old_vaegir_food_quest_started",
                ref _foodQuestStarted);
            dataStore.SyncData(
                "mv_old_vaegir_food_quest_completed",
                ref _foodQuestCompleted);
            dataStore.SyncData(
                "mv_old_vaegir_sea_raider_quest_started",
                ref _seaRaiderQuestStarted);
            dataStore.SyncData(
                "mv_old_vaegir_sea_raider_hideout_cleared",
                ref _seaRaiderHideoutCleared);
            dataStore.SyncData(
                "mv_old_vaegir_sea_raider_quest_completed",
                ref _seaRaiderQuestCompleted);
            dataStore.SyncData(
                "mv_old_vaegir_sea_raider_hideout_id",
                ref _seaRaiderHideoutId);
        }

        public void MarkFoodQuestStarted()
        {
            _foodQuestStarted = true;
        }

        public void MarkFoodQuestCompleted()
        {
            _foodQuestStarted = true;
            _foodQuestCompleted = true;
        }

        public void MarkSeaRaiderQuestStarted(string hideoutId)
        {
            _seaRaiderQuestStarted = true;
            _seaRaiderHideoutCleared = false;
            _seaRaiderHideoutId = hideoutId;
        }

        public void MarkSeaRaiderHideoutCleared()
        {
            _seaRaiderHideoutCleared = true;
        }

        public void MarkSeaRaiderQuestCompleted()
        {
            _seaRaiderQuestStarted = true;
            _seaRaiderHideoutCleared = true;
            _seaRaiderQuestCompleted = true;
        }

        private void OnHideoutBattleCompleted(
            BattleSideEnum battleSide,
            HideoutEventComponent hideoutEvent,
            HideoutEventComponent.HideoutBattleEndState battleEndState)
        {
            if (!_seaRaiderQuestStarted ||
                _seaRaiderHideoutCleared ||
                _seaRaiderQuestCompleted ||
                battleEndState != HideoutEventComponent.HideoutBattleEndState.Victory ||
                hideoutEvent == null ||
                hideoutEvent.MapEvent == null)
            {
                return;
            }

            Settlement settlement = hideoutEvent.MapEvent.MapEventSettlement;
            if (settlement != null &&
                settlement.IsHideout &&
                settlement.StringId == _seaRaiderHideoutId)
            {
                MarkSeaRaiderHideoutCleared();
            }
        }
    }
}
