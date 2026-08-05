using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace MercenaryVariety
{
    public class HodophylakesDialogBehavior : CampaignBehaviorBase
    {
        private const string HodophylakesClanId = "mv_hodophylakes_patrol";

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_ask_identity",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_identity_answer",
                "{=MVHodophylakesAskIdentity}你们是什么样的组织？",
                IsTalkingToHodophylakes,
                null,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_identity_answer",
                "mv_hodophylakes_identity_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesIdentityAnswer}我们是霍多菲拉克斯巡逻队，南帝国道路上的赏金猎人与护路人。我们的职责是追捕劫匪、绿林强盗和袭击旅人的亡命之徒，保护吕卡隆到达努斯提卡之间的村庄、商队与行人。我们不向任何王位宣誓，也不接受国家战争的雇佣契约；王公们自有军队，而道路上的百姓需要有人守望。",
                null,
                null,
                120);
        }

        private static bool IsTalkingToHodophylakes()
        {
            Hero hero = Hero.OneToOneConversationHero;
            return hero != null && hero.Clan != null && hero.Clan.StringId == HodophylakesClanId;
        }
    }
}
