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
                "{=MVHodophylakesAskIdentity}What kind of company are you?",
                IsTalkingToHodophylakes,
                null,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_identity_answer",
                "mv_hodophylakes_identity_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesIdentityAnswer}We are the Hodophylakes Patrol, road-wardens and bounty hunters of the southern Imperial roads. Our charge is to hunt looters, forest bandits, and every cutthroat who preys upon travelers, villages, and caravans between Lycaron and Danustica. We swear no oath to any throne, and we take no contract in wars between realms. Princes have armies enough. The common folk on the roads need someone to watch over them.",
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
