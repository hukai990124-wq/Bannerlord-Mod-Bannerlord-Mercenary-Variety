using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public sealed class SeaRaiderSiegeBehavior : CampaignBehaviorBase
    {
        private const string TavernMenuId = "mv_sea_raider_tavern";
        private const string SeaRaiderClanId = "sea_raiders";
        private const string PravendId = "town_V3";
        private const string TharklifVillageId = "castle_village_N2_2";
        private const string T3SeaRaiderTroopId = "sea_raiders_raider";
        private const string T4SeaRaiderTroopId = "sea_raiders_chief";
        private const string T5SeaRaiderTroopId = "sea_raiders_boss";
        private const string T6SeaRaiderTroopId = "mv_sea_raider_warlord";
        private const int CombinedArmySize = 800;
        private static bool AllowExperimentalLordPartySiege => false;

        private List<string> _activeSiegePartyIds = new List<string>();
        private List<string> _partiesAwaitingDisband = new List<string>();
        private int _siegeSequence;
        private bool _harmonyAvailable;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.AfterSiegeCompletedEvent.AddNonSerializedListener(this, OnAfterSiegeCompleted);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("mv_sea_raider_siege_party_ids", ref _activeSiegePartyIds);
            dataStore.SyncData("mv_sea_raider_siege_disband_ids", ref _partiesAwaitingDisband);
            dataStore.SyncData("mv_sea_raider_siege_sequence", ref _siegeSequence);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                TavernMenuId,
                "mv_sea_raider_siege_pravend",
                "{=MVSeaRaiderSiegePravend}Urge the Sea Raider Host to march on Pravend",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    return true;
                },
                args => LaunchPravendSiege(),
                false,
                -1,
                false,
                null);
        }

        private void LaunchPravendSiege()
        {
            if (!AllowExperimentalLordPartySiege)
            {
                DisplayMessage(new TextObject(
                    "{=MVSeaRaiderSiegeSealed}The Sea Raider Host is not ready to march yet."));
                return;
            }

            if (!_harmonyAvailable)
            {
                _harmonyAvailable = SeaRaiderSiegeHarmony.TryPatch();
            }

            if (!_harmonyAvailable)
            {
                DisplayMessage(new TextObject(
                    "{=MVSeaRaiderSiegeUnavailable}The Sea Raider Host cannot be summoned because the optional Harmony patch is unavailable."));
                return;
            }

            _harmonyAvailable = true;
            Settlement targetSettlement = Settlement.Find(PravendId);
            Settlement homeSettlement = Settlement.Find(TharklifVillageId);
            Clan seaRaiderClan = Clan.FindFirst(clan => clan.StringId == SeaRaiderClanId);
            CharacterObject t3 = CharacterObject.Find(T3SeaRaiderTroopId);
            CharacterObject t4 = CharacterObject.Find(T4SeaRaiderTroopId);
            CharacterObject t5 = CharacterObject.Find(T5SeaRaiderTroopId);
            CharacterObject t6 = CharacterObject.Find(T6SeaRaiderTroopId);

            if (targetSettlement == null || homeSettlement == null || seaRaiderClan == null ||
                t3 == null || t4 == null || t5 == null || t6 == null)
            {
                DisplayMessage(new TextObject(
                    "{=MVSeaRaiderSiegeSpawnFailed}The Sea Raider Host could not be assembled."));
                return;
            }

            CampaignVec2 spawnPosition = GetSpawnPosition(targetSettlement);
            Hero commander = HeroCreator.CreateSpecialHero(t6, homeSettlement, seaRaiderClan, null, 35);
            commander.SetName(
                new TextObject("{=MVSeaRaiderHostCommander}Commander of the Sea Raider Host"),
                new TextObject("{=MVSeaRaiderHostCommander}Commander of the Sea Raider Host"));
            if (!seaRaiderClan.Heroes.Contains(commander))
            {
                seaRaiderClan.Heroes.Add(commander);
            }

            int sequence = ++_siegeSequence;
            MobileParty host = LordPartyComponent.CreateLordParty(
                "mv_sea_raider_siege_pravend_" + sequence,
                commander,
                spawnPosition,
                0f,
                homeSettlement,
                commander);
            host.ActualClan = seaRaiderClan;
            host.Party.SetCustomName(new TextObject(
                "{=MVSeaRaiderHostName}Sea Raider Host"));
            RemoveNonHeroMembers(host);
            host.MemberRoster.AddToCounts(t3, 240, false, 0, 0, true, -1);
            host.MemberRoster.AddToCounts(t4, 260, false, 0, 0, true, -1);
            host.MemberRoster.AddToCounts(t5, 210, false, 0, 0, true, -1);
            host.MemberRoster.AddToCounts(t6, CombinedArmySize - 1 - 240 - 260 - 210, false, 0, 0, true, -1);
            host.ItemRoster.AddToCounts(DefaultItems.Grain, CombinedArmySize * 20);
            host.Party.SetVisualAsDirty();

            if (!seaRaiderClan.IsAtWarWith(targetSettlement.MapFaction))
            {
                DeclareWarAction.ApplyByDefault(seaRaiderClan, targetSettlement.MapFaction);
            }

            _activeSiegePartyIds.Add(host.StringId);
            host.Ai.SetDoNotMakeNewDecisions(true);
            host.SetMoveBesiegeSettlement(targetSettlement, host.NavigationCapability);

            DisplayMessage(new TextObject(
                "{=MVSeaRaiderSiegeStarted}The Sea Raider Host has gathered near Pravend and begun its march on the city."));
            GameMenu.SwitchToMenu("village");
        }

        private void OnHourlyTick()
        {
            foreach (string partyId in _activeSiegePartyIds.ToList())
            {
                MobileParty party = FindParty(partyId);
                if (party == null || !party.IsActive)
                {
                    _activeSiegePartyIds.Remove(partyId);
                    continue;
                }

                SiegeEvent siegeEvent = party.SiegeEvent;
                if (siegeEvent == null || siegeEvent.BesiegerCamp == null ||
                    siegeEvent.BesiegerCamp.LeaderParty != party || party.MapEvent != null ||
                    siegeEvent.BesiegedSettlement.Party.MapEvent != null)
                {
                    continue;
                }

                SiegeEvent.SiegeEngineConstructionProgress preparations =
                    siegeEvent.BesiegerCamp.SiegeEngines.SiegePreparations;
                if (preparations != null && !preparations.IsConstructed)
                {
                    preparations.SetProgress(1f);
                }

                if (preparations != null && preparations.IsConstructed)
                {
                    StartBattleAction.ApplyStartAssaultAgainstWalls(party, siegeEvent.BesiegedSettlement);
                }
            }
        }

        private void OnDailyTick()
        {
            foreach (string partyId in _partiesAwaitingDisband.ToList())
            {
                MobileParty party = FindParty(partyId);
                if (party == null || !party.IsActive)
                {
                    _partiesAwaitingDisband.Remove(partyId);
                    continue;
                }

                if (party.MapEvent == null && party.SiegeEvent == null)
                {
                    Hero commander = party.LeaderHero;
                    DestroyPartyAction.Apply(null, party);
                    RemoveTemporaryCommander(commander);
                    _partiesAwaitingDisband.Remove(partyId);
                }
            }
        }

        private void OnAfterSiegeCompleted(
            Settlement settlement,
            MobileParty attackerParty,
            bool isWin,
            MapEvent.BattleTypes battleType)
        {
            if (attackerParty == null || battleType != MapEvent.BattleTypes.Siege ||
                !_activeSiegePartyIds.Contains(attackerParty.StringId))
            {
                return;
            }

            _activeSiegePartyIds.Remove(attackerParty.StringId);
            if (!_partiesAwaitingDisband.Contains(attackerParty.StringId))
            {
                _partiesAwaitingDisband.Add(attackerParty.StringId);
            }
        }

        internal bool TryConvertVictoryToPillage(
            Settlement settlement,
            MobileParty attackerParty,
            bool isWin,
            MapEvent.BattleTypes battleType)
        {
            if (!isWin || battleType != MapEvent.BattleTypes.Siege || attackerParty == null ||
                !_activeSiegePartyIds.Contains(attackerParty.StringId))
            {
                return false;
            }

            settlement.SiegeEvent?.BesiegerCamp?.RemoveAllSiegeParties();
            _activeSiegePartyIds.Remove(attackerParty.StringId);
            if (!_partiesAwaitingDisband.Contains(attackerParty.StringId))
            {
                _partiesAwaitingDisband.Add(attackerParty.StringId);
            }

            DisplayMessage(new TextObject(
                "{=MVSeaRaiderSiegePillage}The Sea Raider Host has pillaged Pravend and scattered before it can claim the city."));
            return true;
        }

        private static CampaignVec2 GetSpawnPosition(Settlement targetSettlement)
        {
            Vec2 direction = new Vec2(
                targetSettlement.GatePosition.X - targetSettlement.Position.X,
                targetSettlement.GatePosition.Y - targetSettlement.Position.Y);
            if (direction.LengthSquared < 0.01f)
            {
                direction = new Vec2(1f, 0f);
            }

            direction.Normalize();
            CampaignVec2 desiredPosition = targetSettlement.GatePosition + direction * 8f;
            return Campaign.Current.MapSceneWrapper.GetAccessiblePointNearPosition(
                in desiredPosition,
                4f);
        }

        private static void RemoveNonHeroMembers(MobileParty party)
        {
            for (int index = party.MemberRoster.Count - 1; index >= 0; index--)
            {
                TroopRosterElement element = party.MemberRoster.GetElementCopyAtIndex(index);
                if (!element.Character.IsHero)
                {
                    party.MemberRoster.AddToCounts(
                        element.Character,
                        -element.Number,
                        false,
                        0,
                        0,
                        true,
                        -1);
                }
            }
        }

        private static void RemoveTemporaryCommander(Hero commander)
        {
            if (commander == null)
            {
                return;
            }

            Clan clan = commander.Clan;
            DisableHeroAction.Apply(commander);
            clan?.Heroes.Remove(commander);
        }

        private static MobileParty FindParty(string partyId)
        {
            return MobileParty.All.FirstOrDefault(party => party.StringId == partyId);
        }

        private static void DisplayMessage(TextObject message)
        {
            InformationManager.DisplayMessage(
                new InformationMessage(message.ToString(), new Color(0.35f, 0.65f, 1f, 1f)));
        }
    }

    internal static class SeaRaiderSiegeHarmony
    {
        private const string HarmonyId = "MercenaryVariety.SeaRaiderSiege";
        private static bool _isPatched;

        internal static bool TryPatch()
        {
            if (_isPatched)
            {
                return true;
            }

            try
            {
                Type harmonyType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("HarmonyLib.Harmony"))
                    .FirstOrDefault(type => type != null);
                Type harmonyMethodType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("HarmonyLib.HarmonyMethod"))
                    .FirstOrDefault(type => type != null);
                if (harmonyType == null || harmonyMethodType == null)
                {
                    return false;
                }

                MethodInfo targetMethod = typeof(KingdomManager).GetMethod(
                    "SiegeCompleted",
                    BindingFlags.Instance | BindingFlags.Public);
                MethodInfo prefixMethod = typeof(SeaRaiderSiegeHarmony).GetMethod(
                    nameof(PrefixKingdomManagerSiegeCompleted),
                    BindingFlags.Static | BindingFlags.Public);
                MethodInfo patchMethod = harmonyType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(method => method.Name == "Patch" && method.GetParameters().Length == 5);
                if (targetMethod == null || prefixMethod == null || patchMethod == null)
                {
                    return false;
                }

                object harmony = Activator.CreateInstance(harmonyType, HarmonyId);
                object prefix = Activator.CreateInstance(harmonyMethodType, prefixMethod);
                patchMethod.Invoke(harmony, new[] { (object)targetMethod, prefix, null, null, null });
                _isPatched = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool PrefixKingdomManagerSiegeCompleted(
            Settlement settlement,
            MobileParty capturerParty,
            bool isWin,
            MapEvent.BattleTypes battleType)
        {
            SeaRaiderSiegeBehavior behavior = Campaign.Current?.GetCampaignBehavior<SeaRaiderSiegeBehavior>();
            return behavior == null || !behavior.TryConvertVictoryToPillage(
                settlement,
                capturerParty,
                isWin,
                battleType);
        }
    }
}
