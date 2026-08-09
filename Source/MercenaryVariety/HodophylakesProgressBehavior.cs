using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MercenaryVariety
{
    public class HodophylakesProgressBehavior : CampaignBehaviorBase
    {
        public static HodophylakesProgressBehavior Instance { get; private set; }

        private bool _theodoraFoodQuestStarted;
        private bool _theodoraFoodQuestCompleted;
        private bool _theodoraForestBanditQuestStarted;
        private bool _theodoraForestBanditHideoutCleared;
        private bool _theodoraForestBanditQuestCompleted;
        private string _theodoraForestBanditHideoutId;
        private bool _theodoraRebelQuestStarted;
        private bool _theodoraRebelPartyDefeated;
        private bool _theodoraRebelQuestCompleted;
        private string _theodoraRebelPartyId;

        public bool IsTheodoraFoodQuestStarted => _theodoraFoodQuestStarted;
        public bool IsTheodoraFoodQuestCompleted => _theodoraFoodQuestCompleted;
        public bool IsT4SwordSisterRecruitmentUnlocked => _theodoraFoodQuestCompleted;
        public bool IsTheodoraForestBanditQuestStarted => _theodoraForestBanditQuestStarted;
        public bool IsTheodoraForestBanditHideoutCleared => _theodoraForestBanditHideoutCleared;
        public bool IsTheodoraForestBanditQuestCompleted => _theodoraForestBanditQuestCompleted;
        public bool IsT5CrossbowRecruitmentUnlocked => _theodoraForestBanditQuestCompleted;
        public string TheodoraForestBanditHideoutId => _theodoraForestBanditHideoutId;
        public bool IsTheodoraRebelQuestStarted => _theodoraRebelQuestStarted;
        public bool IsTheodoraRebelPartyDefeated => _theodoraRebelPartyDefeated;
        public bool IsTheodoraRebelQuestCompleted => _theodoraRebelQuestCompleted;
        public bool IsT6CrossbowRecruitmentUnlocked => _theodoraRebelQuestCompleted;
        public string TheodoraRebelPartyId => _theodoraRebelPartyId;

        public HodophylakesProgressBehavior()
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
                "mv_hodophylakes_theodora_food_quest_started",
                ref _theodoraFoodQuestStarted);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_food_quest_completed",
                ref _theodoraFoodQuestCompleted);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_forest_bandit_quest_started",
                ref _theodoraForestBanditQuestStarted);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_forest_bandit_hideout_cleared",
                ref _theodoraForestBanditHideoutCleared);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_forest_bandit_quest_completed",
                ref _theodoraForestBanditQuestCompleted);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_forest_bandit_hideout_id",
                ref _theodoraForestBanditHideoutId);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_rebel_quest_started",
                ref _theodoraRebelQuestStarted);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_rebel_party_defeated",
                ref _theodoraRebelPartyDefeated);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_rebel_quest_completed",
                ref _theodoraRebelQuestCompleted);
            dataStore.SyncData(
                "mv_hodophylakes_theodora_rebel_party_id",
                ref _theodoraRebelPartyId);
        }

        public void MarkTheodoraFoodQuestStarted()
        {
            _theodoraFoodQuestStarted = true;
        }

        public void MarkTheodoraFoodQuestCompleted()
        {
            _theodoraFoodQuestStarted = true;
            _theodoraFoodQuestCompleted = true;
        }

        public void MarkTheodoraForestBanditQuestStarted(string hideoutId)
        {
            _theodoraForestBanditQuestStarted = true;
            _theodoraForestBanditHideoutCleared = false;
            _theodoraForestBanditHideoutId = hideoutId;
        }

        public void MarkTheodoraForestBanditHideoutCleared()
        {
            _theodoraForestBanditHideoutCleared = true;
        }

        public void MarkTheodoraForestBanditQuestCompleted()
        {
            _theodoraForestBanditQuestStarted = true;
            _theodoraForestBanditHideoutCleared = true;
            _theodoraForestBanditQuestCompleted = true;
        }

        public void MarkTheodoraRebelQuestStarted(string partyId)
        {
            _theodoraRebelQuestStarted = true;
            _theodoraRebelPartyDefeated = false;
            _theodoraRebelPartyId = partyId;
        }

        public void MarkTheodoraRebelPartyDefeated()
        {
            _theodoraRebelPartyDefeated = true;
        }

        public void MarkTheodoraRebelQuestCompleted()
        {
            _theodoraRebelQuestStarted = true;
            _theodoraRebelPartyDefeated = true;
            _theodoraRebelQuestCompleted = true;
        }

        private void OnHideoutBattleCompleted(
            BattleSideEnum battleSide,
            HideoutEventComponent hideoutEvent,
            HideoutEventComponent.HideoutBattleEndState battleEndState)
        {
            if (!_theodoraForestBanditQuestStarted ||
                _theodoraForestBanditHideoutCleared ||
                _theodoraForestBanditQuestCompleted ||
                battleEndState != HideoutEventComponent.HideoutBattleEndState.Victory ||
                hideoutEvent == null ||
                hideoutEvent.MapEvent == null)
            {
                return;
            }

            Settlement settlement = hideoutEvent.MapEvent.MapEventSettlement;
            if (settlement != null &&
                settlement.IsHideout &&
                settlement.StringId == _theodoraForestBanditHideoutId)
            {
                MarkTheodoraForestBanditHideoutCleared();
            }
        }

        private void OnMobilePartyDestroyed(MobileParty destroyedParty, PartyBase destroyerParty)
        {
            if (!_theodoraRebelQuestStarted ||
                _theodoraRebelPartyDefeated ||
                _theodoraRebelQuestCompleted ||
                destroyedParty == null ||
                destroyedParty.StringId != _theodoraRebelPartyId)
            {
                return;
            }

            MarkTheodoraRebelPartyDefeated();
        }
    }
}
