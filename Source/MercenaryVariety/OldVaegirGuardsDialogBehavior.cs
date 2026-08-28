using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace MercenaryVariety
{
    public class OldVaegirGuardsDialogBehavior : CampaignBehaviorBase
    {
        private const string OldVaegirGuardsClanId = "mv_old_vaegir_guards";
        private const string VasevolodHeroId = "mv_old_vaegir_guards_leader_0";
        private const string RequiredMeatItemId = "meat";
        private const int RequiredMeatUnits = 20;
        private const string DiathmaId = "town_EN2";
        private const string SeaRaiderHideoutId = "hideout_seaside_3";
        private const string SeaRaiderClanId = "sea_raiders";
        private const string SeaRaiderPartyId = "mv_old_vaegir_sea_raider_party";
        private const string SeaRaiderPartyTemplateId = "sea_raiders_template";
        private const float MaxSeaRaiderHideoutDistanceFromDiathma = 30f;
        private const string RecordsPartyId = "mv_old_vaegir_records_party";
        private const int RecordsPartySize = 40;
        private const int RecordsPaymentFee = 50000;
        private const string RecordsPartyName = "{=MVOldVaegirRecordsPartyName}Imperial Paymaster's Deserters";
        private const string AmprelaId = "town_EN6";
        private const string GovernorPartyId = "mv_old_vaegir_governor_party";
        private const int GovernorPartySize = 200;
        private const string GovernorPartyName = "{=MVOldVaegirGovernorPartyName}The Governor's Iron Retinue";

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
                CanShowGenericOfferHelp,
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

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_offer_food_help",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_offer_food_help_answer",
                "{=MVOldVaegirOfferHelp}Is there anything I can help with?",
                CanOfferFoodQuest,
                StartFoodQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_offer_food_help_answer",
                "mv_old_vaegir_offer_food_help_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirOfferFoodHelpAnswer}There is one thing, though I would not ask if we had another choice. We need twenty pieces of meat. Dobromir can smoke it into strips, and that would keep our men fed for a while. Since the Empire dismissed us, we have wandered from contract to contract. Our stores are gone, our purses are thin, and we have no other way to keep the men fed.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_deliver_meat",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_deliver_meat_answer",
                "{=MVOldVaegirDeliverMeat}I have brought the meat you requested.",
                CanDeliverMeat,
                CompleteFoodQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_deliver_meat_answer",
                "mv_old_vaegir_deliver_meat_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirDeliverMeatAnswer}This will become dried meat for the road. You have done more than fill our larder; you have helped a company that the Empire chose to forget.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_meat_not_ready",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_meat_not_ready_answer",
                "{=MVOldVaegirMeatNotReady}I am still gathering the meat you requested.",
                IsFoodQuestActiveWithoutMeat,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_meat_not_ready_answer",
                "mv_old_vaegir_meat_not_ready_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirMeatNotReadyAnswer}Then come back when you have it. We cannot feed a travelling guard on promises.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_offer_sea_raider_help",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_sea_raider_context",
                "{=MVOldVaegirOfferSeaRaiderHelp}You mentioned that some of your old comrades became sea raiders. What can I do?",
                CanOfferSeaRaiderQuest,
                StartSeaRaiderQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_sea_raider_context",
                "mv_old_vaegir_sea_raider_context",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirSeaRaiderContext}Vasevolod has heard that several former guards, unwilling to endure the hardship of life after dismissal, have taken to piracy along the northern waters. Their camp lies near Diathma. Find it and put an end to their raids before their old comradeship becomes an excuse for preying on travelers.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_report_sea_raider",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_report_sea_raider_answer",
                "{=MVOldVaegirReportSeaRaider}The sea raider camp near Diathma is no more.",
                CanCompleteSeaRaiderQuest,
                CompleteSeaRaiderQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_report_sea_raider_answer",
                "mv_old_vaegir_report_sea_raider_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirReportSeaRaiderAnswer}Then their stolen lives end with their stolen trade. I am sorry that we had to meet our old comrades as enemies, but the road cannot be protected by sentiment alone. You have my thanks, and the thanks of every man who still marches beneath this banner.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_sea_raider_not_ready",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_sea_raider_not_ready_answer",
                "{=MVOldVaegirSeaRaiderNotReady}I have not yet cleared the sea raider camp near Diathma.",
                IsSeaRaiderQuestActiveWithoutClearing,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_sea_raider_not_ready_answer",
                "mv_old_vaegir_sea_raider_not_ready_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirSeaRaiderNotReadyAnswer}Then do not delay. Every day they remain there, another traveler may pay for our old comrades' desperation.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_offer_records_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_records_context",
                "{=MVOldVaegirOfferRecordsQuest}You mentioned that the Empire withheld your final pay and military records. Is there something I can do?",
                CanOfferRecordsQuest,
                StartRecordsQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_records_context",
                "mv_old_vaegir_records_context",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirRecordsContext}When the Empire dismissed the Vaegir Guard, our muster rolls and wage ledgers were taken instead of returned. A group of former Imperial deserters now holds them near Diathma. They use our names and the wages owed to us as leverage, demanding payment before they will surrender anything. Recover those records, and we can finally leave the Empire with proof of who we were.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_report_records_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_report_records_quest_answer",
                "{=MVOldVaegirReportRecordsQuest}The Imperial deserters have surrendered the Vaegir Guard's records.",
                CanCompleteRecordsQuest,
                CompleteRecordsQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_report_records_quest_answer",
                "mv_old_vaegir_report_records_quest_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirReportRecordsQuestAnswer}Then the last account of our service is back in our hands. You have not restored our old place in the Empire, but you have returned the one thing no officer should have been able to take from us: the proof that we served.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_records_quest_in_progress",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_records_quest_in_progress_answer",
                "{=MVOldVaegirRecordsQuestInProgress}I am still dealing with the Imperial deserters near Diathma.",
                IsRecordsQuestActiveWithoutDefeat,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_records_quest_in_progress_answer",
                "mv_old_vaegir_records_quest_in_progress_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirRecordsQuestInProgressAnswer}Then do not let them sell our names or spend our wages. Those papers are the last proof that the Vaegir Guard existed as more than a discarded expense in an Imperial ledger.",
                null,
                null,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_records_party_encounter_start",
                "start",
                "mv_old_vaegir_records_party_options",
                "{=MVOldVaegirRecordsPartyIntroduction}These papers bear the seal of the late Emperor Arenikos. The Empire dismissed your old guard, but it never paid what it owed. We took the rolls and the ledgers before they could be destroyed, and now they are worth more to us than the swords of the men who once carried them.",
                IsOldVaegirRecordsPartyEncounter,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_records_party_fight",
                "mv_old_vaegir_records_party_options",
                "mv_old_vaegir_records_party_fight_answer",
                "{=MVOldVaegirRecordsPartyFight}You have no right to keep their names or the wages owed to them. Hand over the records, or answer for this in battle.",
                IsOldVaegirRecordsPartyEncounter,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_records_party_fight_answer",
                "mv_old_vaegir_records_party_fight_answer",
                "close_window",
                "{=MVOldVaegirRecordsPartyFightAnswer}Then come and take them. We will see whether old Imperial names still carry any weight on the road.",
                null,
                StartRecordsPartyBattle,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_records_party_pay",
                "mv_old_vaegir_records_party_options",
                "mv_old_vaegir_records_party_pay_answer",
                "{=MVOldVaegirRecordsPartyPay}I will pay 50,000 denars. Surrender every record and the wages owed to the Vaegir Guard. The payment will be made in the Empire's name.",
                CanPayRecordsFee,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_records_party_pay_answer",
                "mv_old_vaegir_records_party_pay_answer",
                "close_window",
                "{=MVOldVaegirRecordsPartyPayAnswer}Fifty thousand for papers the Empire abandoned? Very well. Take the rolls and the ledgers. We will call it an Imperial payment, if that makes the transaction easier to swallow.",
                null,
                PayAndResolveRecordsParty,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_records_party_negotiate",
                "mv_old_vaegir_records_party_options",
                "mv_old_vaegir_records_party_negotiate_answer",
                "{=MVOldVaegirRecordsPartyNegotiate}Those records are stolen military documents, not your property. Surrender them, and leave before this becomes a crime the Empire can still punish.",
                IsOldVaegirRecordsPartyEncounter,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_records_party_negotiate_answer",
                "mv_old_vaegir_records_party_negotiate_answer",
                "close_window",
                "{=MVOldVaegirRecordsPartyNegotiateAnswer}You speak with more certainty than the officers who abandoned those men. Take the records. We have no wish to die for an Imperial debt that was never ours to collect.",
                null,
                ResolveRecordsPartyByNegotiation,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_offer_governor_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_governor_context",
                "{=MVOldVaegirOfferGovernorQuest}You said an Imperial governor was responsible for our disgrace. Is there something I can do?",
                CanOfferGovernorQuest,
                StartGovernorQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_governor_context",
                "mv_old_vaegir_governor_context",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirGovernorContext}The governor of Lycaron twisted the inquiry after Emperor Arenikos died. He claimed that our guard had conspired with the men who failed to protect the emperor, and his testimony gave the court an excuse to dismiss us. He still rides with a personal retinue near Amprela, dressed in Imperial colors and calling our dismissal a lawful judgment. Find him and end the man who turned our service into a lie.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_report_governor_quest",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_report_governor_quest_answer",
                "{=MVOldVaegirReportGovernorQuest}The governor's cavalry has been defeated.",
                CanCompleteGovernorQuest,
                CompleteGovernorQuest,
                110);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_report_governor_quest_answer",
                "mv_old_vaegir_report_governor_quest_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirReportGovernorQuestAnswer}Then the last voice that condemned us is silent. You have done what our swords could not: you forced the Empire to answer for the lie that destroyed our name. From this day, we will trust your judgment with our own.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_ask_future",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_answer_future",
                "{=MVOldVaegirAskFuture}What do you intend to do now?",
                CanDiscussVaegirFuture,
                null,
                115);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_answer_future",
                "mv_old_vaegir_answer_future",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirAnswerFuture}In return, you have become the most trusted friend the Vaegir among us have ever known. We will accept your employment whenever you call, and many younger men will gladly join your ranks. But as for us old soldiers, we only wish to save enough coin to return to our northern homeland and spend our remaining years there. If you wish, we can still fight for you as mercenaries.",
                null,
                null,
                120);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_governor_quest_in_progress",
                "lord_talk_speak_diplomacy_2",
                "mv_old_vaegir_governor_quest_in_progress_answer",
                "{=MVOldVaegirGovernorQuestInProgress}I have not yet dealt with the governor's retinue near Amprela.",
                IsGovernorQuestActiveWithoutDefeat,
                null,
                100);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_governor_quest_in_progress_answer",
                "mv_old_vaegir_governor_quest_in_progress_answer",
                "lord_talk_speak_diplomacy_2",
                "{=MVOldVaegirGovernorQuestInProgressAnswer}Do not mistake his banners for justice. That man used the emperor's death to turn our service into a crime.",
                null,
                null,
                120);

            campaignGameStarter.AddDialogLine(
                "mv_old_vaegir_governor_party_encounter_start",
                "start",
                "mv_old_vaegir_governor_party_options",
                "{=MVOldVaegirGovernorPartyIntroduction}The governor's retinue blocks the road beneath Imperial colors. Their commander raises his seal and names the dismissal of the Vaegir Guard a lawful judgment. There will be no hearing here, only the consequence of what he did.",
                IsOldVaegirGovernorPartyEncounter,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_old_vaegir_governor_party_fight",
                "mv_old_vaegir_governor_party_options",
                "close_window",
                "{=MVOldVaegirGovernorPartyFight}You condemned honorable soldiers with a lie. Now answer for it in blood.",
                IsOldVaegirGovernorPartyEncounter,
                StartGovernorPartyBattle,
                200);
        }

        private static bool IsTalkingToOldVaegirGuards()
        {
            Hero hero = Hero.OneToOneConversationHero;
            return hero != null && hero.Clan != null && hero.Clan.StringId == OldVaegirGuardsClanId;
        }

        private static bool IsTalkingToVasevolod()
        {
            Hero hero = Hero.OneToOneConversationHero;
            return IsTalkingToOldVaegirGuards() && hero.StringId == VasevolodHeroId;
        }

        private static bool CanShowGenericOfferHelp()
        {
            return IsTalkingToVasevolod() &&
                   !CanOfferFoodQuest() &&
                   !IsFoodQuestActive() &&
                   !CanOfferSeaRaiderQuest() &&
                   !IsSeaRaiderQuestActive() &&
                   !CanOfferRecordsQuest() &&
                   !IsRecordsQuestActive() &&
                   !CanOfferGovernorQuest() &&
                   !IsGovernorQuestActive();
        }

        private static bool CanOfferFoodQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   !progress.IsFoodQuestStarted &&
                   !progress.IsFoodQuestCompleted &&
                   Hero.MainHero.GetRelation(Hero.OneToOneConversationHero) >= 0;
        }

        private static bool IsFoodQuestActive()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsFoodQuestStarted &&
                   !progress.IsFoodQuestCompleted;
        }

        private static bool CanDeliverMeat()
        {
            return IsFoodQuestActive() &&
                   FindActiveFoodQuest() != null &&
                   GetMeatCount() >= RequiredMeatUnits;
        }

        private static bool IsFoodQuestActiveWithoutMeat()
        {
            return IsFoodQuestActive() && !CanDeliverMeat();
        }

        private static bool CanOfferSeaRaiderQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsFoodQuestCompleted &&
                   !progress.IsSeaRaiderQuestStarted &&
                   !progress.IsSeaRaiderQuestCompleted;
        }

        private static bool IsSeaRaiderQuestActive()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsSeaRaiderQuestStarted &&
                   !progress.IsSeaRaiderQuestCompleted;
        }

        private static void StartFoodQuest()
        {
            Hero vasevolod = Hero.OneToOneConversationHero;
            if (vasevolod != null && CanOfferFoodQuest())
            {
                new OldVaegirFoodSupplyQuest(vasevolod).StartQuest();
            }
        }

        private static void CompleteFoodQuest()
        {
            OldVaegirFoodSupplyQuest quest = FindActiveFoodQuest();
            if (quest == null || !CanDeliverMeat())
            {
                return;
            }

            RemoveMeat(RequiredMeatUnits);
            quest.CompleteQuestWithSuccess();
        }

        private static void StartSeaRaiderQuest()
        {
            Hero vasevolod = Hero.OneToOneConversationHero;
            if (vasevolod == null || !CanOfferSeaRaiderQuest())
            {
                return;
            }

            if (TryPrepareSeaRaiderHideout(out Settlement targetHideout))
            {
                new OldVaegirSeaRaiderQuest(vasevolod, targetHideout).StartQuest();
            }
        }

        private static bool CanCompleteSeaRaiderQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsSeaRaiderQuestStarted &&
                   !progress.IsSeaRaiderQuestCompleted &&
                   progress.IsSeaRaiderHideoutCleared &&
                   FindActiveSeaRaiderQuest() != null;
        }

        private static bool IsSeaRaiderQuestActiveWithoutClearing()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsSeaRaiderQuestActive() &&
                   progress != null &&
                   !progress.IsSeaRaiderHideoutCleared;
        }

        private static bool CanOfferRecordsQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsSeaRaiderQuestCompleted &&
                   !progress.IsRecordsQuestStarted &&
                   !progress.IsRecordsQuestCompleted;
        }

        private static bool IsRecordsQuestActive()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsRecordsQuestStarted &&
                   !progress.IsRecordsQuestCompleted;
        }

        private static bool IsRecordsQuestActiveWithoutDefeat()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsRecordsQuestActive() &&
                   progress != null &&
                   !progress.IsRecordsPartyDefeated;
        }

        private static bool CanOfferGovernorQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsRecordsQuestCompleted &&
                   !progress.IsGovernorQuestStarted &&
                   !progress.IsGovernorQuestCompleted;
        }

        private static bool CanDiscussVaegirFuture()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsGovernorQuestCompleted;
        }

        private static bool IsGovernorQuestActive()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsGovernorQuestStarted &&
                   !progress.IsGovernorQuestCompleted;
        }

        private static bool IsGovernorQuestActiveWithoutDefeat()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsGovernorQuestActive() &&
                   progress != null &&
                   !progress.IsGovernorPartyDefeated;
        }

        private static void StartGovernorQuest()
        {
            Hero vasevolod = Hero.OneToOneConversationHero;
            if (vasevolod == null || !CanOfferGovernorQuest())
            {
                return;
            }

            if (TryFindOrCreateGovernorParty(out MobileParty targetParty))
            {
                new OldVaegirGovernorQuest(vasevolod, targetParty).StartQuest();
            }
        }

        private static bool CanCompleteGovernorQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsGovernorQuestStarted &&
                   !progress.IsGovernorQuestCompleted &&
                   progress.IsGovernorPartyDefeated &&
                   FindActiveGovernorQuest() != null;
        }

        private static void CompleteGovernorQuest()
        {
            OldVaegirGovernorQuest quest = FindActiveGovernorQuest();
            if (quest != null && CanCompleteGovernorQuest())
            {
                quest.CompleteQuestWithSuccess();
            }
        }

        private static bool IsOldVaegirGovernorPartyEncounter()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            MobileParty encounteredParty = PlayerEncounter.EncounteredMobileParty;

            return Campaign.Current != null &&
                   Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter &&
                   encounteredParty != null &&
                   progress != null &&
                   progress.IsGovernorQuestStarted &&
                   !progress.IsGovernorQuestCompleted &&
                   encounteredParty.StringId == progress.GovernorPartyId;
        }

        private static void StartGovernorPartyBattle()
        {
            if (!IsOldVaegirGovernorPartyEncounter())
            {
                return;
            }

            PlayerEncounter.StartHostileAction();
            PlayerEncounter.StartBattle();
        }

        private static void StartRecordsQuest()
        {
            Hero vasevolod = Hero.OneToOneConversationHero;
            if (vasevolod == null || !CanOfferRecordsQuest())
            {
                return;
            }

            if (TryFindOrCreateRecordsParty(out MobileParty targetParty))
            {
                new OldVaegirRecordsQuest(vasevolod, targetParty).StartQuest();
            }
        }

        private static bool CanCompleteRecordsQuest()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            return IsTalkingToVasevolod() &&
                   progress != null &&
                   progress.IsRecordsQuestStarted &&
                   !progress.IsRecordsQuestCompleted &&
                   progress.IsRecordsPartyDefeated &&
                   FindActiveRecordsQuest() != null;
        }

        private static void CompleteRecordsQuest()
        {
            OldVaegirRecordsQuest quest = FindActiveRecordsQuest();
            if (quest != null && CanCompleteRecordsQuest())
            {
                quest.CompleteQuestWithSuccess();
            }
        }

        private static bool IsOldVaegirRecordsPartyEncounter()
        {
            OldVaegirGuardsProgressBehavior progress = OldVaegirGuardsProgressBehavior.Instance;
            MobileParty encounteredParty = PlayerEncounter.EncounteredMobileParty;

            return Campaign.Current != null &&
                   Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter &&
                   encounteredParty != null &&
                   progress != null &&
                   progress.IsRecordsQuestStarted &&
                   !progress.IsRecordsQuestCompleted &&
                   encounteredParty.StringId == progress.RecordsPartyId;
        }

        private static bool CanPayRecordsFee()
        {
            return IsOldVaegirRecordsPartyEncounter() &&
                   Hero.MainHero.Gold >= RecordsPaymentFee;
        }

        private static void StartRecordsPartyBattle()
        {
            if (!IsOldVaegirRecordsPartyEncounter())
            {
                return;
            }

            PlayerEncounter.StartHostileAction();
            PlayerEncounter.StartBattle();
        }

        private static void PayAndResolveRecordsParty()
        {
            if (!CanPayRecordsFee())
            {
                return;
            }

            OldVaegirRecordsQuest quest = FindActiveRecordsQuest();
            if (quest == null)
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-RecordsPaymentFee);
            quest.ResolveByPayment();
            PlayerEncounter.LeaveEncounter = true;
        }

        private static void ResolveRecordsPartyByNegotiation()
        {
            if (!IsOldVaegirRecordsPartyEncounter())
            {
                return;
            }

            OldVaegirRecordsQuest quest = FindActiveRecordsQuest();
            if (quest == null)
            {
                return;
            }

            quest.ResolveByNegotiation();
            PlayerEncounter.LeaveEncounter = true;
        }

        private static void CompleteSeaRaiderQuest()
        {
            OldVaegirSeaRaiderQuest quest = FindActiveSeaRaiderQuest();
            if (quest != null && CanCompleteSeaRaiderQuest())
            {
                quest.CompleteQuestWithSuccess();
            }
        }

        private static OldVaegirFoodSupplyQuest FindActiveFoodQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                OldVaegirFoodSupplyQuest foodQuest = quest as OldVaegirFoodSupplyQuest;
                if (foodQuest != null)
                {
                    return foodQuest;
                }
            }

            return null;
        }

        private static OldVaegirSeaRaiderQuest FindActiveSeaRaiderQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                OldVaegirSeaRaiderQuest seaRaiderQuest = quest as OldVaegirSeaRaiderQuest;
                if (seaRaiderQuest != null)
                {
                    return seaRaiderQuest;
                }
            }

            return null;
        }

        private static OldVaegirRecordsQuest FindActiveRecordsQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                OldVaegirRecordsQuest recordsQuest = quest as OldVaegirRecordsQuest;
                if (recordsQuest != null)
                {
                    return recordsQuest;
                }
            }

            return null;
        }

        private static OldVaegirGovernorQuest FindActiveGovernorQuest()
        {
            if (Campaign.Current == null || Campaign.Current.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                OldVaegirGovernorQuest governorQuest = quest as OldVaegirGovernorQuest;
                if (governorQuest != null)
                {
                    return governorQuest;
                }
            }

            return null;
        }

        private static bool TryFindOrCreateRecordsParty(out MobileParty targetParty)
        {
            targetParty = null;
            Settlement diathma = Settlement.Find(DiathmaId);
            if (diathma == null)
            {
                return false;
            }

            foreach (MobileParty party in MobileParty.All)
            {
                if (party != null && party.IsActive && party.StringId == RecordsPartyId)
                {
                    targetParty = party;
                    return true;
                }
            }

            TroopRoster memberRoster = TroopRoster.CreateDummyTroopRoster();
            AddTroopIfAvailable(memberRoster, "imperial_legionary", 10);
            AddTroopIfAvailable(memberRoster, "imperial_veteran_infantryman", 15);
            AddTroopIfAvailable(memberRoster, "imperial_trained_infantryman", 10);
            AddTroopIfAvailable(memberRoster, "imperial_archer", 5);

            if (memberRoster.TotalManCount != RecordsPartySize)
            {
                return false;
            }

            targetParty = MobileParty.CreateParty(RecordsPartyId, null);
            if (targetParty == null)
            {
                return false;
            }

            targetParty.InitializeMobilePartyAroundPosition(
                memberRoster,
                TroopRoster.CreateDummyTroopRoster(),
                diathma.Position,
                2.0f,
                1.0f,
                false);
            targetParty.Party.SetCustomName(new TextObject(RecordsPartyName));
            targetParty.SetCustomHomeSettlement(diathma);
            targetParty.SetPartyUsedByQuest(true);
            targetParty.SetMoveModeHold();
            return true;
        }

        private static bool TryFindOrCreateGovernorParty(out MobileParty targetParty)
        {
            targetParty = null;
            Settlement amprela = Settlement.Find(AmprelaId);
            if (amprela == null)
            {
                return false;
            }

            foreach (MobileParty party in MobileParty.All)
            {
                if (party != null && party.IsActive && party.StringId == GovernorPartyId)
                {
                    targetParty = party;
                    return true;
                }
            }

            TroopRoster memberRoster = TroopRoster.CreateDummyTroopRoster();
            AddTroopIfAvailable(memberRoster, "imperial_elite_cataphract", 100);
            AddTroopIfAvailable(memberRoster, "imperial_cataphract", 100);

            if (memberRoster.TotalManCount != GovernorPartySize)
            {
                return false;
            }

            targetParty = MobileParty.CreateParty(GovernorPartyId, null);
            if (targetParty == null)
            {
                return false;
            }

            targetParty.InitializeMobilePartyAroundPosition(
                memberRoster,
                TroopRoster.CreateDummyTroopRoster(),
                amprela.Position,
                2.0f,
                1.0f,
                false);
            targetParty.Party.SetCustomName(new TextObject(GovernorPartyName));
            targetParty.SetCustomHomeSettlement(amprela);
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

        private static bool TryPrepareSeaRaiderHideout(out Settlement targetHideout)
        {
            targetHideout = Settlement.Find(SeaRaiderHideoutId);
            Settlement diathma = Settlement.Find(DiathmaId);

            if (targetHideout == null ||
                diathma == null ||
                targetHideout.Hideout == null ||
                targetHideout.GetPosition2D.Distance(diathma.GetPosition2D) > MaxSeaRaiderHideoutDistanceFromDiathma)
            {
                targetHideout = null;
                return false;
            }

            targetHideout.IsActive = true;
            targetHideout.IsVisible = true;
            targetHideout.Hideout.IsSpotted = true;
            targetHideout.Hideout.SetNextPossibleAttackTime(CampaignTime.Zero);

            Clan seaRaiderClan = Clan.FindFirst(clan => clan.StringId == SeaRaiderClanId);
            PartyTemplateObject partyTemplate =
                MBObjectManager.Instance.GetObject<PartyTemplateObject>(SeaRaiderPartyTemplateId);

            if (seaRaiderClan == null || partyTemplate == null)
            {
                targetHideout = null;
                return false;
            }

            BanditPartyComponent.CreateBanditParty(
                SeaRaiderPartyId,
                seaRaiderClan,
                targetHideout.Hideout,
                false,
                partyTemplate,
                targetHideout.Position);

            return true;
        }

        private static int GetMeatCount()
        {
            MobileParty party = MobileParty.MainParty;
            if (party == null)
            {
                return 0;
            }

            int meatCount = 0;
            foreach (ItemRosterElement element in party.ItemRoster)
            {
                ItemObject item = element.EquipmentElement.Item;
                if (item != null && item.StringId == RequiredMeatItemId)
                {
                    meatCount += element.Amount;
                }
            }

            return meatCount;
        }

        private static void RemoveMeat(int amount)
        {
            MobileParty party = MobileParty.MainParty;
            if (party == null)
            {
                return;
            }

            List<ItemObject> meatItems = new List<ItemObject>();
            foreach (ItemRosterElement element in party.ItemRoster)
            {
                ItemObject item = element.EquipmentElement.Item;
                if (item != null && item.StringId == RequiredMeatItemId && !meatItems.Contains(item))
                {
                    meatItems.Add(item);
                }
            }

            int remaining = amount;
            foreach (ItemObject item in meatItems)
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
