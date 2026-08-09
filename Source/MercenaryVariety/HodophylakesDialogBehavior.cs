using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public class HodophylakesDialogBehavior : CampaignBehaviorBase
    {
        private const string HodophylakesClanId = "mv_hodophylakes_patrol";
        private const string TheodoraHeroId = "mv_hodophylakes_leader_0";
        private const string DanusticaId = "town_ES1";
        private const string LycaronId = "town_ES4";
        private const string ForestBanditCultureId = "forest_bandits";
        private const int RequiredFoodUnits = 40;
        private const string RequiredFoodItemId = "grain";
        private const float MaxForestBanditHideoutDistanceFromDanustica = 30f;
        private const string RebelPartyId = "mv_theodora_rebel_party";
        private const int RebelPartySize = 120;
        private const int RebelDisbandFee = 50000;

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

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_offer_help",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_offer_help_answer",
                "{=MVHodophylakesOfferHelp}Is there anything I can help with?",
                CanOfferGenericHelp,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_offer_help_answer",
                "mv_hodophylakes_offer_help_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesOfferHelpAnswer}Thank you, but we do not need any help at the moment. Thank you for your concern.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_offer_food_help",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_offer_food_help_answer",
                "{=MVHodophylakesOfferHelp}Is there anything I can help with?",
                CanOfferFoodQuest,
                StartTheodoraFoodQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_offer_food_help_answer",
                "mv_hodophylakes_offer_food_help_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesOfferFoodHelpAnswer}There is, actually. Our patrols have been stretched thin, and our stores are running low. Bring us forty units of grain, and we will put it to good use.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_deliver_food",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_deliver_food_answer",
                "{=MVHodophylakesDeliverFood}I have brought the food supplies you requested.",
                CanDeliverFoodSupplies,
                CompleteTheodoraFoodSupplyQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_deliver_food_answer",
                "mv_hodophylakes_deliver_food_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesDeliverFoodAnswer}Good. This will keep our people fed and our patrols moving. You have our thanks.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_food_not_ready",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_food_not_ready_answer",
                "{=MVHodophylakesFoodNotReady}I am still gathering the supplies.",
                IsFoodQuestActiveWithoutSupplies,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_food_not_ready_answer",
                "mv_hodophylakes_food_not_ready_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesFoodNotReadyAnswer}Then come back when you have it. We cannot feed a patrol on promises.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_offer_forest_bandit_help",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_forest_bandit_context",
                "{=MVHodophylakesOfferHelp}Is there anything I can help with?",
                CanOfferForestBanditQuest,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_forest_bandit_context",
                "mv_hodophylakes_forest_bandit_context",
                "mv_hodophylakes_forest_bandit_quest_prompt",
                "{=MVHodophylakesForestBanditContext}I am still carrying out our daily patrols, protecting merchants and villagers along the roads. But a band of forest bandits has begun to trouble us.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_offer_forest_bandit_quest",
                "mv_hodophylakes_forest_bandit_quest_prompt",
                "mv_hodophylakes_offer_forest_bandit_quest_answer",
                "{=MVHodophylakesOfferForestBanditQuest}You mentioned a group of forest bandits causing trouble. What can I do?",
                CanOfferForestBanditQuest,
                StartTheodoraForestBanditQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_offer_forest_bandit_quest_answer",
                "mv_hodophylakes_offer_forest_bandit_quest_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesOfferForestBanditQuestAnswer}There is a band of forest bandits that has made a habit of attacking our patrols. They have taken supplies, ambushed our people, and left warnings along the road. Find their hideout and put an end to them.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_report_forest_bandit_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_report_forest_bandit_quest_answer",
                "{=MVHodophylakesReportForestBanditQuest}The forest bandit hideout is no more.",
                CanCompleteTheodoraForestBanditQuest,
                CompleteTheodoraForestBanditQuest,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_report_forest_bandit_quest_answer",
                "mv_hodophylakes_report_forest_bandit_quest_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesReportForestBanditQuestAnswer}Then the road will breathe easier. You have done the Hodophylakes a service we will not forget.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_forest_bandit_quest_in_progress",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_forest_bandit_quest_in_progress_answer",
                "{=MVHodophylakesForestBanditQuestInProgress}I am still hunting the forest bandits.",
                IsTheodoraForestBanditQuestActiveWithoutClear,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_forest_bandit_quest_in_progress_answer",
                "mv_hodophylakes_forest_bandit_quest_in_progress_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesForestBanditQuestInProgressAnswer}Their camp still stands. Return when you have dealt with them.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_offer_rebel_noble_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_rebel_noble_quest_part1",
                "{=MVHodophylakesOfferRebelNobleQuest}Is there another matter troubling the roads?",
                CanOfferRebelNobleQuest,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_rebel_noble_quest_part1",
                "mv_hodophylakes_rebel_noble_quest_part1",
                "mv_hodophylakes_rebel_noble_quest_part2_prompt",
                "{=MVHodophylakesRebelNobleQuestPart1}There is a more dangerous threat near Lycaron. A group of nobles has raised its banners in revolt. They call themselves defenders of the Empire, but their soldiers have been stopping caravans, stripping villages of grain, and demanding obedience from every traveler who passes beneath their colors.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_rebel_noble_quest_part2",
                "mv_hodophylakes_rebel_noble_quest_part2_prompt",
                "mv_hodophylakes_rebel_noble_quest_part2_answer",
                "{=MVHodophylakesRebelNobleQuestPart2Prompt}A noble revolt? Why would the Hodophylakes involve themselves in an imperial dispute?",
                null,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_rebel_noble_quest_part2_answer",
                "mv_hodophylakes_rebel_noble_quest_part2_answer",
                "mv_hodophylakes_rebel_noble_quest_part3_prompt",
                "{=MVHodophylakesRebelNobleQuestPart2Answer}We would not. We have no claim to a throne and no wish to choose which imperial claimant deserves the crown. But their revolt has ceased to be a matter between nobles. They use their titles as a license to prey upon the same people our patrols are sworn to protect. A noble who taxes a road at swordpoint is still a bandit, even when his seal is made of gold.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_rebel_noble_quest_part3",
                "mv_hodophylakes_rebel_noble_quest_part3_prompt",
                "mv_hodophylakes_rebel_noble_quest_part3_answer",
                "{=MVHodophylakesRebelNobleQuestPart3Prompt}If they are rebels against the Empire, why does no imperial army deal with them?",
                null,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_rebel_noble_quest_part3_answer",
                "mv_hodophylakes_rebel_noble_quest_part3_answer",
                "mv_hodophylakes_rebel_noble_quest_part4_prompt",
                "{=MVHodophylakesRebelNobleQuestPart3Answer}Because every imperial army is already being asked to fight somewhere else. Lycaron is a prize to commanders and a burden to everyone who lives beyond its walls. The court argues over legitimacy while these nobles gather men, seize supplies, and turn the roads into their private frontier. We cannot march beneath an imperial banner, but we can prevent an armed faction from making our routes unsafe.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_rebel_noble_quest_part4",
                "mv_hodophylakes_rebel_noble_quest_part4_prompt",
                "mv_hodophylakes_rebel_noble_quest_part4_answer",
                "{=MVHodophylakesRebelNobleQuestPart4Prompt}What exactly do you want me to do?",
                null,
                null,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_rebel_noble_quest_part4_answer",
                "mv_hodophylakes_rebel_noble_quest_part4_answer",
                "mv_hodophylakes_rebel_noble_quest_accept_prompt",
                "{=MVHodophylakesRebelNobleQuestPart4Answer}Find the rebel noble party operating near Lycaron and break it. I do not ask you to conquer a fief or swear yourself to a claimant. I ask you to end the force that is using rebellion as an excuse to terrorize the roads. Once their party is gone, their threat will be broken, whether the lords in their palaces choose to admit it or not.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_accept_rebel_noble_quest",
                "mv_hodophylakes_rebel_noble_quest_accept_prompt",
                "mv_hodophylakes_rebel_noble_quest_accept_answer",
                "{=MVHodophylakesAcceptRebelNobleQuest}I will deal with them.",
                null,
                StartTheodoraRebelNobleQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_rebel_noble_quest_accept_answer",
                "mv_hodophylakes_rebel_noble_quest_accept_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesAcceptRebelNobleQuestAnswer}Then go carefully. A rebel lord may be dangerous, but he is still a lord, and that makes him more likely to believe his own legend. Break his party, and the people near Lycaron will remember who actually kept the road open.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_report_rebel_noble_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_report_rebel_noble_quest_answer",
                "{=MVHodophylakesReportRebelNobleQuest}The rebel noble party near Lycaron has been destroyed.",
                CanCompleteTheodoraRebelNobleQuest,
                CompleteTheodoraRebelNobleQuest,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_report_rebel_noble_quest_answer",
                "mv_hodophylakes_report_rebel_noble_quest_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesReportRebelNobleQuestAnswer}Then you have done something that armies and courts failed to do. You did not choose a claimant; you chose the people who had to travel those roads. The Hodophylakes will remember that distinction.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_hodophylakes_rebel_noble_quest_in_progress",
                "lord_talk_speak_diplomacy_2",
                "mv_hodophylakes_rebel_noble_quest_in_progress_answer",
                "{=MVHodophylakesRebelNobleQuestInProgress}I am still pursuing the rebel nobles near Lycaron.",
                IsTheodoraRebelNobleQuestActiveWithoutDefeat,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_hodophylakes_rebel_noble_quest_in_progress_answer",
                "mv_hodophylakes_rebel_noble_quest_in_progress_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVHodophylakesRebelNobleQuestInProgressAnswer}Their banners are still on the road. Do not let them turn a rebellion into a permanent kingdom of tolls and fear.",
                null,
                null,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_theodora_rebel_party_encounter_start",
                "start",
                "mv_theodora_rebel_party_options",
                "{=MVTheodoraRebelPartyIntroduction}Our rights come from the late Emperor Arenikos himself. Our blood is noble, our status beyond question, and every loyal Imperial citizen owes us the taxes we are due.",
                IsTheodoraRebelPartyEncounter,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_fight",
                "mv_theodora_rebel_party_options",
                "mv_theodora_rebel_party_fight_answer",
                "{=MVTheodoraRebelPartyFight}You nobles have preyed upon and oppressed villagers and caravans long enough. You will answer for it now!",
                IsTheodoraRebelPartyEncounter,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_theodora_rebel_party_fight_answer",
                "mv_theodora_rebel_party_fight_answer",
                "close_window",
                "{=MVTheodoraRebelPartyFightAnswer}Very well, then. We shall be the ones to pass judgment on you.",
                null,
                StartRebelPartyBattle,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_disband",
                "mv_theodora_rebel_party_options",
                "mv_theodora_rebel_party_disband_answer",
                "{=MVTheodoraRebelPartyDisband}You know your cause has no future. I can offer you a generous settlement. Lay down your arms and end this.",
                IsTheodoraRebelPartyEncounter,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_theodora_rebel_party_disband_answer",
                "mv_theodora_rebel_party_disband_answer",
                "mv_theodora_rebel_party_disband_options",
                "{=MVTheodoraRebelPartyDisbandAnswer}We can disband, of course. We could return to the fields, take up a trade, or sell our swords as mercenaries. But we need enough to start over. Fifty thousand denars should cover our expenses.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_pay_disband",
                "mv_theodora_rebel_party_disband_options",
                "close_window",
                "{=MVTheodoraRebelPartyPayDisband}Pay 50,000 denars and have them disband.",
                CanPayRebelDisbandFee,
                PayAndDisbandRebelParty,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_cannot_pay_disband",
                "mv_theodora_rebel_party_disband_options",
                "close_window",
                "{=MVTheodoraRebelPartyCannotPayDisband}I do not have 50,000 denars to spare.",
                CanNotPayRebelDisbandFee,
                LeaveRebelPartyEncounter,
                100);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_cancel_disband",
                "mv_theodora_rebel_party_disband_options",
                "close_window",
                "{=MVTheodoraRebelPartyCancelDisband}I will reconsider this offer.",
                IsTheodoraRebelPartyEncounter,
                LeaveRebelPartyEncounter,
                90);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_recruit",
                "mv_theodora_rebel_party_options",
                "mv_theodora_rebel_party_recruit_answer",
                "{=MVTheodoraRebelPartyRecruit}You have long since lost any legitimate claim to remain here. Why not follow me and build something worthy together?",
                IsTheodoraRebelPartyEncounter,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_theodora_rebel_party_recruit_answer",
                "mv_theodora_rebel_party_recruit_answer",
                "mv_theodora_rebel_party_recruit_options",
                "{=MVTheodoraRebelPartyRecruitAnswer}You would have us abandon our claim and follow your banner? Then prove that your offer is more than another promise made on the road.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_recruit_accept",
                "mv_theodora_rebel_party_recruit_options",
                "close_window",
                "{=MVTheodoraRebelPartyRecruitAccept}Join me, and you will have a future larger than this roadside revolt.",
                IsTheodoraRebelPartyEncounter,
                RecruitRebelParty,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_theodora_rebel_party_recruit_cancel",
                "mv_theodora_rebel_party_recruit_options",
                "close_window",
                "{=MVTheodoraRebelPartyRecruitCancel}Then we have nothing more to discuss.",
                IsTheodoraRebelPartyEncounter,
                LeaveRebelPartyEncounter,
                90);
        }

        private static bool IsTalkingToHodophylakes()
        {
            Hero hero = Hero.OneToOneConversationHero;
            return hero != null && hero.Clan != null && hero.Clan.StringId == HodophylakesClanId;
        }

        private static bool IsTalkingToTheodora()
        {
            Hero hero = Hero.OneToOneConversationHero;
            return hero != null && hero.StringId == TheodoraHeroId;
        }

        private static bool CanOfferGenericHelp()
        {
            return IsTalkingToTheodora() &&
                   !CanOfferFoodQuest() &&
                   !IsTheodoraFoodQuestActive() &&
                   !CanOfferForestBanditQuest() &&
                   !IsTheodoraForestBanditQuestActive() &&
                   !CanOfferRebelNobleQuest() &&
                   !IsTheodoraRebelNobleQuestActive();
        }

        private static bool CanOfferFoodQuest()
        {
            Hero theodora = Hero.OneToOneConversationHero;
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;

            return theodora != null &&
                   theodora.StringId == TheodoraHeroId &&
                   progress != null &&
                   !progress.IsTheodoraFoodQuestStarted &&
                   !progress.IsTheodoraFoodQuestCompleted &&
                   !IsAnyTheodoraQuestActive() &&
                   Hero.MainHero.GetRelation(theodora) >= 0;
        }

        private static bool IsTheodoraFoodQuestActive()
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return progress != null &&
                   progress.IsTheodoraFoodQuestStarted &&
                   !progress.IsTheodoraFoodQuestCompleted;
        }

        private static bool CanOfferForestBanditQuest()
        {
            Hero theodora = Hero.OneToOneConversationHero;
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;

            return theodora != null &&
                   theodora.StringId == TheodoraHeroId &&
                   progress != null &&
                   progress.IsTheodoraFoodQuestCompleted &&
                   !progress.IsTheodoraForestBanditQuestStarted &&
                   !progress.IsTheodoraForestBanditQuestCompleted &&
                   !IsAnyTheodoraQuestActive() &&
                   Hero.MainHero.GetRelation(theodora) >= 20 &&
                   TryFindForestBanditHideout(out _);
        }

        private static bool IsTheodoraForestBanditQuestActive()
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return progress != null &&
                   progress.IsTheodoraForestBanditQuestStarted &&
                   !progress.IsTheodoraForestBanditQuestCompleted;
        }

        private static bool IsTheodoraForestBanditQuestActiveWithoutClear()
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return IsTalkingToTheodora() &&
                   IsTheodoraForestBanditQuestActive() &&
                   progress != null &&
                   !progress.IsTheodoraForestBanditHideoutCleared;
        }

        private static bool CanOfferRebelNobleQuest()
        {
            Hero theodora = Hero.OneToOneConversationHero;
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;

            return theodora != null &&
                   theodora.StringId == TheodoraHeroId &&
                   progress != null &&
                   progress.IsTheodoraForestBanditQuestCompleted &&
                   !progress.IsTheodoraRebelQuestStarted &&
                   !progress.IsTheodoraRebelQuestCompleted &&
                   !IsAnyTheodoraQuestActive() &&
                   Hero.MainHero.GetRelation(theodora) >= 40 &&
                   TryFindOrCreateRebelPartyNearLycaron(out _);
        }

        private static bool IsTheodoraRebelNobleQuestActive()
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return progress != null &&
                   progress.IsTheodoraRebelQuestStarted &&
                   !progress.IsTheodoraRebelQuestCompleted;
        }

        private static bool IsAnyTheodoraQuestActive()
        {
            return IsTheodoraFoodQuestActive() ||
                   IsTheodoraForestBanditQuestActive() ||
                   IsTheodoraRebelNobleQuestActive();
        }

        private static bool IsTheodoraRebelNobleQuestActiveWithoutDefeat()
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return IsTalkingToTheodora() &&
                   IsTheodoraRebelNobleQuestActive() &&
                   progress != null &&
                   !progress.IsTheodoraRebelPartyDefeated;
        }

        private static bool IsFoodQuestActiveWithoutSupplies()
        {
            return IsTheodoraFoodQuestActive() && !CanDeliverFoodSupplies();
        }

        private static bool CanDeliverFoodSupplies()
        {
            return IsTalkingToTheodora() &&
                   IsTheodoraFoodQuestActive() &&
                   FindActiveFoodQuest() != null &&
                   GetFoodSupplyCount() >= RequiredFoodUnits;
        }

        private static void StartTheodoraFoodQuest()
        {
            Hero theodora = Hero.OneToOneConversationHero;
            if (theodora != null && CanOfferFoodQuest())
            {
                new TheodoraFoodSupplyQuest(theodora).StartQuest();
            }
        }

        private static void StartTheodoraForestBanditQuest()
        {
            Hero theodora = Hero.OneToOneConversationHero;
            if (theodora == null || !CanOfferForestBanditQuest())
            {
                return;
            }

            if (TryFindForestBanditHideout(out Settlement targetHideout))
            {
                new TheodoraForestBanditQuest(theodora, targetHideout).StartQuest();
            }
        }

        private static void StartTheodoraRebelNobleQuest()
        {
            Hero theodora = Hero.OneToOneConversationHero;
            if (theodora == null || !CanOfferRebelNobleQuest())
            {
                return;
            }

            if (TryFindOrCreateRebelPartyNearLycaron(out MobileParty targetParty))
            {
                new TheodoraRebelNobleQuest(theodora, targetParty).StartQuest();
            }
        }

        private static bool IsTheodoraRebelPartyEncounter()
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            MobileParty encounteredParty = PlayerEncounter.EncounteredMobileParty;

            return Campaign.Current != null &&
                   Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter &&
                   encounteredParty != null &&
                   progress != null &&
                   progress.IsTheodoraRebelQuestStarted &&
                   !progress.IsTheodoraRebelQuestCompleted &&
                   encounteredParty.StringId == progress.TheodoraRebelPartyId;
        }

        private static bool CanPayRebelDisbandFee()
        {
            return IsTheodoraRebelPartyEncounter() && Hero.MainHero.Gold >= RebelDisbandFee;
        }

        private static bool CanNotPayRebelDisbandFee()
        {
            return IsTheodoraRebelPartyEncounter() && Hero.MainHero.Gold < RebelDisbandFee;
        }

        private static void StartRebelPartyBattle()
        {
            if (!IsTheodoraRebelPartyEncounter())
            {
                return;
            }

            PlayerEncounter.StartHostileAction();
            PlayerEncounter.StartBattle();
        }

        private static void PayAndDisbandRebelParty()
        {
            if (!CanPayRebelDisbandFee())
            {
                return;
            }

            TheodoraRebelNobleQuest quest = FindActiveRebelNobleQuest();
            if (quest == null)
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-RebelDisbandFee);
            quest.ResolveByDisbanding();
            PlayerEncounter.LeaveEncounter = true;
        }

        private static void RecruitRebelParty()
        {
            if (!IsTheodoraRebelPartyEncounter())
            {
                return;
            }

            TheodoraRebelNobleQuest quest = FindActiveRebelNobleQuest();
            if (quest == null)
            {
                return;
            }

            quest.ResolveByRecruiting();
            PlayerEncounter.LeaveEncounter = true;
        }

        private static void LeaveRebelPartyEncounter()
        {
            PlayerEncounter.LeaveEncounter = true;
        }

        private static void CompleteTheodoraFoodSupplyQuest()
        {
            TheodoraFoodSupplyQuest quest = FindActiveFoodQuest();
            if (quest == null || !CanDeliverFoodSupplies())
            {
                return;
            }

            RemoveFoodSupplies(RequiredFoodUnits);
            quest.CompleteQuestWithSuccess();
        }

        private static bool CanCompleteTheodoraForestBanditQuest()
        {
            return IsTalkingToTheodora() &&
                   IsTheodoraForestBanditQuestActive() &&
                   HodophylakesProgressBehavior.Instance != null &&
                   HodophylakesProgressBehavior.Instance.IsTheodoraForestBanditHideoutCleared &&
                   FindActiveForestBanditQuest() != null;
        }

        private static void CompleteTheodoraForestBanditQuest()
        {
            TheodoraForestBanditQuest quest = FindActiveForestBanditQuest();
            if (quest != null && CanCompleteTheodoraForestBanditQuest())
            {
                quest.CompleteQuestWithSuccess();
            }
        }

        private static bool CanCompleteTheodoraRebelNobleQuest()
        {
            return IsTalkingToTheodora() &&
                   IsTheodoraRebelNobleQuestActive() &&
                   HodophylakesProgressBehavior.Instance != null &&
                   HodophylakesProgressBehavior.Instance.IsTheodoraRebelPartyDefeated &&
                   FindActiveRebelNobleQuest() != null;
        }

        private static void CompleteTheodoraRebelNobleQuest()
        {
            TheodoraRebelNobleQuest quest = FindActiveRebelNobleQuest();
            if (quest != null && CanCompleteTheodoraRebelNobleQuest())
            {
                quest.CompleteQuestWithSuccess();
            }
        }

        private static TheodoraFoodSupplyQuest FindActiveFoodQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                TheodoraFoodSupplyQuest foodQuest = quest as TheodoraFoodSupplyQuest;
                if (foodQuest != null)
                {
                    return foodQuest;
                }
            }

            return null;
        }

        private static TheodoraForestBanditQuest FindActiveForestBanditQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                TheodoraForestBanditQuest forestBanditQuest = quest as TheodoraForestBanditQuest;
                if (forestBanditQuest != null)
                {
                    return forestBanditQuest;
                }
            }

            return null;
        }

        private static TheodoraRebelNobleQuest FindActiveRebelNobleQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                TheodoraRebelNobleQuest rebelQuest = quest as TheodoraRebelNobleQuest;
                if (rebelQuest != null)
                {
                    return rebelQuest;
                }
            }

            return null;
        }

        private static bool TryFindForestBanditHideout(out Settlement targetHideout)
        {
            targetHideout = null;
            Settlement danustica = null;

            foreach (Settlement settlement in Settlement.All)
            {
                if (settlement.StringId == DanusticaId)
                {
                    danustica = settlement;
                    break;
                }
            }

            if (danustica == null)
            {
                return false;
            }

            float closestDistance = float.MaxValue;
            foreach (Hideout hideout in Hideout.All)
            {
                Settlement settlement = hideout.Settlement;
                if (settlement == null || !settlement.IsActive || !settlement.IsHideout)
                {
                    continue;
                }

                IFaction mapFaction = settlement.MapFaction;
                if (mapFaction == null || mapFaction.Culture == null)
                {
                    continue;
                }

                string cultureId = mapFaction.Culture.StringId;
                if (cultureId != ForestBanditCultureId &&
                    cultureId != "Culture." + ForestBanditCultureId)
                {
                    continue;
                }

                float distance = settlement.GetPosition2D.Distance(danustica.GetPosition2D);
                if (distance > MaxForestBanditHideoutDistanceFromDanustica)
                {
                    continue;
                }

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetHideout = settlement;
                }
            }

            return targetHideout != null;
        }

        private static bool TryFindOrCreateRebelPartyNearLycaron(out MobileParty targetParty)
        {
            targetParty = null;
            Settlement lycaron = null;

            foreach (Settlement settlement in Settlement.All)
            {
                if (settlement.StringId == LycaronId)
                {
                    lycaron = settlement;
                    break;
                }
            }

            if (lycaron == null)
            {
                return false;
            }

            foreach (MobileParty party in MobileParty.All)
            {
                if (party != null && party.IsActive && party.StringId == RebelPartyId)
                {
                    targetParty = party;
                    return true;
                }
            }

            TroopRoster memberRoster = TroopRoster.CreateDummyTroopRoster();
            AddTroopIfAvailable(memberRoster, "imperial_elite_cataphract", 15);
            AddTroopIfAvailable(memberRoster, "imperial_legionary", 30);
            AddTroopIfAvailable(memberRoster, "imperial_veteran_infantryman", 30);
            AddTroopIfAvailable(memberRoster, "imperial_trained_infantryman", 45);

            if (memberRoster.TotalManCount != RebelPartySize)
            {
                return false;
            }

            targetParty = MobileParty.CreateParty(RebelPartyId, null);
            if (targetParty == null)
            {
                return false;
            }

            targetParty.InitializeMobilePartyAroundPosition(
                memberRoster,
                TroopRoster.CreateDummyTroopRoster(),
                lycaron.Position,
                2.0f,
                1.0f,
                false);
            targetParty.Party.SetCustomName(
                new TextObject("{=MVTheodoraRebelPartyName}Lycaron Rebel Nobles"));
            targetParty.SetCustomHomeSettlement(lycaron);
            targetParty.SetPartyUsedByQuest(true);
            targetParty.SetMoveModeHold();
            return true;
        }

        private static void AddTroopIfAvailable(TroopRoster roster, string troopId, int count)
        {
            CharacterObject troop = CharacterObject.Find(troopId);
            if (troop != null)
            {
                roster.AddToCounts(troop, count);
            }
        }

        private static int GetFoodSupplyCount()
        {
            MobileParty party = MobileParty.MainParty;
            if (party == null)
            {
                return 0;
            }

            int foodCount = 0;
            foreach (ItemRosterElement element in party.ItemRoster)
            {
                ItemObject item = element.EquipmentElement.Item;
                if (item != null && item.StringId == RequiredFoodItemId)
                {
                    foodCount += element.Amount;
                }
            }

            return foodCount;
        }

        private static void RemoveFoodSupplies(int amount)
        {
            MobileParty party = MobileParty.MainParty;
            if (party == null)
            {
                return;
            }

            List<ItemObject> foodItems = new List<ItemObject>();
            foreach (ItemRosterElement element in party.ItemRoster)
            {
                ItemObject item = element.EquipmentElement.Item;
                if (item != null && item.StringId == RequiredFoodItemId && !foodItems.Contains(item))
                {
                    foodItems.Add(item);
                }
            }

            int remaining = amount;
            foreach (ItemObject item in foodItems)
            {
                int removable = Math.Min(party.ItemRoster.GetItemNumber(item), remaining);
                if (removable <= 0)
                {
                    continue;
                }

                party.ItemRoster.AddToCounts(item, -removable);
                remaining -= removable;
                if (remaining <= 0)
                {
                    break;
                }
            }
        }
    }
}
