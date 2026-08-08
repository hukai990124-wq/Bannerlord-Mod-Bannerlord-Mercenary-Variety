using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace MercenaryVariety
{
    public class OldVaegirGuardsDialogBehavior : CampaignBehaviorBase
    {
        private const string OldVaegirGuardsClanId = "mv_old_vaegir_guards";

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
                "mv_old_vaegir_ask_why_together",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_answer_why_together",
                "{=MVOldVaegirAskWhyTogether}I hear the Empire dismissed you. Why are you still operating together? Do you serve anyone now?",
                IsTalkingToOldVaegirGuards,
                null,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_answer_why_together",
                "mv_old_vaegir_answer_why_together",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirAnswerWhyTogether}Most of us have spent the greater part of our lives making a living by the sword. Dismissal from Imperial service did not give us another trade, so we still take work by force of arms. At least, while we remain one company, we can win larger contracts and find more orders than any of us could secure alone. That is why we continue to march together, even after the Empire cast us out.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_offer_help",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_offer_help_answer",
                "{=MVOldVaegirOfferHelp}Is there anything I can help with?",
                IsTalkingToOldVaegirGuards,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_offer_help_answer",
                "mv_old_vaegir_offer_help_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirOfferHelpAnswer}We don't need your help with anything.",
                null,
                null,
                120);
        }

        private static bool IsTalkingToOldVaegirGuards()
        {
            Hero hero = Hero.OneToOneConversationHero;
            return hero != null && hero.Clan != null && hero.Clan.StringId == OldVaegirGuardsClanId;
        }
    }
}
