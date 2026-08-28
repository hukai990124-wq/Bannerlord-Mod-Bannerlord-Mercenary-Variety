using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace MercenaryVariety
{
    public class SeaRaiderGuildMenuBehavior : CampaignBehaviorBase
    {
        private const string TharklifVillageId = "castle_village_N2_2";
        private const string TavernMenuId = "mv_sea_raider_tavern";
        private const string WarriorPromiseMenuId = "mv_sea_raider_warrior_promise";
        private const string GreatRaidOrdersMenuId = "mv_sea_raider_great_raid_orders";
        private const string SeaWolvesOrdersMenuId = "mv_sea_raider_sea_wolves_orders";
        private const string T3SeaRaiderTroopId = "sea_raiders_raider";
        private const string T4SeaRaiderTroopId = "sea_raiders_chief";
        private const string T5SeaRaiderTroopId = "sea_raiders_boss";
        private const string T6SeaRaiderTroopId = "mv_sea_raider_warlord";
        private const string SeaRaiderClanId = "sea_raiders";
        private const string NorthernPiratesClanId = "northern_pirates";
        private const string GreatRaidPartyTemplateId = "mv_sea_raider_great_raiding_army_template";
        private const string SeaWolvesPartyTemplateId = "mv_sea_raider_sea_wolves_test_template";
        private const string PravendId = "town_V3";
        private const string CarBansethId = "town_B3";
        private const string EpicroteaId = "town_EN1";
        private const string VarchegId = "town_S1";
        private const string MakebId = "town_K3";
        private const string HargardId = "town_N4";
        private const string OsticanId = "town_V8";
        private const string OmorId = "town_S3";
        private const string ThronderlagId = "town_N3";
        private const string ArgoronId = "town_EN4";
        private const string OrtysiaId = "town_EW4";
        private const int T3RecruitmentCost = 2000;
        private const int T4RecruitmentCost = 4000;
        private const int GreatRaidingCost = 20000;
        private const int GreatRaidingMinimumTroops = 50;
        private const int GreatRaidingMaximumTroops = 100;
        private const int GreatRaidingMinimumWarlords = 1;
        private const int GreatRaidingMaximumWarlords = 3;
        private const int GreatRaidRequiredSkill = 150;
        private const int GreatRaidPreparationDays = 7;
        private const int GreatRaidLifetimeDays = 15;
        private const int SeaRaiderSafeConductDays = 30;
        private const int SeaRaiderAiExemptionHours = 2;
        private const int GreatRaidPartyCount = 6;
        private const int SeaWolvesPartyCount = 6;
        private const int SeaWolvesSpawnAttemptsPerParty = 10;
        private const float SeaWolvesPatrolRadius = 10f;
        private const float SeaWolvesMinimumSpawnDistance = 0.5f;
        private const float GreatRaidAnchorDistance = 6f;
        private const float GreatRaidAnchorSpreadRadians = 0.25f;
        private const float RecruitmentCooldownDays = 10f;
        private const float GreatRaidingCooldownDays = 30f;
        private static readonly Color SeaRaiderEventColor = new Color(0.35f, 0.65f, 1f, 1f);
        private CampaignTime _nextT3RecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextT4RecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextGreatRaidingTime = CampaignTime.Zero;
        private CampaignTime _seaRaiderSafeConductUntil = CampaignTime.Zero;
        private string _pendingGreatRaidTargetSettlementId;
        private CampaignTime _pendingGreatRaidSpawnTime = CampaignTime.Zero;
        private CampaignTime _activeGreatRaidExpiryTime = CampaignTime.Zero;
        private List<string> _activeGreatRaidPartyIds = new List<string>();
        private string _activeGreatRaidTargetSettlementId;
        private int _greatRaidSequence;
        private string _pendingSeaWolvesTargetSettlementId;
        private CampaignTime _pendingSeaWolvesSpawnTime = CampaignTime.Zero;
        private CampaignTime _activeSeaWolvesExpiryTime = CampaignTime.Zero;
        private List<string> _activeSeaWolvesPartyIds = new List<string>();
        private string _activeSeaWolvesTargetSettlementId;
        private int _seaWolvesSequence;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(
                this,
                OnSessionLaunched);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(
                "mv_sea_raider_next_t3_recruitment_time",
                ref _nextT3RecruitmentTime);
            dataStore.SyncData(
                "mv_sea_raider_next_t4_recruitment_time",
                ref _nextT4RecruitmentTime);
            dataStore.SyncData(
                "mv_sea_raider_next_great_raiding_time",
                ref _nextGreatRaidingTime);
            dataStore.SyncData(
                "mv_sea_raider_safe_conduct_until",
                ref _seaRaiderSafeConductUntil);
            dataStore.SyncData(
                "mv_sea_raider_pending_great_raid_target_settlement_id",
                ref _pendingGreatRaidTargetSettlementId);
            dataStore.SyncData(
                "mv_sea_raider_pending_great_raid_spawn_time",
                ref _pendingGreatRaidSpawnTime);
            dataStore.SyncData(
                "mv_sea_raider_active_great_raid_expiry_time",
                ref _activeGreatRaidExpiryTime);
            dataStore.SyncData(
                "mv_sea_raider_active_great_raid_party_ids",
                ref _activeGreatRaidPartyIds);
            dataStore.SyncData(
                "mv_sea_raider_active_great_raid_target_settlement_id",
                ref _activeGreatRaidTargetSettlementId);
            dataStore.SyncData(
                "mv_sea_raider_great_raid_sequence",
                ref _greatRaidSequence);
            dataStore.SyncData(
                "mv_sea_raider_pending_sea_wolves_target_settlement_id",
                ref _pendingSeaWolvesTargetSettlementId);
            dataStore.SyncData(
                "mv_sea_raider_pending_sea_wolves_spawn_time",
                ref _pendingSeaWolvesSpawnTime);
            dataStore.SyncData(
                "mv_sea_raider_active_sea_wolves_expiry_time",
                ref _activeSeaWolvesExpiryTime);
            dataStore.SyncData(
                "mv_sea_raider_active_sea_wolves_party_ids",
                ref _activeSeaWolvesPartyIds);
            dataStore.SyncData(
                "mv_sea_raider_active_sea_wolves_target_settlement_id",
                ref _activeSeaWolvesTargetSettlementId);
            dataStore.SyncData(
                "mv_sea_raider_sea_wolves_sequence",
                ref _seaWolvesSequence);
            if (_activeGreatRaidPartyIds == null)
            {
                _activeGreatRaidPartyIds = new List<string>();
            }

            if (_activeSeaWolvesPartyIds == null)
            {
                _activeSeaWolvesPartyIds = new List<string>();
            }
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                "village",
                "mv_sea_raider_tavern_entry",
                "{=MVSeaRaiderTavernEntry}Enter the Sea Raider Tavern",
                args =>
                {
                    if (!IsTharklif())
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(TavernMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                TavernMenuId,
                "{=MVSeaRaiderTavernMenu}Tharklif is a remote northern harbor where men who cannot survive the winter on honest work gather crews and seek their fortune to the south. If your reputation is strong enough, you may find warriors here willing to sail under your command.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                TavernMenuId,
                "mv_sea_raider_recruit_t3",
                 "{=MVSeaRaiderTavernRecruitOption}Recruit 20 T3 Sea Raider Warriors (2000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT3();
                    args.Tooltip = new TaleWorlds.Localization.TextObject(
                        "{=MVSeaRaiderT3RecruitmentTooltip}Requirements: Roguery 50, Charm 50, 2000 denars, and an available 10-day recruitment cooldown.");
                    return true;
                },
                args => RecruitT3(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                TavernMenuId,
                "mv_sea_raider_recruit_t4",
                "{=MVSeaRaiderTavernRecruitT4Option}Recruit 20 T4 Sea Raider Chiefs (4000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT4();
                    args.Tooltip = new TaleWorlds.Localization.TextObject(
                        "{=MVSeaRaiderT4RecruitmentTooltip}Requirements: Roguery 75, Charm 75, 4000 denars, and an available 10-day recruitment cooldown.");
                    return true;
                },
                args => RecruitT4(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                TavernMenuId,
                "mv_sea_raider_promise_warriors",
                "{=MVSeaRaiderPromiseWarriors}Promise Glory and Gold to the Warriors",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    args.IsEnabled = CanOpenWarriorPromiseMenu();
                    args.Tooltip = new TextObject(
                        "{=MVSeaRaiderPromiseWarriorsTooltip}Requires Roguery 100 and Charm 100. Greater promises require Roguery 150 and Charm 150.");
                    return true;
                },
                args => GameMenu.SwitchToMenu(WarriorPromiseMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                WarriorPromiseMenuId,
                "{=MVSeaRaiderPromiseWarriorsMenu}The crews gather in a wary silence. You may offer them a place in your own company, call the Sea Wolves to a coastal blockade, or unite the northern raiders for a great raid.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                WarriorPromiseMenuId,
                "mv_sea_raider_great_raiding",
                "{=MVSeaRaiderGreatRaiding}Organize Your Own Raiding Party",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanOrganizeGreatRaiding();
                    args.Tooltip = new TaleWorlds.Localization.TextObject(
                        "{=MVSeaRaiderGreatRaidingTooltip}Requirements: Roguery 100, Charm 100, 20000 denars, and an available 30-day cooldown. Grants 50-100 Sea Raiders, including 1-3 T6 Sea Raider Warlords.");
                    return true;
                },
                args => OrganizeGreatRaiding(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                WarriorPromiseMenuId,
                "mv_sea_raider_open_sea_wolves",
                "{=MVSeaRaiderPromiseSeaWolves}Organize the Sea Wolves",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    args.IsEnabled = CanOpenSeaWolvesOrdersMenu();
                    args.Tooltip = new TextObject(
                        "{=MVSeaRaiderPromiseSeaWolvesTooltip}Requires Roguery 150 and Charm 150. Select a coastal settlement for a fifteen-day naval blockade.");
                    return true;
                },
                args => GameMenu.SwitchToMenu(SeaWolvesOrdersMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                WarriorPromiseMenuId,
                "mv_sea_raider_open_great_raid",
                "{=MVSeaRaiderPromiseGreatRaid}Organize the Great Raiding",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    args.IsEnabled = CanOrderGreatRaid();
                    args.Tooltip = new TextObject(
                        "{=MVSeaRaiderPromiseGreatRaidTooltip}Requires Roguery 150 and Charm 150. Select a settlement for a fifteen-day land raid.");
                    return true;
                },
                args => GameMenu.SwitchToMenu(GreatRaidOrdersMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                WarriorPromiseMenuId,
                "mv_sea_raider_promise_back",
                "{=MVSeaRaiderPromiseBack}Return to the Sea Raider Tavern",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => GameMenu.SwitchToMenu(TavernMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                GreatRaidOrdersMenuId,
                "{=MVSeaRaiderGreatRaidOrdersMenu}Your promise spreads from Tharklif's harbor to the northern coasts. Choose where the crews will gather. The raid will appear in seven days and disperse after fifteen.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            AddGreatRaidTargetOption(
                campaignGameStarter,
                "mv_sea_raider_great_raid_pravend",
                "{=MVSeaRaiderGreatRaidPravend}Send the Great Raiding against Pravend",
                PravendId);
            AddGreatRaidTargetOption(
                campaignGameStarter,
                "mv_sea_raider_great_raid_car_banseth",
                "{=MVSeaRaiderGreatRaidCarBanseth}Send the Great Raiding against Car Banseth",
                CarBansethId);
            AddGreatRaidTargetOption(
                campaignGameStarter,
                "mv_sea_raider_great_raid_epicrotea",
                "{=MVSeaRaiderGreatRaidEpicrotea}Send the Great Raiding against Epicrotea",
                EpicroteaId);
            AddGreatRaidTargetOption(
                campaignGameStarter,
                "mv_sea_raider_great_raid_varcheg",
                "{=MVSeaRaiderGreatRaidVarcheg}Send the Great Raiding against Varcheg",
                VarchegId);
            AddGreatRaidTargetOption(
                campaignGameStarter,
                "mv_sea_raider_great_raid_makeb",
                "{=MVSeaRaiderGreatRaidMakeb}Send the Great Raiding against Makeb",
                MakebId);
            AddGreatRaidTargetOption(
                campaignGameStarter,
                "mv_sea_raider_great_raid_hargard",
                "{=MVSeaRaiderGreatRaidHargard}Send the Great Raiding against Hargard",
                HargardId);

            campaignGameStarter.AddGameMenuOption(
                GreatRaidOrdersMenuId,
                "mv_sea_raider_great_raid_back",
                "{=MVSeaRaiderGreatRaidBack}Return to the Warrior Promise",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => GameMenu.SwitchToMenu(WarriorPromiseMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                SeaWolvesOrdersMenuId,
                "{=MVSeaRaiderSeaWolvesOrdersMenu}Choose the harbor that the Sea Wolves will choke. The fleets will appear in seven days and disperse after fifteen.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_ostican",
                "{=MVSeaRaiderSeaWolvesOstican}Send the Sea Wolves against Ostican",
                OsticanId);
            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_car_banseth",
                "{=MVSeaRaiderSeaWolvesCarBanseth}Send the Sea Wolves against Car Banseth",
                CarBansethId);
            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_omor",
                "{=MVSeaRaiderSeaWolvesOmor}Send the Sea Wolves against Omor",
                OmorId);
            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_thronderlag",
                "{=MVSeaRaiderSeaWolvesThronderlag}Send the Sea Wolves against Thronderlag",
                ThronderlagId);
            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_argoron",
                "{=MVSeaRaiderSeaWolvesArgoron}Send the Sea Wolves against Argoron",
                ArgoronId);
            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_ortysia",
                "{=MVSeaRaiderSeaWolvesOrtysia}Send the Sea Wolves against Ortysia",
                OrtysiaId);
            AddSeaWolvesTargetOption(
                campaignGameStarter,
                "mv_sea_raider_sea_wolves_makeb",
                "{=MVSeaRaiderSeaWolvesMakeb}Send the Sea Wolves against Makeb",
                MakebId);

            campaignGameStarter.AddGameMenuOption(
                SeaWolvesOrdersMenuId,
                "mv_sea_raider_sea_wolves_back",
                "{=MVSeaRaiderSeaWolvesBack}Return to the Warrior Promise",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => GameMenu.SwitchToMenu(WarriorPromiseMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                TavernMenuId,
                "mv_sea_raider_tavern_leave",
                "{=MVSeaRaiderTavernLeave}Leave",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => GameMenu.SwitchToMenu("village"),
                true,
                -1,
                false,
                null);
        }

        private static bool IsTharklif()
        {
            Settlement settlement = Settlement.CurrentSettlement;
            return settlement != null && settlement.StringId == TharklifVillageId;
        }

        private bool CanRecruitT3()
        {
            return Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) >= 50 &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Charm) >= 50 &&
                   CanRecruit(
                T3SeaRaiderTroopId,
                20,
                T3RecruitmentCost,
                _nextT3RecruitmentTime);
        }

        private bool CanRecruitT4()
        {
            return Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) >= 75 &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Charm) >= 75 &&
                   CanRecruit(
                       T4SeaRaiderTroopId,
                       20,
                       T4RecruitmentCost,
                       _nextT4RecruitmentTime);
        }

        private void RecruitT3()
        {
            if (!Recruit(
                T3SeaRaiderTroopId,
                20,
                T3RecruitmentCost,
                ref _nextT3RecruitmentTime))
            {
                return;
            }

            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderT3RecruitmentSuccess}Some young men join you on a trial basis, hoping to try their luck under your banner."));
        }

        private void RecruitT4()
        {
            if (!Recruit(
                T4SeaRaiderTroopId,
                20,
                T4RecruitmentCost,
                ref _nextT4RecruitmentTime))
            {
                return;
            }

            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderT4RecruitmentSuccess}A band of veterans judge you a worthy leader and decide to let you chart their course."));
        }

        private bool CanOrganizeGreatRaiding()
        {
            MobileParty party = MobileParty.MainParty;
            return party != null &&
                   Hero.MainHero != null &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) >= 100 &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Charm) >= 100 &&
                   _nextGreatRaidingTime.IsPast &&
                   Hero.MainHero.Gold >= GreatRaidingCost &&
                   CharacterObject.Find(T3SeaRaiderTroopId) != null &&
                   CharacterObject.Find(T4SeaRaiderTroopId) != null &&
                   CharacterObject.Find(T5SeaRaiderTroopId) != null &&
                   CharacterObject.Find(T6SeaRaiderTroopId) != null;
        }

        private void OrganizeGreatRaiding()
        {
            if (!CanOrganizeGreatRaiding())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-GreatRaidingCost);
            _nextGreatRaidingTime = CampaignTime.DaysFromNow(GreatRaidingCooldownDays);

            int troopCount = MBRandom.RandomInt(
                GreatRaidingMinimumTroops,
                GreatRaidingMaximumTroops + 1);
            int warlordCount = MBRandom.RandomInt(
                GreatRaidingMinimumWarlords,
                GreatRaidingMaximumWarlords + 1);
            for (int i = 0; i < warlordCount; i++)
            {
                MobileParty.MainParty.MemberRoster.AddToCounts(
                    CharacterObject.Find(T6SeaRaiderTroopId),
                    1,
                    false,
                    0,
                    0,
                    false,
                    0);
            }

            for (int i = warlordCount; i < troopCount; i++)
            {
                string troopId = MBRandom.RandomInt(0, 3) switch
                {
                    0 => T3SeaRaiderTroopId,
                    1 => T4SeaRaiderTroopId,
                    _ => T5SeaRaiderTroopId
                };
                MobileParty.MainParty.MemberRoster.AddToCounts(
                    CharacterObject.Find(troopId),
                    1,
                    false,
                    0,
                    0,
                    false,
                    0);
            }

            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderGreatRaidingSuccess}The sea raiders raise their drinking horns and shout your name. They had heard of your reputation; now they follow you as their leader. ({COUNT} joined your company.)")
                    .SetTextVariable("COUNT", troopCount));
            GameMenu.SwitchToMenu("village");
        }

        private void AddGreatRaidTargetOption(
            CampaignGameStarter campaignGameStarter,
            string optionId,
            string optionText,
            string settlementId)
        {
            campaignGameStarter.AddGameMenuOption(
                GreatRaidOrdersMenuId,
                optionId,
                optionText,
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanOrderGreatRaid();
                    args.Tooltip = new TextObject(
                        "{=MVSeaRaiderGreatRaidTargetTooltip}The crews need seven days to gather. Six Sea Raider armies will operate for fifteen days.");
                    return true;
                },
                args => ScheduleGreatRaid(settlementId),
                false,
                -1,
                false,
                null);
        }

        private bool CanOrderGreatRaid()
        {
            return Hero.MainHero != null &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) >= GreatRaidRequiredSkill &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Charm) >= GreatRaidRequiredSkill &&
                   !HasPendingGreatRaid() &&
                   !HasActiveGreatRaid();
        }

        private bool HasPendingGreatRaid()
        {
            return !string.IsNullOrEmpty(_pendingGreatRaidTargetSettlementId);
        }

        private bool HasActiveGreatRaid()
        {
            return _activeGreatRaidPartyIds.Count > 0;
        }

        private void ScheduleGreatRaid(string targetSettlementId)
        {
            if (!CanOrderGreatRaid() || Settlement.Find(targetSettlementId) == null)
            {
                return;
            }

            _pendingGreatRaidTargetSettlementId = targetSettlementId;
            _pendingGreatRaidSpawnTime = CampaignTime.DaysFromNow(GreatRaidPreparationDays);
            GrantSeaRaiderSafeConduct();
            PlayGreatRaidHorn();
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderGreatRaidOrdered}Rumor has it that a great many people have gathered in a remote corner of Nord. Answering a leader's call, they are rowing south."));
            GameMenu.SwitchToMenu("village");
        }

        private void AddSeaWolvesTargetOption(
            CampaignGameStarter campaignGameStarter,
            string optionId,
            string optionText,
            string settlementId)
        {
            campaignGameStarter.AddGameMenuOption(
                SeaWolvesOrdersMenuId,
                optionId,
                optionText,
                args =>
                {
                    Settlement targetSettlement = Settlement.Find(settlementId);
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanOrderSeaWolves(settlementId);
                    args.Tooltip = new TextObject(
                        "{=MVSeaRaiderSeaWolvesTooltip}Requirements: Roguery 150, Charm 150, and no pending or active Sea Wolves. After seven days, six fleets of 110 Sea Raiders will appear in the waters outside {TOWN_NAME}, each with two Drakkars, and operate for fifteen days.")
                        .SetTextVariable(
                            "TOWN_NAME",
                            targetSettlement != null
                                ? targetSettlement.Name
                                : new TextObject("{=MVSeaRaiderSeaWolvesTargetWaters}the target waters"));
                    return true;
                },
                args => ScheduleSeaWolves(settlementId),
                false,
                -1,
                false,
                null);
        }

        private bool CanOrderSeaWolves(string targetSettlementId)
        {
            Settlement targetSettlement = Settlement.Find(targetSettlementId);
            Clan northernPiratesClan =
                Clan.FindFirst(clan => clan.StringId == NorthernPiratesClanId);
            PiratesCampaignBehavior piratesBehavior =
                Campaign.Current.GetCampaignBehavior<PiratesCampaignBehavior>();

            return Hero.MainHero != null &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) >= GreatRaidRequiredSkill &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Charm) >= GreatRaidRequiredSkill &&
                   targetSettlement != null &&
                   northernPiratesClan != null &&
                   MBObjectManager.Instance.GetObject<PartyTemplateObject>(SeaWolvesPartyTemplateId) != null &&
                   !HasPendingSeaWolves() &&
                   !HasActiveSeaWolves() &&
                   piratesBehavior != null &&
                   CreateSeaWolvesPatrolZone(targetSettlement) != null;
        }

        private bool CanOpenWarriorPromiseMenu()
        {
            return Hero.MainHero != null &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Roguery) >= 100 &&
                   Hero.MainHero.GetSkillValue(DefaultSkills.Charm) >= 100;
        }

        private bool CanOpenSeaWolvesOrdersMenu()
        {
            return CanOrderSeaWolves(OsticanId) ||
                   CanOrderSeaWolves(CarBansethId) ||
                   CanOrderSeaWolves(OmorId) ||
                   CanOrderSeaWolves(ThronderlagId) ||
                   CanOrderSeaWolves(ArgoronId) ||
                   CanOrderSeaWolves(OrtysiaId) ||
                   CanOrderSeaWolves(MakebId);
        }

        private bool HasPendingSeaWolves()
        {
            return !string.IsNullOrEmpty(_pendingSeaWolvesTargetSettlementId);
        }

        private bool HasActiveSeaWolves()
        {
            return _activeSeaWolvesPartyIds.Count > 0;
        }

        private void ScheduleSeaWolves(string targetSettlementId)
        {
            Settlement targetSettlement = Settlement.Find(targetSettlementId);
            if (!CanOrderSeaWolves(targetSettlementId) || targetSettlement == null)
            {
                return;
            }

            _pendingSeaWolvesTargetSettlementId = targetSettlementId;
            _pendingSeaWolvesSpawnTime = CampaignTime.DaysFromNow(GreatRaidPreparationDays);
            GrantSeaRaiderSafeConduct();
            PlayGreatRaidHorn();
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderSeaWolvesOrdered}Word spreads from Tharklif: the Sea Wolves are gathering to sail against {TOWN_NAME}.")
                    .SetTextVariable("TOWN_NAME", targetSettlement.Name));
            GameMenu.SwitchToMenu("village");
        }

        private void OnDailyTick()
        {
            ProcessPendingRaids();
            UpdateActiveGreatRaid();
            UpdateActiveSeaWolves();
        }

        private void OnHourlyTick()
        {
            if (!_seaRaiderSafeConductUntil.IsPast)
            {
                ApplySeaRaiderSafeConduct();
            }

            PreventActiveGreatRaidHideoutReturns();
            RefreshActiveSeaWolvesPatrols();
        }

        private void ProcessPendingRaids()
        {
            if (HasPendingGreatRaid() && _pendingGreatRaidSpawnTime.IsPast)
            {
                SpawnPendingGreatRaid();
            }

            if (HasPendingSeaWolves() && _pendingSeaWolvesSpawnTime.IsPast)
            {
                SpawnPendingSeaWolves();
            }
        }

        private void GrantSeaRaiderSafeConduct()
        {
            _seaRaiderSafeConductUntil = CampaignTime.DaysFromNow(SeaRaiderSafeConductDays);
            ApplySeaRaiderSafeConduct();
        }

        private static void ApplySeaRaiderSafeConduct()
        {
            foreach (MobileParty party in MobileParty.All)
            {
                if (party == null || !party.IsActive || party.Ai == null || party.ActualClan == null)
                {
                    continue;
                }

                string clanId = party.ActualClan.StringId;
                if (clanId == SeaRaiderClanId || clanId == NorthernPiratesClanId)
                {
                    party.Ai.SetDoNotAttackMainParty(SeaRaiderAiExemptionHours);
                }
            }
        }

        private void SpawnPendingGreatRaid()
        {
            Settlement targetSettlement = Settlement.Find(_pendingGreatRaidTargetSettlementId);
            Clan seaRaiderClan = Clan.FindFirst(clan => clan.StringId == SeaRaiderClanId);
            PartyTemplateObject partyTemplate =
                MBObjectManager.Instance.GetObject<PartyTemplateObject>(GreatRaidPartyTemplateId);

            _pendingGreatRaidTargetSettlementId = null;
            _pendingGreatRaidSpawnTime = CampaignTime.Zero;

            if (targetSettlement == null || seaRaiderClan == null || partyTemplate == null)
            {
                DisplaySeaRaiderMessage(
                    new TextObject(
                        "{=MVSeaRaiderGreatRaidSpawnFailed}The promised raiders could not gather near {TOWN_NAME}.")
                    .SetTextVariable("TOWN_NAME", GetGreatRaidTargetName(targetSettlement)));
                return;
            }

            List<MobileParty> createdParties = new List<MobileParty>();
            int raidSequence = ++_greatRaidSequence;
            for (int index = 0; index < GreatRaidPartyCount; index++)
            {
                MobileParty party = BanditPartyComponent.CreateLooterParty(
                    "mv_sea_raider_great_raid_" + raidSequence + "_" + (index + 1),
                    seaRaiderClan,
                    targetSettlement,
                    false,
                    partyTemplate,
                    GetGreatRaidSpawnPosition(targetSettlement, index));

                if (party != null)
                {
                    party.Party.SetCustomName(GetGreatRaidClanName(index));
                    createdParties.Add(party);
                }
            }

            if (createdParties.Count != GreatRaidPartyCount)
            {
                foreach (MobileParty party in createdParties)
                {
                    DestroyPartyAction.Apply(null, party);
                }

                DisplaySeaRaiderMessage(
                    new TextObject(
                        "{=MVSeaRaiderGreatRaidSpawnFailed}The promised raiders could not gather near {TOWN_NAME}.")
                    .SetTextVariable("TOWN_NAME", targetSettlement.Name));
                return;
            }

            _activeGreatRaidPartyIds.Clear();
            foreach (MobileParty party in createdParties)
            {
                _activeGreatRaidPartyIds.Add(party.StringId);
            }

            _activeGreatRaidTargetSettlementId = targetSettlement.StringId;
            _activeGreatRaidExpiryTime = CampaignTime.DaysFromNow(GreatRaidLifetimeDays);
            PlayGreatRaidHorn();
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderGreatRaidSpawned}Rumor has it that a leader of extraordinary charm has persuaded northern warriors to unite. Guided by that leader, they have slipped past the coastal defenses and now surround {TOWN_NAME}!")
                .SetTextVariable("TOWN_NAME", targetSettlement.Name));
        }

        private void SpawnPendingSeaWolves()
        {
            Settlement targetSettlement = Settlement.Find(_pendingSeaWolvesTargetSettlementId);
            Clan northernPiratesClan =
                Clan.FindFirst(clan => clan.StringId == NorthernPiratesClanId);
            PartyTemplateObject partyTemplate =
                MBObjectManager.Instance.GetObject<PartyTemplateObject>(SeaWolvesPartyTemplateId);
            PiratesCampaignBehavior piratesBehavior =
                Campaign.Current.GetCampaignBehavior<PiratesCampaignBehavior>();

            _pendingSeaWolvesTargetSettlementId = null;
            _pendingSeaWolvesSpawnTime = CampaignTime.Zero;

            CampaignVec2 patrolCenter;
            object patrolZone = CreateSeaWolvesPatrolZone(targetSettlement);
            if (targetSettlement == null || northernPiratesClan == null || partyTemplate == null || piratesBehavior == null || patrolZone == null || !TryGetSeaWolvesPatrolCenter(targetSettlement, out patrolCenter))
            {
                DisplaySeaRaiderMessage(
                    new TextObject(
                        "{=MVSeaRaiderSeaWolvesSpawnFailed}The Sea Wolves could not find a navigable route into {TOWN_NAME}'s waters.")
                        .SetTextVariable("TOWN_NAME", GetSeaWolvesTargetName(targetSettlement)));
                return;
            }

            List<MobileParty> createdParties = new List<MobileParty>();
            int seaWolvesSequence = ++_seaWolvesSequence;
            for (int index = 0; index < SeaWolvesPartyCount; index++)
            {
                MobileParty party = TryCreateSeaWolvesParty(
                    piratesBehavior,
                    patrolZone,
                    patrolCenter,
                    northernPiratesClan,
                    targetSettlement,
                    partyTemplate,
                    seaWolvesSequence,
                    index,
                    createdParties);
                if (party != null)
                {
                    party.Party.SetCustomName(GetSeaWolvesClanName(index));
                    createdParties.Add(party);
                }
            }

            if (createdParties.Count != SeaWolvesPartyCount)
            {
                foreach (MobileParty party in createdParties)
                {
                    DestroyPartyAction.Apply(null, party);
                }

                DisplaySeaRaiderMessage(
                    new TextObject(
                        "{=MVSeaRaiderSeaWolvesSpawnFailed}The Sea Wolves could not find a navigable route into {TOWN_NAME}'s waters.")
                        .SetTextVariable("TOWN_NAME", targetSettlement.Name));
                return;
            }

            _activeSeaWolvesPartyIds.Clear();
            foreach (MobileParty party in createdParties)
            {
                _activeSeaWolvesPartyIds.Add(party.StringId);
            }

            _activeSeaWolvesTargetSettlementId = targetSettlement.StringId;
            _activeSeaWolvesExpiryTime = CampaignTime.DaysFromNow(GreatRaidLifetimeDays);
            PlayGreatRaidHorn();
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderSeaWolvesSpawned}Six Sea Wolf fleets have appeared in the waters outside {TOWN_NAME}. Each fleet carries 110 Sea Raiders in two Drakkars.")
                .SetTextVariable("TOWN_NAME", targetSettlement.Name));
        }

        private static CampaignVec2 GetGreatRaidSpawnPosition(Settlement targetSettlement, int index)
        {
            Vec2 outwardDirection = new Vec2(
                targetSettlement.GatePosition.X - targetSettlement.Position.X,
                targetSettlement.GatePosition.Y - targetSettlement.Position.Y);
            if (outwardDirection.LengthSquared < 0.01f)
            {
                outwardDirection = new Vec2(1f, 0f);
            }

            outwardDirection.Normalize();
            outwardDirection.RotateCCW((index - 1) * GreatRaidAnchorSpreadRadians);
            CampaignVec2 desiredPosition = targetSettlement.GatePosition +
                outwardDirection * GreatRaidAnchorDistance;
            return Campaign.Current.MapSceneWrapper.GetAccessiblePointNearPosition(
                in desiredPosition,
                GreatRaidAnchorDistance * 0.5f);
        }

        private static TextObject GetGreatRaidClanName(int index)
        {
            return new TextObject(index switch
            {
                0 => "{=MVSeaRaiderClanGreyWolf}Grey Wolf Clan",
                1 => "{=MVSeaRaiderClanBlackOar}Black Oar Clan",
                2 => "{=MVSeaRaiderClanIronRaven}Iron Raven Clan",
                3 => "{=MVSeaRaiderClanStormAxe}Storm Axe Clan",
                4 => "{=MVSeaRaiderClanWinterBear}Winter Bear Clan",
                _ => "{=MVSeaRaiderClanBrokenShield}Broken Shield Clan"
            });
        }

        private static TextObject GetSeaWolvesClanName(int index)
        {
            return new TextObject(index switch
            {
                0 => "{=MVSeaRaiderClanWhiteWake}White Wake Clan",
                1 => "{=MVSeaRaiderClanSaltFang}Salt Fang Clan",
                2 => "{=MVSeaRaiderClanSeaSerpent}Sea Serpent Clan",
                3 => "{=MVSeaRaiderClanColdTide}Cold Tide Clan",
                4 => "{=MVSeaRaiderClanWhaleRoad}Whale Road Clan",
                _ => "{=MVSeaRaiderClanBlackSail}Black Sail Clan"
            });
        }

        private static TextObject GetSeaWolvesTargetName(Settlement targetSettlement)
        {
            return targetSettlement != null
                ? targetSettlement.Name
                : new TextObject("{=MVSeaRaiderSeaWolvesTargetWaters}the target waters");
        }

        private static TextObject GetGreatRaidTargetName(Settlement targetSettlement)
        {
            return targetSettlement != null
                ? targetSettlement.Name
                : new TextObject("{=MVSeaRaiderGreatRaidTargetArea}the target area");
        }

        private TextObject GetActiveSeaWolvesTargetName()
        {
            return GetSeaWolvesTargetName(
                Settlement.Find(_activeSeaWolvesTargetSettlementId));
        }

        private static object CreateSeaWolvesPatrolZone(Settlement targetSettlement)
        {
            CampaignVec2 patrolCenter;
            if (!TryGetSeaWolvesPatrolCenter(targetSettlement, out patrolCenter))
            {
                return null;
            }

            Type patrolZoneType = typeof(PiratesCampaignBehavior).GetNestedType(
                "PatrolZone",
                BindingFlags.NonPublic);
            if (patrolZoneType == null)
            {
                return null;
            }

            ConstructorInfo constructor = patrolZoneType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(CampaignVec2), typeof(float) },
                null);
            return constructor == null
                ? null
                : constructor.Invoke(new object[]
                {
                    patrolCenter,
                    SeaWolvesPatrolRadius
                });
        }

        private static bool TryGetSeaWolvesPatrolCenter(
            Settlement targetSettlement,
            out CampaignVec2 patrolCenter)
        {
            if (targetSettlement != null && targetSettlement.HasPort)
            {
                patrolCenter = targetSettlement.PortPosition;
                return true;
            }

            patrolCenter = default(CampaignVec2);
            return false;
        }

        private static CampaignVec2 GetPirateSpawnPosition(
            PiratesCampaignBehavior piratesBehavior,
            object patrolZone,
            CampaignVec2 fallbackPosition)
        {
            MethodInfo getSpawnPositionMethod = typeof(PiratesCampaignBehavior).GetMethod(
                "GetSpawnPosition",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (getSpawnPositionMethod == null)
            {
                return fallbackPosition;
            }

            try
            {
                return (CampaignVec2)getSpawnPositionMethod.Invoke(
                    piratesBehavior,
                    new[] { patrolZone });
            }
            catch (TargetInvocationException)
            {
                return fallbackPosition;
            }
        }

        private static MobileParty TryCreateSeaWolvesParty(
            PiratesCampaignBehavior piratesBehavior,
            object patrolZone,
            CampaignVec2 patrolCenter,
            Clan northernPiratesClan,
            Settlement targetSettlement,
            PartyTemplateObject partyTemplate,
            int seaWolvesSequence,
            int partyIndex,
            List<MobileParty> createdParties)
        {
            for (int attempt = 0; attempt < SeaWolvesSpawnAttemptsPerParty; attempt++)
            {
                CampaignVec2 spawnPosition = GetPirateSpawnPosition(
                    piratesBehavior,
                    patrolZone,
                    patrolCenter);
                if (!IsSeaWolvesSpawnPositionAvailable(spawnPosition, createdParties))
                {
                    continue;
                }

                MobileParty party = BanditPartyComponent.CreateLooterParty(
                    "mv_sea_raider_sea_wolves_" + seaWolvesSequence + "_" +
                    (partyIndex + 1) + "_" + (attempt + 1),
                    northernPiratesClan,
                    targetSettlement,
                    false,
                    partyTemplate,
                    spawnPosition);
                if (party == null)
                {
                    continue;
                }

                if (!InitializeNativePirateParty(piratesBehavior, party, northernPiratesClan))
                {
                    DestroyPartyAction.Apply(null, party);
                    continue;
                }

                party.SetMovePatrolAroundPoint(
                    patrolCenter,
                    MobileParty.NavigationType.Naval);
                return party;
            }

            return null;
        }

        private static bool IsSeaWolvesSpawnPositionAvailable(
            CampaignVec2 candidatePosition,
            List<MobileParty> createdParties)
        {
            float minimumDistanceSquared = SeaWolvesMinimumSpawnDistance *
                SeaWolvesMinimumSpawnDistance;
            foreach (MobileParty party in createdParties)
            {
                if (party == null || !party.IsActive)
                {
                    continue;
                }

                float offsetX = candidatePosition.X - party.Position.X;
                float offsetY = candidatePosition.Y - party.Position.Y;
                if (offsetX * offsetX + offsetY * offsetY < minimumDistanceSquared)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool InitializeNativePirateParty(
            PiratesCampaignBehavior piratesBehavior,
            MobileParty party,
            Clan seaRaiderClan)
        {
            MethodInfo initializePartyMethod = typeof(PiratesCampaignBehavior).GetMethod(
                "InitializePirateParty",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (piratesBehavior == null || initializePartyMethod == null)
            {
                return false;
            }

            try
            {
                initializePartyMethod.Invoke(
                    piratesBehavior,
                    new object[] { party, seaRaiderClan });
                return true;
            }
            catch (TargetInvocationException)
            {
                return false;
            }
        }

        private void PreventActiveGreatRaidHideoutReturns()
        {
            if (_activeGreatRaidPartyIds.Count == 0)
            {
                return;
            }

            Settlement targetSettlement = GetActiveGreatRaidTargetSettlement();
            if (targetSettlement == null)
            {
                return;
            }

            foreach (string partyId in _activeGreatRaidPartyIds)
            {
                MobileParty party = FindMobileParty(partyId);
                if (party == null || !party.IsActive || party.MapEvent != null)
                {
                    continue;
                }

                Settlement aiTargetSettlement = party.TargetSettlement;
                if (aiTargetSettlement == null || !aiTargetSettlement.IsHideout)
                {
                    continue;
                }

                party.SetMovePatrolAroundPoint(
                    targetSettlement.GatePosition,
                    MobileParty.NavigationType.Default);
            }
        }

        private Settlement GetActiveGreatRaidTargetSettlement()
        {
            Settlement targetSettlement = string.IsNullOrEmpty(
                _activeGreatRaidTargetSettlementId)
                ? null
                : Settlement.Find(_activeGreatRaidTargetSettlementId);
            if (targetSettlement != null)
            {
                return targetSettlement;
            }

            foreach (string partyId in _activeGreatRaidPartyIds)
            {
                MobileParty party = FindMobileParty(partyId);
                if (party == null || party.HomeSettlement == null)
                {
                    continue;
                }

                _activeGreatRaidTargetSettlementId = party.HomeSettlement.StringId;
                return party.HomeSettlement;
            }

            return null;
        }

        private void RefreshActiveSeaWolvesPatrols()
        {
            if (_activeSeaWolvesPartyIds.Count == 0 ||
                !TryGetSeaWolvesPatrolCenter(
                    Settlement.Find(_activeSeaWolvesTargetSettlementId),
                    out CampaignVec2 patrolCenter))
            {
                return;
            }

            foreach (string partyId in _activeSeaWolvesPartyIds)
            {
                MobileParty party = FindMobileParty(partyId);
                if (party == null || !party.IsActive || party.MapEvent != null)
                {
                    continue;
                }

                party.SetMovePatrolAroundPoint(
                    patrolCenter,
                    MobileParty.NavigationType.Naval);
            }
        }

        private void UpdateActiveGreatRaid()
        {
            bool hadActiveGreatRaidParties = _activeGreatRaidPartyIds.Count > 0;
            RemoveMissingGreatRaidPartyIds();
            if (_activeGreatRaidPartyIds.Count == 0)
            {
                if (hadActiveGreatRaidParties)
                {
                    if (_activeGreatRaidExpiryTime.IsPast)
                    {
                        FinishGreatRaidDisbanding();
                    }
                    else
                    {
                        FinishGreatRaidDefeat();
                    }
                }
                else
                {
                    _activeGreatRaidExpiryTime = CampaignTime.Zero;
                    _activeGreatRaidTargetSettlementId = null;
                }

                return;
            }

            if (!_activeGreatRaidExpiryTime.IsPast)
            {
                return;
            }

            List<string> disbandedPartyIds = new List<string>();
            foreach (string partyId in _activeGreatRaidPartyIds)
            {
                MobileParty party = FindMobileParty(partyId);
                if (party == null || !party.IsActive)
                {
                    disbandedPartyIds.Add(partyId);
                    continue;
                }

                if (party.MapEvent == null)
                {
                    DestroyPartyAction.Apply(null, party);
                    disbandedPartyIds.Add(partyId);
                }
            }

            foreach (string partyId in disbandedPartyIds)
            {
                _activeGreatRaidPartyIds.Remove(partyId);
            }

            if (_activeGreatRaidPartyIds.Count == 0)
            {
                FinishGreatRaidDisbanding();
            }
        }

        private void UpdateActiveSeaWolves()
        {
            bool hadActiveSeaWolves = _activeSeaWolvesPartyIds.Count > 0;
            RemoveMissingSeaWolvesPartyIds();
            if (_activeSeaWolvesPartyIds.Count == 0)
            {
                if (hadActiveSeaWolves)
                {
                    if (_activeSeaWolvesExpiryTime.IsPast)
                    {
                        FinishSeaWolvesDisbanding();
                    }
                    else
                    {
                        FinishSeaWolvesDefeat();
                    }
                }
                else
                {
                    _activeSeaWolvesExpiryTime = CampaignTime.Zero;
                    _activeSeaWolvesTargetSettlementId = null;
                }

                return;
            }

            if (!_activeSeaWolvesExpiryTime.IsPast)
            {
                return;
            }

            List<string> disbandedPartyIds = new List<string>();
            foreach (string partyId in _activeSeaWolvesPartyIds)
            {
                MobileParty party = FindMobileParty(partyId);
                if (party == null || !party.IsActive)
                {
                    disbandedPartyIds.Add(partyId);
                    continue;
                }

                if (party.MapEvent == null)
                {
                    DestroyPartyAction.Apply(null, party);
                    disbandedPartyIds.Add(partyId);
                }
            }

            foreach (string partyId in disbandedPartyIds)
            {
                _activeSeaWolvesPartyIds.Remove(partyId);
            }

            if (_activeSeaWolvesPartyIds.Count == 0)
            {
                FinishSeaWolvesDisbanding();
            }
        }

        private void FinishSeaWolvesDefeat()
        {
            TextObject targetName = GetActiveSeaWolvesTargetName();
            _activeSeaWolvesPartyIds.Clear();
            _activeSeaWolvesExpiryTime = CampaignTime.Zero;
            _activeSeaWolvesTargetSettlementId = null;
            SoundEvent.PlaySound2D("event:/ui/notification/death");
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderSeaWolvesDefeated}The Sea Wolves have been scattered or destroyed in the waters outside {TOWN_NAME}.")
                .SetTextVariable("TOWN_NAME", targetName));
        }

        private void FinishSeaWolvesDisbanding()
        {
            TextObject targetName = GetActiveSeaWolvesTargetName();
            _activeSeaWolvesPartyIds.Clear();
            _activeSeaWolvesExpiryTime = CampaignTime.Zero;
            _activeSeaWolvesTargetSettlementId = null;
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderSeaWolvesDisbanded}After fifteen days in the waters outside {TOWN_NAME}, the Sea Wolves have divided their plunder and sailed north again.")
                .SetTextVariable("TOWN_NAME", targetName));
        }

        private void FinishGreatRaidDefeat()
        {
            _activeGreatRaidPartyIds.Clear();
            _activeGreatRaidExpiryTime = CampaignTime.Zero;
            _activeGreatRaidTargetSettlementId = null;
            SoundEvent.PlaySound2D("event:/ui/notification/death");
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderGreatRaidDefeated}At heart, they were only a disorderly rabble. The entire Great Raiding force has been destroyed, and whoever set it in motion has likely failed to get what they wanted."));
        }

        private void FinishGreatRaidDisbanding()
        {
            _activeGreatRaidPartyIds.Clear();
            _activeGreatRaidExpiryTime = CampaignTime.Zero;
            _activeGreatRaidTargetSettlementId = null;
            DisplaySeaRaiderMessage(
                new TextObject(
                    "{=MVSeaRaiderGreatRaidDisbanded}Sea raiders are ultimately a rabble gathered on a passing impulse. Having seized enough coin and food, most cheerfully return home. Their invasion was never meant to last."));
        }

        private void RemoveMissingGreatRaidPartyIds()
        {
            _activeGreatRaidPartyIds.RemoveAll(partyId =>
            {
                MobileParty party = FindMobileParty(partyId);
                return party == null || !party.IsActive;
            });
        }

        private void RemoveMissingSeaWolvesPartyIds()
        {
            _activeSeaWolvesPartyIds.RemoveAll(partyId =>
            {
                MobileParty party = FindMobileParty(partyId);
                return party == null || !party.IsActive;
            });
        }

        private static MobileParty FindMobileParty(string partyId)
        {
            foreach (MobileParty party in MobileParty.All)
            {
                if (party.StringId == partyId)
                {
                    return party;
                }
            }

            return null;
        }

        private static void DisplaySeaRaiderMessage(TextObject message)
        {
            InformationManager.DisplayMessage(
                new InformationMessage(message.ToString(), SeaRaiderEventColor));
        }

        private static void PlayGreatRaidHorn()
        {
            SoundEvent.PlaySound2D("event:/alerts/horns/attack");
        }

        private static bool CanRecruit(
            string troopId,
            int count,
            int cost,
            CampaignTime nextRecruitmentTime)
        {
            MobileParty party = MobileParty.MainParty;
            CharacterObject troop = CharacterObject.Find(troopId);

            return party != null &&
                   troop != null &&
                   nextRecruitmentTime.IsPast &&
                   Hero.MainHero.Gold >= cost;
        }

        private static bool Recruit(
            string troopId,
            int count,
            int cost,
            ref CampaignTime nextRecruitmentTime)
        {
            if (!CanRecruit(troopId, count, cost, nextRecruitmentTime))
            {
                return false;
            }

            CharacterObject troop = CharacterObject.Find(troopId);
            Hero.MainHero.ChangeHeroGold(-cost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                troop,
                count,
                false,
                0,
                0,
                false,
                0);

            nextRecruitmentTime = CampaignTime.DaysFromNow(RecruitmentCooldownDays);
            GameMenu.SwitchToMenu("village");
            return true;
        }
    }
}
