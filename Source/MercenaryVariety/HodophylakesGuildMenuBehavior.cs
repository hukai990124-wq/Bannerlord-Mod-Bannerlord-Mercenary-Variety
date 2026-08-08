using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MercenaryVariety
{
    public class HodophylakesGuildMenuBehavior : CampaignBehaviorBase
    {
        private const string DanusticaId = "town_ES1";
        private const string GuildMenuId = "mv_hodophylakes_guild_placeholder";
        private const string SwordSisterRecruitmentMenuId = "mv_hodophylakes_sword_sister_recruitment";
        private const string T4SwordSisterRecruitmentMenuId = "mv_hodophylakes_t4_sword_sister_recruitment";
        private const string T5SwordSisterRecruitmentMenuId = "mv_hodophylakes_t5_sword_sister_recruitment";
        private const string T6SwordSisterRecruitmentMenuId = "mv_hodophylakes_t6_sword_sister_recruitment";
        private const string SwordSisterTroopId = "sword_sisters_sister_t3";
        private const string T4SwordSisterTroopId = "sword_sisters_sister_t4";
        private const string T5CrossbowTroopId = "sword_sisters_sister_infantry_t5";
        private const string T6CrossbowTroopId = "mv_sisterhood_arbalest_t6";
        private const float SwordSisterRecruitmentCooldownDays = 7f;
        private CampaignTime _nextT3SwordSisterRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextT4SwordSisterRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextT5SwordSisterRecruitmentTime = CampaignTime.Zero;
        private CampaignTime _nextT6SwordSisterRecruitmentTime = CampaignTime.Zero;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(
                this,
                OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(
                "mv_hodophylakes_next_t3_sword_sister_recruitment_time",
                ref _nextT3SwordSisterRecruitmentTime);
            dataStore.SyncData(
                "mv_hodophylakes_next_t4_sword_sister_recruitment_time",
                ref _nextT4SwordSisterRecruitmentTime);
            dataStore.SyncData(
                "mv_hodophylakes_next_t5_sword_sister_recruitment_time",
                ref _nextT5SwordSisterRecruitmentTime);
            dataStore.SyncData(
                "mv_hodophylakes_next_t6_sword_sister_recruitment_time",
                ref _nextT6SwordSisterRecruitmentTime);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                "town",
                "mv_hodophylakes_guild_entry",
                "{=MVHodophylakesGuildEntry}Enter the Hodophylakes Guild",
                args =>
                {
                    if (!IsDanustica())
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(GuildMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GuildMenuId,
                "mv_hodophylakes_recruit_t4_sword_sister_followers",
                "{=MVHodophylakesRecruitT4SisterFollowers}Recruit T4 Sword Sisters Followers",
                args =>
                {
                    HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
                    if (progress == null || !progress.IsT4SwordSisterRecruitmentUnlocked)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(T4SwordSisterRecruitmentMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                GuildMenuId,
                "{=MVHodophylakesGuildMenu}You have arrived at the Hodophylakes Guild of Danustica. Since the Empire fractured and fell into civil war, the nobles have had little time to concern themselves with the safety of ordinary people. In response, some citizens formed the Hodophylakes Patrols to protect travelers on the roads. You are now standing in the Danustica guildhall, a gathering place for honorable men and women drawn here by the city's position at the heart of the Imperial trade routes.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                GuildMenuId,
                "mv_hodophylakes_recruit_sword_sister_followers",
                "{=MVHodophylakesRecruitSisterFollowers}Recruit Sword Sisters Followers",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(SwordSisterRecruitmentMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GuildMenuId,
                "mv_hodophylakes_recruit_t5_crossbow_sword_sisters",
                "{=MVHodophylakesRecruitT5CrossbowSisterFollowers}Recruit T5 Sword Sister Crossbowmen",
                args =>
                {
                    HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
                    if (progress == null || !progress.IsT5CrossbowRecruitmentUnlocked)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(T5SwordSisterRecruitmentMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GuildMenuId,
                "mv_hodophylakes_recruit_t6_crossbow_sword_sisters",
                "{=MVHodophylakesRecruitT6CrossbowSisterFollowers}Recruit T6 Sisterhood Arbalests",
                args =>
                {
                    HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
                    if (progress == null || !progress.IsT6CrossbowRecruitmentUnlocked)
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(T6SwordSisterRecruitmentMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                SwordSisterRecruitmentMenuId,
                "{=MVHodophylakesSwordSisterRecruitmentMenu}The guild can provide Sword Sisters Followers for immediate service. Choose the size of the group you wish to recruit.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenu(
                T4SwordSisterRecruitmentMenuId,
                "{=MVHodophylakesT4SwordSisterRecruitmentMenu}The guild can provide experienced T4 Sword Sisters Followers for immediate service. Choose the size of the group you wish to recruit.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenu(
                T5SwordSisterRecruitmentMenuId,
                "{=MVHodophylakesT5SwordSisterRecruitmentMenu}The guild can provide veteran T5 Sword Sister Crossbowmen for immediate service. The mounted branch is not available through the guild.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenu(
                T6SwordSisterRecruitmentMenuId,
                "{=MVHodophylakesT6SwordSisterRecruitmentMenu}The guild can provide elite T6 Sisterhood Arbalests for immediate service. The mounted branch is not available through the guild.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_sword_sisters_10",
                "{=MVHodophylakesRecruitSisters10}Recruit 10 T3 Sword Sisters (1200 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitSwordSisters(10, 1200);
                    return true;
                },
                args => RecruitSwordSisters(10, 1200),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T4SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t4_sword_sisters_10",
                "{=MVHodophylakesRecruitT4Sisters10}Recruit 10 T4 Sword Sisters (1800 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT4SwordSisters(10, 1800);
                    return true;
                },
                args => RecruitT4SwordSisters(10, 1800),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T4SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t4_sword_sisters_20",
                "{=MVHodophylakesRecruitT4Sisters20}Recruit 20 T4 Sword Sisters (3000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT4SwordSisters(20, 3000);
                    return true;
                },
                args => RecruitT4SwordSisters(20, 3000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T4SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t4_sword_sisters_30",
                "{=MVHodophylakesRecruitT4Sisters30}Recruit 30 T4 Sword Sisters (4000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT4SwordSisters(30, 4000);
                    return true;
                },
                args => RecruitT4SwordSisters(30, 4000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T4SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_t4_sword_sister_recruitment_back",
                "{=MVHodophylakesT4SwordSisterRecruitmentBack}Back",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(GuildMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T5SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t5_crossbow_sword_sisters_10",
                "{=MVHodophylakesRecruitT5CrossbowSisters10}Recruit 10 T5 Sword Sister Crossbowmen (3000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT5CrossbowSisters(10, 3000);
                    return true;
                },
                args => RecruitT5CrossbowSisters(10, 3000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T5SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t5_crossbow_sword_sisters_20",
                "{=MVHodophylakesRecruitT5CrossbowSisters20}Recruit 20 T5 Sword Sister Crossbowmen (5000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT5CrossbowSisters(20, 5000);
                    return true;
                },
                args => RecruitT5CrossbowSisters(20, 5000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T5SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t5_crossbow_sword_sisters_30",
                "{=MVHodophylakesRecruitT5CrossbowSisters30}Recruit 30 T5 Sword Sister Crossbowmen (7000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT5CrossbowSisters(30, 7000);
                    return true;
                },
                args => RecruitT5CrossbowSisters(30, 7000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T5SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_t5_sword_sister_recruitment_back",
                "{=MVHodophylakesT5SwordSisterRecruitmentBack}Back",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(GuildMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T6SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t6_crossbow_sword_sisters_10",
                "{=MVHodophylakesRecruitT6CrossbowSisters10}Recruit 10 T6 Sisterhood Arbalests (5000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT6CrossbowSisters(10, 5000);
                    return true;
                },
                args => RecruitT6CrossbowSisters(10, 5000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T6SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t6_crossbow_sword_sisters_20",
                "{=MVHodophylakesRecruitT6CrossbowSisters20}Recruit 20 T6 Sisterhood Arbalests (9000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT6CrossbowSisters(20, 9000);
                    return true;
                },
                args => RecruitT6CrossbowSisters(20, 9000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T6SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_t6_crossbow_sword_sisters_30",
                "{=MVHodophylakesRecruitT6CrossbowSisters30}Recruit 30 T6 Sisterhood Arbalests (13000 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitT6CrossbowSisters(30, 13000);
                    return true;
                },
                args => RecruitT6CrossbowSisters(30, 13000),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                T6SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_t6_sword_sister_recruitment_back",
                "{=MVHodophylakesT6SwordSisterRecruitmentBack}Back",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(GuildMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_sword_sisters_20",
                "{=MVHodophylakesRecruitSisters20}Recruit 20 T3 Sword Sisters (2400 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitSwordSisters(20, 2400);
                    return true;
                },
                args => RecruitSwordSisters(20, 2400),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_recruit_sword_sisters_30",
                "{=MVHodophylakesRecruitSisters30}Recruit 30 T3 Sword Sisters (3600 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitSwordSisters(30, 3600);
                    return true;
                },
                args => RecruitSwordSisters(30, 3600),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                SwordSisterRecruitmentMenuId,
                "mv_hodophylakes_sword_sister_recruitment_back",
                "{=MVHodophylakesSwordSisterRecruitmentBack}Back",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(GuildMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                GuildMenuId,
                "mv_hodophylakes_guild_leave",
                "{=MVHodophylakesGuildLeave}Leave",
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
        }

        private static bool IsDanustica()
        {
            Settlement settlement = Settlement.CurrentSettlement;
            return settlement != null && settlement.StringId == DanusticaId;
        }

        private bool CanRecruitSwordSisters(int count, int cost)
        {
            return CanRecruitTroops(
                SwordSisterTroopId,
                count,
                cost,
                _nextT3SwordSisterRecruitmentTime);
        }

        private bool CanRecruitT4SwordSisters(int count, int cost)
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return progress != null &&
                   progress.IsT4SwordSisterRecruitmentUnlocked &&
                   CanRecruitTroops(
                       T4SwordSisterTroopId,
                       count,
                       cost,
                       _nextT4SwordSisterRecruitmentTime);
        }

        private bool CanRecruitT5CrossbowSisters(int count, int cost)
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return progress != null &&
                   progress.IsT5CrossbowRecruitmentUnlocked &&
                   CanRecruitTroops(
                       T5CrossbowTroopId,
                       count,
                       cost,
                       _nextT5SwordSisterRecruitmentTime);
        }

        private bool CanRecruitT6CrossbowSisters(int count, int cost)
        {
            HodophylakesProgressBehavior progress = HodophylakesProgressBehavior.Instance;
            return progress != null &&
                   progress.IsT6CrossbowRecruitmentUnlocked &&
                   CanRecruitTroops(
                       T6CrossbowTroopId,
                       count,
                       cost,
                       _nextT6SwordSisterRecruitmentTime);
        }

        private void RecruitSwordSisters(int count, int cost)
        {
            RecruitTroops(
                SwordSisterTroopId,
                count,
                cost,
                ref _nextT3SwordSisterRecruitmentTime);
        }

        private void RecruitT4SwordSisters(int count, int cost)
        {
            RecruitTroops(
                T4SwordSisterTroopId,
                count,
                cost,
                ref _nextT4SwordSisterRecruitmentTime);
        }

        private void RecruitT5CrossbowSisters(int count, int cost)
        {
            RecruitTroops(
                T5CrossbowTroopId,
                count,
                cost,
                ref _nextT5SwordSisterRecruitmentTime);
        }

        private void RecruitT6CrossbowSisters(int count, int cost)
        {
            RecruitTroops(
                T6CrossbowTroopId,
                count,
                cost,
                ref _nextT6SwordSisterRecruitmentTime);
        }

        private bool CanRecruitTroops(
            string troopId,
            int count,
            int cost,
            CampaignTime nextRecruitmentTime)
        {
            MobileParty party = MobileParty.MainParty;
            CharacterObject troop = CharacterObject.Find(troopId);

            return party != null && troop != null &&
                   nextRecruitmentTime.IsPast &&
                   Hero.MainHero.Gold >= cost &&
                   party.Party.NumberOfAllMembers + count <= party.Party.PartySizeLimit;
        }

        private void RecruitTroops(
            string troopId,
            int count,
            int cost,
            ref CampaignTime nextRecruitmentTime)
        {
            if (!CanRecruitTroops(troopId, count, cost, nextRecruitmentTime))
            {
                return;
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

            nextRecruitmentTime = CampaignTime.DaysFromNow(SwordSisterRecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }
    }
}
