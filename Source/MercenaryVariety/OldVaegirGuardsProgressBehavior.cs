using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
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
        private bool _recordsQuestStarted;
        private bool _recordsPartyDefeated;
        private bool _recordsQuestCompleted;
        private string _recordsPartyId;
        private bool _governorQuestStarted;
        private bool _governorPartyDefeated;
        private bool _governorQuestCompleted;
        private string _governorPartyId;
        private bool _oldVaegirMercenaryHired;

        public bool IsFoodQuestStarted => _foodQuestStarted;
        public bool IsFoodQuestCompleted => _foodQuestCompleted;
        public bool IsSeaRaiderQuestStarted => _seaRaiderQuestStarted;
        public bool IsSeaRaiderHideoutCleared => _seaRaiderHideoutCleared;
        public bool IsSeaRaiderQuestCompleted => _seaRaiderQuestCompleted;
        public bool IsT4VaegirRecruitmentUnlocked => _seaRaiderQuestCompleted;
        public string SeaRaiderHideoutId => _seaRaiderHideoutId;
        public bool IsRecordsQuestStarted => _recordsQuestStarted;
        public bool IsRecordsPartyDefeated => _recordsPartyDefeated;
        public bool IsRecordsQuestCompleted => _recordsQuestCompleted;
        public bool IsT5VaegirRecruitmentUnlocked => _recordsQuestCompleted;
        public string RecordsPartyId => _recordsPartyId;
        public bool IsGovernorQuestStarted => _governorQuestStarted;
        public bool IsGovernorPartyDefeated => _governorPartyDefeated;
        public bool IsGovernorQuestCompleted => _governorQuestCompleted;
        public bool IsT6VaegirRecruitmentUnlocked => _governorQuestCompleted;
        public string GovernorPartyId => _governorPartyId;
        public bool IsOldVaegirMercenaryHired => _oldVaegirMercenaryHired;

        public OldVaegirGuardsProgressBehavior()
        {
            Instance = this;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnHideoutBattleCompletedEvent.AddNonSerializedListener(
                this,
                OnHideoutBattleCompleted);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(
                this,
                OnMobilePartyDestroyed);
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
            dataStore.SyncData(
                "mv_old_vaegir_records_quest_started",
                ref _recordsQuestStarted);
            dataStore.SyncData(
                "mv_old_vaegir_records_party_defeated",
                ref _recordsPartyDefeated);
            dataStore.SyncData(
                "mv_old_vaegir_records_quest_completed",
                ref _recordsQuestCompleted);
            dataStore.SyncData(
                "mv_old_vaegir_records_party_id",
                ref _recordsPartyId);
            dataStore.SyncData(
                "mv_old_vaegir_governor_quest_started",
                ref _governorQuestStarted);
            dataStore.SyncData(
                "mv_old_vaegir_governor_party_defeated",
                ref _governorPartyDefeated);
            dataStore.SyncData(
                "mv_old_vaegir_governor_quest_completed",
                ref _governorQuestCompleted);
            dataStore.SyncData(
                "mv_old_vaegir_governor_party_id",
                ref _governorPartyId);
            dataStore.SyncData(
                "mv_old_vaegir_mercenary_hired",
                ref _oldVaegirMercenaryHired);
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

        public void MarkRecordsQuestStarted(string partyId)
        {
            _recordsQuestStarted = true;
            _recordsPartyDefeated = false;
            _recordsPartyId = partyId;
        }

        public void MarkRecordsPartyDefeated()
        {
            _recordsPartyDefeated = true;
        }

        public void MarkRecordsQuestCompleted()
        {
            _recordsQuestStarted = true;
            _recordsPartyDefeated = true;
            _recordsQuestCompleted = true;
        }

        public void MarkGovernorQuestStarted(string partyId)
        {
            _governorQuestStarted = true;
            _governorPartyDefeated = false;
            _governorPartyId = partyId;
        }

        public void MarkGovernorPartyDefeated()
        {
            _governorPartyDefeated = true;
        }

        public void MarkGovernorQuestCompleted()
        {
            _governorQuestStarted = true;
            _governorPartyDefeated = true;
            _governorQuestCompleted = true;
        }

        public void MarkOldVaegirMercenaryHired()
        {
            _oldVaegirMercenaryHired = true;
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

        private void OnMobilePartyDestroyed(MobileParty destroyedParty, PartyBase destroyerParty)
        {
            if (destroyedParty == null)
            {
                return;
            }

            if (_recordsQuestStarted &&
                !_recordsPartyDefeated &&
                !_recordsQuestCompleted &&
                destroyedParty.StringId == _recordsPartyId)
            {
                MarkRecordsPartyDefeated();
                return;
            }

            if (_governorQuestStarted &&
                !_governorPartyDefeated &&
                !_governorQuestCompleted &&
                destroyedParty.StringId == _governorPartyId)
            {
                MarkGovernorPartyDefeated();
            }
        }
    }
}
