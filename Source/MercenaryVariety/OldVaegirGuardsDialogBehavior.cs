using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
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
            return IsTalkingToOldVaegirGuards() &&
                   (!IsTalkingToVasevolod() ||
                    (!CanOfferFoodQuest() &&
                     !IsFoodQuestActive() &&
                     !CanOfferSeaRaiderQuest() &&
                     !IsSeaRaiderQuestActive()));
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
