using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public class WesternMercenaryOutpostMenuBehavior : CampaignBehaviorBase
    {
        private const string SargotId = "town_V1";
        private const string OutpostMenuId = "mv_western_mercenary_outpost";
        private const string DonationMenuId = "mv_western_mercenary_donation";
        private const string GoldenBoarMenuId = "mv_western_mercenary_golden_boar";
        private const string BrotherhoodOfWoodsMenuId = "mv_western_mercenary_brotherhood_of_woods";
        private const string GoldenBoarClanId = "company_of_the_boar";
        private const string BrotherhoodOfWoodsClanId = "brotherhood_of_woods";
        private const string T3WesternMercenaryTroopId = "western_mercenary";
        private const string T4WesternPikeTroopId = "western_mercenary_t4";
        private const string T4WesternCrossbowTroopId = "western_crossbow_t4";
        private const string T5WesternPikeTroopId = "western_mercenary_t5";
        private const string T5WesternCrossbowTroopId = "western_crossbow_t5";
        private const string T6WesternPikeTroopId = "mv_western_mercenary_t6";
        private const string T6WesternCrossbowTroopId = "mv_western_crossbow_t6";
        private const string VlandianBannerKnightTroopId = "vlandian_banner_knight";
        private const string GoldenBoarT2TroopId = "company_of_the_boar_tier_1";
        private const string GoldenBoarT3TroopId = "company_of_the_boar_tier_2";
        private const string GoldenBoarT4TroopId = "company_of_the_boar_tier_3";
        private const string GoldenBoarT5TroopId = "mv_company_of_the_boar_t5";
        private const string GoldenBoarT6TroopId = "mv_company_of_the_boar_t6";
        private const string BrotherhoodOfWoodsT1TroopId = "brotherhood_of_woods_tier_1";
        private const string BrotherhoodOfWoodsT2TroopId = "brotherhood_of_woods_tier_2";
        private const string BrotherhoodOfWoodsT3TroopId = "brotherhood_of_woods_tier_3";
        private const string BrotherhoodOfWoodsT4TroopId = "mv_brotherhood_of_woods_tier_4";
        private const int BasicMembershipDonation = 50000;
        private const int AdvancedMembershipDonation = 150000;
        private const int GuildSponsorshipDonation = 600000;
        private const int HonoraryGuildmasterDonation = 1200000;
        private const int T3RecruitmentCount = 10;
        private const int T3RecruitmentCost = 1200;
        private const float T3RecruitmentCooldownDays = 5f;
        private const int T4RecruitmentCountPerBranch = 5;
        private const int T4RecruitmentCost = 1800;
        private const float T4RecruitmentCooldownDays = 5f;
        private const int T5RecruitmentCountPerBranch = 5;
        private const int T5RecruitmentCost = 4000;
        private const float T5RecruitmentCooldownDays = 7f;
        private const int NobleMercenaryRecruitmentCountPerType = 5;
        private const int NobleMercenaryRecruitmentCost = 20000;
        private const float NobleMercenaryRecruitmentCooldownDays = 15f;
        private const int GoldenBoarT2RecruitmentCount = 6;
        private const int GoldenBoarT3RecruitmentCount = 6;
        private const int GoldenBoarT4RecruitmentCount = 3;
        private const int GoldenBoarRecruitmentCost = 2700;
        private const float GoldenBoarRecruitmentCooldownDays = 7f;
        private const int BrotherhoodOfWoodsT2RecruitmentCount = 6;
        private const int BrotherhoodOfWoodsT3RecruitmentCount = 6;
        private const int BrotherhoodOfWoodsT4RecruitmentCount = 3;
        private const int BrotherhoodOfWoodsRecruitmentCost = 2700;
        private const float BrotherhoodOfWoodsRecruitmentCooldownDays = 7f;
        private const int GoldenBoarCompanyT2RecruitmentCount = 25;
        private const int GoldenBoarCompanyT3RecruitmentCount = 20;
        private const int GoldenBoarCompanyT4RecruitmentCount = 15;
        private const int GoldenBoarCompanyRecruitmentCost = 9000;
        private const float GoldenBoarCompanyRecruitmentCooldownDays = 15f;
        private const int GoldenBoarServiceCost = 50000;
        private const float GoldenBoarServiceDays = 60f;
        private const int GoldenBoarBuyoutCost = 1500000;
        private const int GoldenBoarMobilizationT2Count = 30;
        private const int GoldenBoarMobilizationT3Count = 25;
        private const int GoldenBoarMobilizationT4Count = 20;
        private const int GoldenBoarMobilizationT5Count = 4;
        private const int GoldenBoarMobilizationT6Count = 1;
        private const float GoldenBoarMobilizationCooldownDays = 60f;
        private const int BrotherhoodOfWoodsServiceCost = 50000;
        private const float BrotherhoodOfWoodsServiceDays = 60f;
        private const int BrotherhoodOfWoodsBuyoutCost = 1500000;
        private const int BrotherhoodOfWoodsMobilizationT1Count = 20;
        private const int BrotherhoodOfWoodsMobilizationT2Count = 25;
        private const int BrotherhoodOfWoodsMobilizationT3Count = 25;
        private const int BrotherhoodOfWoodsMobilizationT4Count = 15;
        private const float BrotherhoodOfWoodsMobilizationCooldownDays = 60f;

        private CampaignTime _nextT3RecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextT4RecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextT5RecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextNobleMercenaryRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextGoldenBoarRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextGoldenBoarCompanyRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextGoldenBoarMobilizationTime = CampaignTime.Zero;
        private CampaignTime _nextBrotherhoodOfWoodsRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextBrotherhoodOfWoodsMobilizationTime = CampaignTime.Zero;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(
                this,
                OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(
                "mv_western_mercenary_next_t3_recruitment_time",
                ref _nextT3RecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_t4_recruitment_time",
                ref _nextT4RecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_t5_recruitment_time",
                ref _nextT5RecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_noble_recruitment_time",
                ref _nextNobleMercenaryRecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_golden_boar_recruitment_time",
                ref _nextGoldenBoarRecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_golden_boar_company_recruitment_time",
                ref _nextGoldenBoarCompanyRecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_golden_boar_mobilization_time",
                ref _nextGoldenBoarMobilizationTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_brotherhood_of_woods_recruitment_time",
                ref _nextBrotherhoodOfWoodsRecruitmentTime);
            dataStore.SyncData(
                "mv_western_mercenary_next_brotherhood_of_woods_mobilization_time",
                ref _nextBrotherhoodOfWoodsMobilizationTime);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                "town",
                "mv_western_mercenary_outpost_entry",
                "{=MVWesternMercenaryOutpostEntry}Enter the Mercenary Guild",
                args =>
                {
                    if (!IsSargot())
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(OutpostMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                OutpostMenuId,
                "{=MVWesternMercenaryOutpostMenu}Sargot lies near the meeting point of Vlandia, Battania, and the Western Empire. Merchants need escorts, nobles need private soldiers, and the border wars never lack men willing to fight for coin. Veterans, landless knights, and wandering crossbowmen come here to offer their skills to the highest bidder who can keep a promise. In time, the city has become one of the most important gathering places for mercenaries.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenu(
                DonationMenuId,
                "{=MVWesternMercenaryDonationMenu}Mercenaries accept contracts from any employer, but capable warriors are not easily found. If you offer the guild a donation and become an honored patron, its staff may introduce you to many exceptional fighters.\nYour current permission level is: {MEMBERSHIP_LEVEL}",
                args => MBTextManager.SetTextVariable(
                    "MEMBERSHIP_LEVEL",
                    GetMembershipLevelText(),
                    false),
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenu(
                GoldenBoarMenuId,
                "{=MVGoldenBoarMenu}Once you have attained a certain standing in Sargot's Mercenary Guild, many members of the Company of the Golden Boar will be willing to make contact. With sufficient influence and wealth, you may even be able to buy the entire company.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenu(
                BrotherhoodOfWoodsMenuId,
                "{=MVBrotherhoodOfWoodsMenu}The Brotherhood of the Woods has long hidden among Vlandia's forests, recruiting desperate peasants and skilled poachers alike. At the guild's highest rank, you can force the Brotherhood into your kingdom's service or buy out its remaining organization entirely.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_donation_entry",
                "{=MVWesternMercenaryDonationEntry}Donate to the Mercenary Guild",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(DonationMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_golden_boar_entry",
                "{=MVGoldenBoarEntry}Contact the Company of the Golden Boar",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(GoldenBoarMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_brotherhood_of_woods_entry",
                "{=MVBrotherhoodOfWoodsEntry}Contact the Brotherhood of the Woods",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(BrotherhoodOfWoodsMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_recruit_t3",
                "{=MVWesternMercenaryRecruitT3}Recruit 10 T3 Western Mercenaries (1200 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT3WesternMercenaries();
                    args.Tooltip = new TextObject(
                        "{=MVWesternMercenaryRecruitT3Tooltip}Requires Basic Mercenary Guild membership, 1200 denars, and an available 5-day recruitment cooldown.");
                    return true;
                },
                args => RecruitT3WesternMercenaries(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_recruit_t4",
                "{=MVWesternMercenaryRecruitT4}Recruit a Band of Seasoned Veterans (5 Pikes, 5 Crossbowmen; 1800 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT4WesternMercenaries();
                    args.Tooltip = new TextObject(
                        "{=MVWesternMercenaryRecruitT4Tooltip}Requires Advanced Mercenary Guild membership, 1800 denars, and an available 5-day recruitment cooldown.");
                    return true;
                },
                args => RecruitT4WesternMercenaries(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_recruit_t5",
                "{=MVWesternMercenaryRecruitT5}Recruit an Elite Warrior Detachment (5 Elite Pikes, 5 Elite Crossbowmen; 4000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT5WesternMercenaries();
                    args.Tooltip = new TextObject(
                        "{=MVWesternMercenaryRecruitT5Tooltip}Requires Mercenary Guild sponsorship, 4000 denars, and an available 7-day recruitment cooldown.");
                    return true;
                },
                args => RecruitT5WesternMercenaries(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_recruit_noble_company",
                "{=MVWesternMercenaryRecruitNobleCompany}Recruit Noble Mercenaries (5 T6 Pikemen, 5 T6 Crossbowmen, 5 Vlandian Banner Knights; 20000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitNobleMercenaries();
                    args.Tooltip = new TextObject(
                        "{=MVWesternMercenaryRecruitNobleCompanyTooltip}Requires Honorary Guildmaster status, 20000 denars, and an available 15-day recruitment cooldown.");
                    return true;
                },
                args => RecruitNobleMercenaries(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                OutpostMenuId,
                "mv_western_mercenary_outpost_leave",
                "{=MVWesternMercenaryOutpostLeave}Leave",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                args => GameMenu.SwitchToMenu("town"),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                DonationMenuId,
                "mv_western_mercenary_donate_basic_membership",
                "{=MVWesternMercenaryDonateBasicMembership}Donate 50000 denars to become a Basic Mercenary Guild Member",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanDonateForBasicMembership();
                    args.Tooltip = GetBasicMembershipDonationTooltip();
                    return true;
                },
                args => DonateForBasicMembership(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                DonationMenuId,
                "mv_western_mercenary_donate_advanced_membership",
                "{=MVWesternMercenaryDonateAdvancedMembership}Donate 150000 denars to become an Advanced Mercenary Guild Member",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanDonateForAdvancedMembership();
                    args.Tooltip = GetAdvancedMembershipDonationTooltip();
                    return true;
                },
                args => DonateForAdvancedMembership(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                DonationMenuId,
                "mv_western_mercenary_donate_guild_sponsorship",
                "{=MVWesternMercenaryDonateGuildSponsorship}Donate 600000 denars to become a Mercenary Guild Sponsor",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanDonateForGuildSponsorship();
                    args.Tooltip = GetGuildSponsorshipDonationTooltip();
                    return true;
                },
                args => DonateForGuildSponsorship(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                DonationMenuId,
                "mv_western_mercenary_donate_honorary_guildmaster",
                "{=MVWesternMercenaryDonateHonoraryGuildmaster}Donate 1200000 denars to become the Honorary Guildmaster",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanDonateForHonoraryGuildmaster();
                    args.Tooltip = GetHonoraryGuildmasterDonationTooltip();
                    return true;
                },
                args => DonateForHonoraryGuildmaster(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                DonationMenuId,
                "mv_western_mercenary_donation_back",
                "{=MVWesternMercenaryDonationBack}Return to the Mercenary Guild",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(OutpostMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GoldenBoarMenuId,
                "mv_western_mercenary_recruit_golden_boar",
                "{=MVGoldenBoarRecruit}Recruit a Company of the Golden Boar Detachment ({GOLDEN_BOAR_RECRUITMENT_COST} denars)",
                args =>
                {
                    MBTextManager.SetTextVariable(
                        "GOLDEN_BOAR_RECRUITMENT_COST",
                        GetGoldenBoarRecruitmentCost());
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitGoldenBoarDetachment();
                    args.Tooltip = GetGoldenBoarRecruitmentTooltip();
                    return true;
                },
                args => RecruitGoldenBoarDetachment(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GoldenBoarMenuId,
                "mv_western_mercenary_hire_golden_boar_company",
                "{=MVGoldenBoarCompanyHire}Hire the Company of the Golden Boar into Your Party (25 Novices, 20 Veterans, 15 Champions; {GOLDEN_BOAR_COMPANY_COST} denars)",
                args =>
                {
                    MBTextManager.SetTextVariable(
                        "GOLDEN_BOAR_COMPANY_COST",
                        GetGoldenBoarCompanyRecruitmentCost());
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanHireGoldenBoarCompany();
                    args.Tooltip = GetGoldenBoarCompanyRecruitmentTooltip();
                    return true;
                },
                args => HireGoldenBoarCompany(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GoldenBoarMenuId,
                "mv_western_mercenary_demand_golden_boar_service",
                "{=MVGoldenBoarDemandService}Require the Company of the Golden Boar to Take the Field (50000 denars)",
                args =>
                {
                    WesternMercenaryGuildProgressBehavior progress =
                        WesternMercenaryGuildProgressBehavior.Instance;
                    if (progress != null && progress.IsGoldenBoarBoughtOut)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanDemandGoldenBoarService();
                    args.Tooltip = GetGoldenBoarServiceTooltip();
                    return true;
                },
                args => DemandGoldenBoarService(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GoldenBoarMenuId,
                "mv_western_mercenary_mobilize_bought_out_golden_boar",
                "{=MVGoldenBoarMobilizeBoughtOut}Call the Company of the Golden Boar to Arms (receive 80 troops)",
                args =>
                {
                    WesternMercenaryGuildProgressBehavior progress =
                        WesternMercenaryGuildProgressBehavior.Instance;
                    if (progress == null || !progress.IsGoldenBoarBoughtOut)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanMobilizeBoughtOutGoldenBoar();
                    args.Tooltip = GetGoldenBoarMobilizationTooltip();
                    return true;
                },
                args => MobilizeBoughtOutGoldenBoar(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GoldenBoarMenuId,
                "mv_western_mercenary_buyout_golden_boar",
                "{=MVGoldenBoarBuyout}Buy Out the Company of the Golden Boar (1500000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanBuyOutGoldenBoarCompany();
                    args.Tooltip = GetGoldenBoarBuyoutTooltip();
                    return true;
                },
                args => BuyOutGoldenBoarCompany(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GoldenBoarMenuId,
                "mv_western_mercenary_golden_boar_back",
                "{=MVGoldenBoarBack}Return to the Mercenary Guild",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(OutpostMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                BrotherhoodOfWoodsMenuId,
                "mv_western_mercenary_recruit_brotherhood_of_woods_detachment",
                "{=MVBrotherhoodOfWoodsRecruitDetachment}Recruit a Brotherhood Detachment: 6 Saplings, 6 Arboreal Brothers and 3 Ancients ({BROTHERHOOD_OF_WOODS_RECRUITMENT_COST} denars)",
                args =>
                {
                    WesternMercenaryGuildProgressBehavior progress =
                        WesternMercenaryGuildProgressBehavior.Instance;
                    if (progress != null && progress.IsBrotherhoodOfWoodsBoughtOut)
                    {
                        return false;
                    }

                    MBTextManager.SetTextVariable(
                        "BROTHERHOOD_OF_WOODS_RECRUITMENT_COST",
                        BrotherhoodOfWoodsRecruitmentCost);
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitBrotherhoodOfWoodsDetachment();
                    args.Tooltip = GetBrotherhoodOfWoodsRecruitmentTooltip();
                    return true;
                },
                args => RecruitBrotherhoodOfWoodsDetachment(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                BrotherhoodOfWoodsMenuId,
                "mv_western_mercenary_demand_brotherhood_of_woods_service",
                "{=MVBrotherhoodOfWoodsDemandService}Require the Brotherhood of the Woods to Take the Field (50000 denars)",
                args =>
                {
                    WesternMercenaryGuildProgressBehavior progress =
                        WesternMercenaryGuildProgressBehavior.Instance;
                    if (progress != null && progress.IsBrotherhoodOfWoodsBoughtOut)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanDemandBrotherhoodOfWoodsService();
                    args.Tooltip = GetBrotherhoodOfWoodsServiceTooltip();
                    return true;
                },
                args => DemandBrotherhoodOfWoodsService(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                BrotherhoodOfWoodsMenuId,
                "mv_western_mercenary_mobilize_bought_out_brotherhood_of_woods",
                "{=MVBrotherhoodOfWoodsMobilize}Call the Brotherhood of the Woods to Arms (receive 85 troops)",
                args =>
                {
                    WesternMercenaryGuildProgressBehavior progress =
                        WesternMercenaryGuildProgressBehavior.Instance;
                    if (progress == null || !progress.IsBrotherhoodOfWoodsBoughtOut)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanMobilizeBoughtOutBrotherhoodOfWoods();
                    args.Tooltip = GetBrotherhoodOfWoodsMobilizationTooltip();
                    return true;
                },
                args => MobilizeBoughtOutBrotherhoodOfWoods(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                BrotherhoodOfWoodsMenuId,
                "mv_western_mercenary_buyout_brotherhood_of_woods",
                "{=MVBrotherhoodOfWoodsBuyout}Buy Out the Brotherhood of the Woods (1500000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Continue;
                    args.IsEnabled = CanBuyOutBrotherhoodOfWoods();
                    args.Tooltip = GetBrotherhoodOfWoodsBuyoutTooltip();
                    return true;
                },
                args => BuyOutBrotherhoodOfWoods(),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                BrotherhoodOfWoodsMenuId,
                "mv_western_mercenary_brotherhood_of_woods_back",
                "{=MVBrotherhoodOfWoodsBack}Return to the Mercenary Guild",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(OutpostMenuId),
                true,
                -1,
                false,
                null);
        }

        private static bool IsSargot()
        {
            Settlement settlement = Settlement.CurrentSettlement;
            return settlement != null && settlement.StringId == SargotId;
        }

        private static bool CanDonateForBasicMembership()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && !progress.IsBasicMember &&
                   Hero.MainHero != null &&
                   Hero.MainHero.Gold >= BasicMembershipDonation;
        }

        private static TextObject GetBasicMembershipDonationTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress != null && progress.IsBasicMember)
            {
                return new TextObject(
                    "{=MVWesternMercenaryAlreadyBasicMember}You are already a Basic Mercenary Guild Member.");
            }

            return new TextObject(
                "{=MVWesternMercenaryDonationBasicTooltip}Requires 50000 denars.");
        }

        private static TextObject GetMembershipLevelText()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress != null && progress.IsHonoraryGuildmaster)
            {
                return new TextObject(
                    "{=MVWesternMercenaryMembershipHonoraryGuildmaster}Honorary Guildmaster");
            }

            if (progress != null && progress.IsGuildSponsor)
            {
                return new TextObject(
                    "{=MVWesternMercenaryMembershipSponsor}Mercenary Guild Sponsor");
            }

            if (progress != null && progress.IsAdvancedMember)
            {
                return new TextObject(
                    "{=MVWesternMercenaryMembershipAdvanced}Advanced Mercenary Guild Member");
            }

            return progress != null && progress.IsBasicMember
                ? new TextObject(
                    "{=MVWesternMercenaryMembershipBasic}Basic Mercenary Guild Member")
                : new TextObject(
                    "{=MVWesternMercenaryMembershipVisitor}Visitor");
        }

        private static void DonateForBasicMembership()
        {
            if (!CanDonateForBasicMembership())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-BasicMembershipDonation);
            WesternMercenaryGuildProgressBehavior.Instance.GrantBasicMembership();
            GameMenu.SwitchToMenu(OutpostMenuId);
        }

        private static bool CanDonateForAdvancedMembership()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsBasicMember &&
                   !progress.IsAdvancedMember && Hero.MainHero != null &&
                   Clan.PlayerClan != null && Clan.PlayerClan.Tier >= 3 &&
                   Hero.MainHero.Gold >= AdvancedMembershipDonation;
        }

        private static TextObject GetAdvancedMembershipDonationTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsBasicMember)
            {
                return new TextObject(
                    "{=MVWesternMercenaryAdvancedRequiresBasic}Requires Basic Mercenary Guild membership.");
            }

            if (progress.IsAdvancedMember)
            {
                return new TextObject(
                    "{=MVWesternMercenaryAlreadyAdvancedMember}You are already an Advanced Mercenary Guild Member.");
            }

            if (Clan.PlayerClan == null || Clan.PlayerClan.Tier < 3)
            {
                return new TextObject(
                    "{=MVWesternMercenaryAdvancedRequiresClanTier}Requires Clan Tier 3.");
            }

            return new TextObject(
                "{=MVWesternMercenaryDonationAdvancedTooltip}Requires 150000 denars.");
        }

        private static void DonateForAdvancedMembership()
        {
            if (!CanDonateForAdvancedMembership())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-AdvancedMembershipDonation);
            WesternMercenaryGuildProgressBehavior.Instance.GrantAdvancedMembership();
            GameMenu.SwitchToMenu(OutpostMenuId);
        }

        private static bool CanDonateForGuildSponsorship()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsAdvancedMember &&
                   !progress.IsGuildSponsor && Hero.MainHero != null &&
                   Clan.PlayerClan != null && Clan.PlayerClan.Tier >= 4 &&
                   Hero.MainHero.Gold >= GuildSponsorshipDonation;
        }

        private static TextObject GetGuildSponsorshipDonationTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsAdvancedMember)
            {
                return new TextObject(
                    "{=MVWesternMercenarySponsorRequiresAdvanced}Requires Advanced Mercenary Guild membership.");
            }

            if (progress.IsGuildSponsor)
            {
                return new TextObject(
                    "{=MVWesternMercenaryAlreadySponsor}You are already a Mercenary Guild Sponsor.");
            }

            if (Clan.PlayerClan == null || Clan.PlayerClan.Tier < 4)
            {
                return new TextObject(
                    "{=MVWesternMercenarySponsorRequiresClanTier}Requires Clan Tier 4.");
            }

            return new TextObject(
                "{=MVWesternMercenaryDonationSponsorTooltip}Requires 600000 denars.");
        }

        private static void DonateForGuildSponsorship()
        {
            if (!CanDonateForGuildSponsorship())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-GuildSponsorshipDonation);
            WesternMercenaryGuildProgressBehavior.Instance.GrantGuildSponsorship();
            GameMenu.SwitchToMenu(OutpostMenuId);
        }

        private static bool CanDonateForHonoraryGuildmaster()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsGuildSponsor &&
                   !progress.IsHonoraryGuildmaster && Hero.MainHero != null &&
                   Hero.MainHero.Gold >= HonoraryGuildmasterDonation;
        }

        private static TextObject GetHonoraryGuildmasterDonationTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsGuildSponsor)
            {
                return new TextObject(
                    "{=MVWesternMercenaryHonoraryRequiresSponsor}Requires Mercenary Guild sponsorship.");
            }

            if (progress.IsHonoraryGuildmaster)
            {
                return new TextObject(
                    "{=MVWesternMercenaryAlreadyHonoraryGuildmaster}You are already the Honorary Guildmaster.");
            }

            return new TextObject(
                "{=MVWesternMercenaryDonationHonoraryTooltip}Requires 1200000 denars.");
        }

        private static void DonateForHonoraryGuildmaster()
        {
            if (!CanDonateForHonoraryGuildmaster())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-HonoraryGuildmasterDonation);
            WesternMercenaryGuildProgressBehavior.Instance.GrantHonoraryGuildmaster();
            GameMenu.SwitchToMenu(OutpostMenuId);
        }

        private bool CanRecruitT3WesternMercenaries()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject troop = CharacterObject.Find(T3WesternMercenaryTroopId);

            return progress != null && progress.IsBasicMember &&
                   Hero.MainHero != null && MobileParty.MainParty != null &&
                   troop != null && Hero.MainHero.Gold >= T3RecruitmentCost &&
                   _nextT3RecruitmentTime.IsPast;
        }

        private void RecruitT3WesternMercenaries()
        {
            if (!CanRecruitT3WesternMercenaries())
            {
                return;
            }

            CharacterObject troop = CharacterObject.Find(T3WesternMercenaryTroopId);
            Hero.MainHero.ChangeHeroGold(-T3RecruitmentCost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                troop,
                T3RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            _nextT3RecruitmentTime = CampaignTime.DaysFromNow(
                T3RecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private bool CanRecruitT4WesternMercenaries()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject pikeTroop = CharacterObject.Find(T4WesternPikeTroopId);
            CharacterObject crossbowTroop = CharacterObject.Find(T4WesternCrossbowTroopId);

            return progress != null && progress.IsAdvancedMember &&
                   Hero.MainHero != null && MobileParty.MainParty != null &&
                   pikeTroop != null && crossbowTroop != null &&
                   Hero.MainHero.Gold >= T4RecruitmentCost &&
                   _nextT4RecruitmentTime.IsPast;
        }

        private void RecruitT4WesternMercenaries()
        {
            if (!CanRecruitT4WesternMercenaries())
            {
                return;
            }

            CharacterObject pikeTroop = CharacterObject.Find(T4WesternPikeTroopId);
            CharacterObject crossbowTroop = CharacterObject.Find(T4WesternCrossbowTroopId);
            Hero.MainHero.ChangeHeroGold(-T4RecruitmentCost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                pikeTroop,
                T4RecruitmentCountPerBranch,
                false,
                0,
                0,
                false,
                0);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                crossbowTroop,
                T4RecruitmentCountPerBranch,
                false,
                0,
                0,
                false,
                0);
            _nextT4RecruitmentTime = CampaignTime.DaysFromNow(
                T4RecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private bool CanRecruitT5WesternMercenaries()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject pikeTroop = CharacterObject.Find(T5WesternPikeTroopId);
            CharacterObject crossbowTroop = CharacterObject.Find(T5WesternCrossbowTroopId);

            return progress != null && progress.IsGuildSponsor &&
                   Hero.MainHero != null && MobileParty.MainParty != null &&
                   pikeTroop != null && crossbowTroop != null &&
                   Hero.MainHero.Gold >= T5RecruitmentCost &&
                   _nextT5RecruitmentTime.IsPast;
        }

        private void RecruitT5WesternMercenaries()
        {
            if (!CanRecruitT5WesternMercenaries())
            {
                return;
            }

            CharacterObject pikeTroop = CharacterObject.Find(T5WesternPikeTroopId);
            CharacterObject crossbowTroop = CharacterObject.Find(T5WesternCrossbowTroopId);
            Hero.MainHero.ChangeHeroGold(-T5RecruitmentCost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                pikeTroop,
                T5RecruitmentCountPerBranch,
                false,
                0,
                0,
                false,
                0);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                crossbowTroop,
                T5RecruitmentCountPerBranch,
                false,
                0,
                0,
                false,
                0);
            _nextT5RecruitmentTime = CampaignTime.DaysFromNow(
                T5RecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private bool CanRecruitNobleMercenaries()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject pikeTroop = CharacterObject.Find(T6WesternPikeTroopId);
            CharacterObject crossbowTroop = CharacterObject.Find(T6WesternCrossbowTroopId);
            CharacterObject bannerKnight = CharacterObject.Find(VlandianBannerKnightTroopId);

            return progress != null && progress.IsHonoraryGuildmaster &&
                   Hero.MainHero != null && MobileParty.MainParty != null &&
                   pikeTroop != null && crossbowTroop != null && bannerKnight != null &&
                   Hero.MainHero.Gold >= NobleMercenaryRecruitmentCost &&
                   _nextNobleMercenaryRecruitmentTime.IsPast;
        }

        private void RecruitNobleMercenaries()
        {
            if (!CanRecruitNobleMercenaries())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-NobleMercenaryRecruitmentCost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(T6WesternPikeTroopId),
                NobleMercenaryRecruitmentCountPerType);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(T6WesternCrossbowTroopId),
                NobleMercenaryRecruitmentCountPerType);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(VlandianBannerKnightTroopId),
                NobleMercenaryRecruitmentCountPerType);
            _nextNobleMercenaryRecruitmentTime = CampaignTime.DaysFromNow(
                NobleMercenaryRecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private static int GetGoldenBoarRecruitmentCost()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsGoldenBoarBoughtOut
                ? 0
                : GoldenBoarRecruitmentCost;
        }

        private static int GetGoldenBoarCompanyRecruitmentCost()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsGoldenBoarBoughtOut
                ? 0
                : GoldenBoarCompanyRecruitmentCost;
        }

        private static TextObject GetGoldenBoarRecruitmentTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsGoldenBoarBoughtOut
                ? new TextObject(
                    "{=MVGoldenBoarRecruitFreeTooltip}The Company has been bought out. This recruitment is free; Basic Mercenary Guild membership and the 7-day cooldown are still required.")
                : new TextObject(
                    "{=MVGoldenBoarRecruitTooltip}Requires Basic Mercenary Guild membership, 2700 denars, and an available 7-day recruitment cooldown.");
        }

        private static TextObject GetGoldenBoarCompanyRecruitmentTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            return progress != null && progress.IsGoldenBoarBoughtOut
                ? new TextObject(
                    "{=MVGoldenBoarCompanyHireFreeTooltip}The Company has been bought out. This recruitment is free; Mercenary Guild sponsorship and the 15-day cooldown are still required.")
                : new TextObject(
                    "{=MVGoldenBoarCompanyHireTooltip}Requires Mercenary Guild sponsorship, 9000 denars, and an available 15-day recruitment cooldown.");
        }

        private bool CanRecruitGoldenBoarDetachment()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject t2Troop = CharacterObject.Find(GoldenBoarT2TroopId);
            CharacterObject t3Troop = CharacterObject.Find(GoldenBoarT3TroopId);
            CharacterObject t4Troop = CharacterObject.Find(GoldenBoarT4TroopId);

            return progress != null && progress.IsBasicMember &&
                   Hero.MainHero != null && MobileParty.MainParty != null &&
                   t2Troop != null && t3Troop != null && t4Troop != null &&
                   Hero.MainHero.Gold >= GetGoldenBoarRecruitmentCost() &&
                   _nextGoldenBoarRecruitmentTime.IsPast;
        }

        private void RecruitGoldenBoarDetachment()
        {
            if (!CanRecruitGoldenBoarDetachment())
            {
                return;
            }

            CharacterObject t2Troop = CharacterObject.Find(GoldenBoarT2TroopId);
            CharacterObject t3Troop = CharacterObject.Find(GoldenBoarT3TroopId);
            CharacterObject t4Troop = CharacterObject.Find(GoldenBoarT4TroopId);
            Hero.MainHero.ChangeHeroGold(-GetGoldenBoarRecruitmentCost());
            MobileParty.MainParty.MemberRoster.AddToCounts(
                t2Troop,
                GoldenBoarT2RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                t3Troop,
                GoldenBoarT3RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                t4Troop,
                GoldenBoarT4RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            _nextGoldenBoarRecruitmentTime = CampaignTime.DaysFromNow(
                GoldenBoarRecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private bool CanHireGoldenBoarCompany()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject t2Troop = CharacterObject.Find(GoldenBoarT2TroopId);
            CharacterObject t3Troop = CharacterObject.Find(GoldenBoarT3TroopId);
            CharacterObject t4Troop = CharacterObject.Find(GoldenBoarT4TroopId);

            return progress != null && progress.IsGuildSponsor &&
                   Hero.MainHero != null && MobileParty.MainParty != null &&
                   t2Troop != null && t3Troop != null && t4Troop != null &&
                   Hero.MainHero.Gold >= GetGoldenBoarCompanyRecruitmentCost() &&
                   _nextGoldenBoarCompanyRecruitmentTime.IsPast;
        }

        private void HireGoldenBoarCompany()
        {
            if (!CanHireGoldenBoarCompany())
            {
                return;
            }

            CharacterObject t2Troop = CharacterObject.Find(GoldenBoarT2TroopId);
            CharacterObject t3Troop = CharacterObject.Find(GoldenBoarT3TroopId);
            CharacterObject t4Troop = CharacterObject.Find(GoldenBoarT4TroopId);
            Hero.MainHero.ChangeHeroGold(-GetGoldenBoarCompanyRecruitmentCost());
            MobileParty.MainParty.MemberRoster.AddToCounts(
                t2Troop,
                GoldenBoarCompanyT2RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                t3Troop,
                GoldenBoarCompanyT3RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                t4Troop,
                GoldenBoarCompanyT4RecruitmentCount,
                false,
                0,
                0,
                false,
                0);
            _nextGoldenBoarCompanyRecruitmentTime = CampaignTime.DaysFromNow(
                GoldenBoarCompanyRecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private static bool CanDemandGoldenBoarService()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            Clan goldenBoar = Clan.FindFirst(clan => clan.StringId == GoldenBoarClanId);

            return progress != null && progress.IsHonoraryGuildmaster &&
                   !progress.IsGoldenBoarBoughtOut && goldenBoar != null &&
                   Clan.PlayerClan?.Kingdom != null && Hero.MainHero != null &&
                   Hero.MainHero.Gold >= GoldenBoarServiceCost;
        }

        private static TextObject GetGoldenBoarServiceTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsHonoraryGuildmaster)
            {
                return new TextObject(
                    "{=MVGoldenBoarServiceRequiresHonorary}Requires Honorary Guildmaster status.");
            }

            if (progress.IsGoldenBoarBoughtOut)
            {
                return new TextObject(
                    "{=MVGoldenBoarServiceBoughtOut}The Company has been bought out and dissolved, so it can no longer take the field as an independent mercenary clan.");
            }

            if (Clan.FindFirst(clan => clan.StringId == GoldenBoarClanId) == null)
            {
                return new TextObject(
                    "{=MVGoldenBoarClanUnavailable}The Company of the Golden Boar no longer exists.");
            }

            if (Clan.PlayerClan?.Kingdom == null)
            {
                return new TextObject(
                    "{=MVGoldenBoarServiceNoKingdom}You must belong to a kingdom before the Company can enter its service.");
            }

            if (Hero.MainHero == null || Hero.MainHero.Gold < GoldenBoarServiceCost)
            {
                return new TextObject(
                    "{=MVGoldenBoarServiceRequiresGold}Requires 50000 denars.");
            }

            return new TextObject(
                "{=MVGoldenBoarServiceTooltip}The Company will serve your kingdom for sixty days. Costs 50000 denars.");
        }

        private static bool CanBuyOutGoldenBoarCompany()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            Clan goldenBoar = Clan.FindFirst(clan => clan.StringId == GoldenBoarClanId);

            return progress != null && progress.IsHonoraryGuildmaster &&
                   !progress.IsGoldenBoarBoughtOut && goldenBoar != null &&
                   Hero.MainHero != null && Hero.MainHero.Gold >= GoldenBoarBuyoutCost;
        }

        private static TextObject GetGoldenBoarBuyoutTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsHonoraryGuildmaster)
            {
                return new TextObject(
                    "{=MVGoldenBoarBuyoutRequiresHonorary}Requires Honorary Guildmaster status.");
            }

            if (progress.IsGoldenBoarBoughtOut)
            {
                return new TextObject(
                    "{=MVGoldenBoarAlreadyBoughtOut}The Company of the Golden Boar has already been bought out.");
            }

            if (Clan.FindFirst(clan => clan.StringId == GoldenBoarClanId) == null)
            {
                return new TextObject(
                    "{=MVGoldenBoarClanUnavailable}The Company of the Golden Boar no longer exists.");
            }

            if (Hero.MainHero == null || Hero.MainHero.Gold < GoldenBoarBuyoutCost)
            {
                return new TextObject(
                    "{=MVGoldenBoarBuyoutRequiresGold}Requires 1500000 denars.");
            }

            return new TextObject(
                "{=MVGoldenBoarBuyoutTooltip}Dissolves the Company and makes its guild recruitment channels free. Costs 1500000 denars.");
        }

        private bool CanMobilizeBoughtOutGoldenBoar()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;

            return progress != null && progress.IsHonoraryGuildmaster &&
                   progress.IsGoldenBoarBoughtOut && MobileParty.MainParty != null &&
                   CharacterObject.Find(GoldenBoarT2TroopId) != null &&
                   CharacterObject.Find(GoldenBoarT3TroopId) != null &&
                   CharacterObject.Find(GoldenBoarT4TroopId) != null &&
                   CharacterObject.Find(GoldenBoarT5TroopId) != null &&
                   CharacterObject.Find(GoldenBoarT6TroopId) != null &&
                   _nextGoldenBoarMobilizationTime.IsPast;
        }

        private TextObject GetGoldenBoarMobilizationTooltip()
        {
            if (!_nextGoldenBoarMobilizationTime.IsPast)
            {
                return new TextObject(
                    "{=MVGoldenBoarMobilizationCooldown}The bought-out Company's warriors can only be called to arms once every sixty days.");
            }

            return new TextObject(
                "{=MVGoldenBoarMobilizationTooltip}Immediately receive 30 Novices, 25 Veterans, 20 Champions, 4 Captains, and 1 Boar Centurion. No party-size limit is applied. Available once every sixty days.");
        }

        private void MobilizeBoughtOutGoldenBoar()
        {
            if (!CanMobilizeBoughtOutGoldenBoar())
            {
                return;
            }

            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(GoldenBoarT2TroopId),
                GoldenBoarMobilizationT2Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(GoldenBoarT3TroopId),
                GoldenBoarMobilizationT3Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(GoldenBoarT4TroopId),
                GoldenBoarMobilizationT4Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(GoldenBoarT5TroopId),
                GoldenBoarMobilizationT5Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(GoldenBoarT6TroopId),
                GoldenBoarMobilizationT6Count);
            _nextGoldenBoarMobilizationTime = CampaignTime.DaysFromNow(
                GoldenBoarMobilizationCooldownDays);

            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVGoldenBoarMobilizationSuccess}Eighty warriors of the bought-out Company of the Golden Boar have answered your call, led by a Boar Centurion.")
                    .ToString()));
            GameMenu.SwitchToMenu("town");
        }

        private static void DemandGoldenBoarService()
        {
            if (!CanDemandGoldenBoarService())
            {
                return;
            }

            Clan goldenBoar = Clan.FindFirst(clan => clan.StringId == GoldenBoarClanId);
            Kingdom playerKingdom = Clan.PlayerClan?.Kingdom;

            CampaignTime contractEnd = CampaignTime.DaysFromNow(GoldenBoarServiceDays);
            if (goldenBoar.Kingdom != null && goldenBoar.Kingdom != playerKingdom)
            {
                ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(goldenBoar, true);
            }

            if (goldenBoar.Kingdom != playerKingdom)
            {
                ChangeKingdomAction.ApplyByJoinFactionAsMercenary(
                    goldenBoar,
                    playerKingdom,
                    contractEnd,
                    1,
                    true);
            }
            else
            {
                goldenBoar.ShouldStayInKingdomUntil = contractEnd;
                goldenBoar.MercenaryAwardMultiplier = 1;
            }

            Hero.MainHero.ChangeHeroGold(-GoldenBoarServiceCost);
            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVGoldenBoarServiceSuccess}The Company of the Golden Boar has entered your kingdom's service for sixty days.")
                    .ToString()));
            GameMenu.SwitchToMenu(GoldenBoarMenuId);
        }

        private static void BuyOutGoldenBoarCompany()
        {
            if (!CanBuyOutGoldenBoarCompany())
            {
                return;
            }

            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            Clan goldenBoar = Clan.FindFirst(clan => clan.StringId == GoldenBoarClanId);

            Hero.MainHero.ChangeHeroGold(-GoldenBoarBuyoutCost);
            progress?.MarkGoldenBoarBoughtOut();
            DestroyClanAction.Apply(goldenBoar);
            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVGoldenBoarBuyoutSuccess}The Company of the Golden Boar has been dissolved. Its recruitment channels now serve you without charge.")
                    .ToString()));
            GameMenu.SwitchToMenu(GoldenBoarMenuId);
        }

        private bool CanRecruitBrotherhoodOfWoodsDetachment()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            CharacterObject t2Troop = CharacterObject.Find(BrotherhoodOfWoodsT2TroopId);
            CharacterObject t3Troop = CharacterObject.Find(BrotherhoodOfWoodsT3TroopId);
            CharacterObject t4Troop = CharacterObject.Find(BrotherhoodOfWoodsT4TroopId);

            return progress != null && progress.IsBasicMember &&
                   !progress.IsBrotherhoodOfWoodsBoughtOut && Hero.MainHero != null &&
                   MobileParty.MainParty != null && t2Troop != null && t3Troop != null &&
                   t4Troop != null &&
                   Hero.MainHero.Gold >= BrotherhoodOfWoodsRecruitmentCost &&
                   _nextBrotherhoodOfWoodsRecruitmentTime.IsPast;
        }

        private TextObject GetBrotherhoodOfWoodsRecruitmentTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsBasicMember)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsRecruitRequiresBasic}Requires Basic Mercenary Guild membership.");
            }

            if (Hero.MainHero == null || Hero.MainHero.Gold < BrotherhoodOfWoodsRecruitmentCost)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsRecruitRequiresGold}Requires 2700 denars.");
            }

            if (!_nextBrotherhoodOfWoodsRecruitmentTime.IsPast)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsRecruitmentCooldown}The Brotherhood of the Woods can provide one detachment every seven days.");
            }

            return new TextObject(
                "{=MVBrotherhoodOfWoodsRecruitmentTooltip}Recruit 6 Saplings, 6 Arboreal Brothers, and 3 Ancients. Requires Basic Mercenary Guild membership and costs 2700 denars. Available once every seven days.");
        }

        private void RecruitBrotherhoodOfWoodsDetachment()
        {
            if (!CanRecruitBrotherhoodOfWoodsDetachment())
            {
                return;
            }

            Hero.MainHero.ChangeHeroGold(-BrotherhoodOfWoodsRecruitmentCost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT2TroopId),
                BrotherhoodOfWoodsT2RecruitmentCount);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT3TroopId),
                BrotherhoodOfWoodsT3RecruitmentCount);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT4TroopId),
                BrotherhoodOfWoodsT4RecruitmentCount);
            _nextBrotherhoodOfWoodsRecruitmentTime = CampaignTime.DaysFromNow(
                BrotherhoodOfWoodsRecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }

        private static bool CanDemandBrotherhoodOfWoodsService()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            Clan brotherhood = Clan.FindFirst(clan => clan.StringId == BrotherhoodOfWoodsClanId);

            return progress != null && progress.IsHonoraryGuildmaster &&
                   !progress.IsBrotherhoodOfWoodsBoughtOut && brotherhood != null &&
                   Clan.PlayerClan?.Kingdom != null && Hero.MainHero != null &&
                   Hero.MainHero.Gold >= BrotherhoodOfWoodsServiceCost;
        }

        private static TextObject GetBrotherhoodOfWoodsServiceTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsHonoraryGuildmaster)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsRequiresHonorary}Requires Honorary Guildmaster status.");
            }

            if (progress.IsBrotherhoodOfWoodsBoughtOut)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsServiceBoughtOut}The Brotherhood has been bought out and dissolved, so it can no longer take the field as an independent mercenary clan.");
            }

            if (Clan.FindFirst(clan => clan.StringId == BrotherhoodOfWoodsClanId) == null)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsClanUnavailable}The Brotherhood of the Woods no longer exists.");
            }

            if (Clan.PlayerClan?.Kingdom == null)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsServiceNoKingdom}You must belong to a kingdom before the Brotherhood can enter its service.");
            }

            if (Hero.MainHero == null || Hero.MainHero.Gold < BrotherhoodOfWoodsServiceCost)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsRequiresGold}Requires 50000 denars.");
            }

            return new TextObject(
                "{=MVBrotherhoodOfWoodsServiceTooltip}The Brotherhood will serve your kingdom for sixty days. Costs 50000 denars.");
        }

        private static bool CanBuyOutBrotherhoodOfWoods()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            Clan brotherhood = Clan.FindFirst(clan => clan.StringId == BrotherhoodOfWoodsClanId);

            return progress != null && progress.IsHonoraryGuildmaster &&
                   !progress.IsBrotherhoodOfWoodsBoughtOut && brotherhood != null &&
                   Hero.MainHero != null && Hero.MainHero.Gold >= BrotherhoodOfWoodsBuyoutCost;
        }

        private static TextObject GetBrotherhoodOfWoodsBuyoutTooltip()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            if (progress == null || !progress.IsHonoraryGuildmaster)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsRequiresHonorary}Requires Honorary Guildmaster status.");
            }

            if (progress.IsBrotherhoodOfWoodsBoughtOut)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsAlreadyBoughtOut}The Brotherhood of the Woods has already been bought out.");
            }

            if (Clan.FindFirst(clan => clan.StringId == BrotherhoodOfWoodsClanId) == null)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsClanUnavailable}The Brotherhood of the Woods no longer exists.");
            }

            if (Hero.MainHero == null || Hero.MainHero.Gold < BrotherhoodOfWoodsBuyoutCost)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsBuyoutRequiresGold}Requires 1500000 denars.");
            }

            return new TextObject(
                "{=MVBrotherhoodOfWoodsBuyoutTooltip}Dissolves the Brotherhood and makes its warriors available for direct mobilization. Costs 1500000 denars.");
        }

        private bool CanMobilizeBoughtOutBrotherhoodOfWoods()
        {
            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;

            return progress != null && progress.IsHonoraryGuildmaster &&
                   progress.IsBrotherhoodOfWoodsBoughtOut && MobileParty.MainParty != null &&
                   CharacterObject.Find(BrotherhoodOfWoodsT1TroopId) != null &&
                   CharacterObject.Find(BrotherhoodOfWoodsT2TroopId) != null &&
                   CharacterObject.Find(BrotherhoodOfWoodsT3TroopId) != null &&
                   CharacterObject.Find(BrotherhoodOfWoodsT4TroopId) != null &&
                   _nextBrotherhoodOfWoodsMobilizationTime.IsPast;
        }

        private TextObject GetBrotherhoodOfWoodsMobilizationTooltip()
        {
            if (!_nextBrotherhoodOfWoodsMobilizationTime.IsPast)
            {
                return new TextObject(
                    "{=MVBrotherhoodOfWoodsMobilizationCooldown}The bought-out Brotherhood's warriors can only be called to arms once every sixty days.");
            }

            return new TextObject(
                "{=MVBrotherhoodOfWoodsMobilizationTooltip}Immediately receive 20 Sprouts, 25 Saplings, 25 Arboreal Brothers, and 15 Ancients. No party-size limit is applied. Available once every sixty days.");
        }

        private void MobilizeBoughtOutBrotherhoodOfWoods()
        {
            if (!CanMobilizeBoughtOutBrotherhoodOfWoods())
            {
                return;
            }

            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT1TroopId),
                BrotherhoodOfWoodsMobilizationT1Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT2TroopId),
                BrotherhoodOfWoodsMobilizationT2Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT3TroopId),
                BrotherhoodOfWoodsMobilizationT3Count);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                CharacterObject.Find(BrotherhoodOfWoodsT4TroopId),
                BrotherhoodOfWoodsMobilizationT4Count);
            _nextBrotherhoodOfWoodsMobilizationTime = CampaignTime.DaysFromNow(
                BrotherhoodOfWoodsMobilizationCooldownDays);

            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVBrotherhoodOfWoodsMobilizationSuccess}Eighty-five warriors of the bought-out Brotherhood of the Woods have answered your call.")
                    .ToString()));
            GameMenu.SwitchToMenu("town");
        }

        private static void DemandBrotherhoodOfWoodsService()
        {
            if (!CanDemandBrotherhoodOfWoodsService())
            {
                return;
            }

            Clan brotherhood = Clan.FindFirst(clan => clan.StringId == BrotherhoodOfWoodsClanId);
            Kingdom playerKingdom = Clan.PlayerClan?.Kingdom;
            CampaignTime contractEnd = CampaignTime.DaysFromNow(BrotherhoodOfWoodsServiceDays);

            if (brotherhood.Kingdom != null && brotherhood.Kingdom != playerKingdom)
            {
                ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(brotherhood, true);
            }

            if (brotherhood.Kingdom != playerKingdom)
            {
                ChangeKingdomAction.ApplyByJoinFactionAsMercenary(
                    brotherhood,
                    playerKingdom,
                    contractEnd,
                    1,
                    true);
            }
            else
            {
                brotherhood.ShouldStayInKingdomUntil = contractEnd;
                brotherhood.MercenaryAwardMultiplier = 1;
            }

            Hero.MainHero.ChangeHeroGold(-BrotherhoodOfWoodsServiceCost);
            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVBrotherhoodOfWoodsServiceSuccess}The Brotherhood of the Woods has entered your kingdom's service for sixty days.")
                    .ToString()));
            GameMenu.SwitchToMenu(BrotherhoodOfWoodsMenuId);
        }

        private static void BuyOutBrotherhoodOfWoods()
        {
            if (!CanBuyOutBrotherhoodOfWoods())
            {
                return;
            }

            WesternMercenaryGuildProgressBehavior progress =
                WesternMercenaryGuildProgressBehavior.Instance;
            Clan brotherhood = Clan.FindFirst(clan => clan.StringId == BrotherhoodOfWoodsClanId);

            Hero.MainHero.ChangeHeroGold(-BrotherhoodOfWoodsBuyoutCost);
            progress?.MarkBrotherhoodOfWoodsBoughtOut();
            DestroyClanAction.Apply(brotherhood);
            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject(
                        "{=MVBrotherhoodOfWoodsBuyoutSuccess}The Brotherhood of the Woods has been dissolved. Its members can now be called directly to your party.")
                    .ToString()));
            GameMenu.SwitchToMenu(BrotherhoodOfWoodsMenuId);
        }
    }
}
