using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace MercenaryVariety
{
    public class VaegirShelterMenuBehavior : CampaignBehaviorBase
    {
        private const string DiathmaId = "town_EN2";
        private const string ShelterMenuId = "mv_vaegir_shelter";
        private const string RecruitmentMenuId = "mv_vaegir_shelter_recruitment";
        private const string VaegirTroopId = "mv_old_vaegir_warrior";
        private const float VaegirRecruitmentCooldownDays = 7f;
        private CampaignTime _nextVaegirRecruitmentTime = CampaignTime.Zero;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(
                this,
                OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(
                "mv_vaegir_shelter_next_recruitment_time",
                ref _nextVaegirRecruitmentTime);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                "town",
                "mv_vaegir_shelter_entry",
                "{=MVVaegirShelterEntry}Enter the Vaegir Shelter",
                args =>
                {
                    if (!IsDiathma())
                    {
                        return false;
                    }

                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(ShelterMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                ShelterMenuId,
                "{=MVVaegirShelterMenu}You have arrived at the Vaegir Shelter in Diathma. Once scattered across the northern frontier, Vaegir veterans, travelers, and displaced families have gathered here under one roof. The shelter offers protection, fellowship, and a place for those who still remember the old northern traditions.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                ShelterMenuId,
                "mv_vaegir_shelter_recruit_vaegir",
                "{=MVVaegirShelterRecruitVaegir}Recruit T3 Vaegir Warriors",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(RecruitmentMenuId),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenu(
                RecruitmentMenuId,
                "{=MVVaegirShelterRecruitmentMenu}The shelter can provide T3 Vaegir Warriors for immediate service. Choose the size of the group you wish to recruit.",
                args => { },
                GameMenu.MenuOverlayType.None,
                GameMenu.MenuFlags.None,
                null);

            campaignGameStarter.AddGameMenuOption(
                RecruitmentMenuId,
                "mv_vaegir_shelter_recruit_vaegir_10",
                "{=MVVaegirShelterRecruitVaegir10}Recruit 10 T3 Vaegir Warriors (1200 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitVaegir(10, 1200);
                    return true;
                },
                args => RecruitVaegir(10, 1200),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                RecruitmentMenuId,
                "mv_vaegir_shelter_recruit_vaegir_20",
                "{=MVVaegirShelterRecruitVaegir20}Recruit 20 T3 Vaegir Warriors (2400 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitVaegir(20, 2400);
                    return true;
                },
                args => RecruitVaegir(20, 2400),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                RecruitmentMenuId,
                "mv_vaegir_shelter_recruit_vaegir_30",
                "{=MVVaegirShelterRecruitVaegir30}Recruit 30 T3 Vaegir Warriors (3600 denars)",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    args.IsEnabled = CanRecruitVaegir(30, 3600);
                    return true;
                },
                args => RecruitVaegir(30, 3600),
                false,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                RecruitmentMenuId,
                "mv_vaegir_shelter_recruitment_back",
                "{=MVVaegirShelterRecruitmentBack}Back",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                args => GameMenu.SwitchToMenu(ShelterMenuId),
                true,
                -1,
                false,
                null);

            campaignGameStarter.AddGameMenuOption(
                ShelterMenuId,
                "mv_vaegir_shelter_leave",
                "{=MVVaegirShelterLeave}Leave",
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

        private static bool IsDiathma()
        {
            Settlement settlement = Settlement.CurrentSettlement;
            return settlement != null && settlement.StringId == DiathmaId;
        }

        private bool CanRecruitVaegir(int count, int cost)
        {
            MobileParty party = MobileParty.MainParty;
            CharacterObject troop = CharacterObject.Find(VaegirTroopId);

            return party != null && troop != null &&
                   _nextVaegirRecruitmentTime.IsPast &&
                   Hero.MainHero.Gold >= cost &&
                   party.Party.NumberOfAllMembers + count <= party.Party.PartySizeLimit;
        }

        private void RecruitVaegir(int count, int cost)
        {
            if (!CanRecruitVaegir(count, cost))
            {
                return;
            }

            CharacterObject troop = CharacterObject.Find(VaegirTroopId);
            Hero.MainHero.ChangeHeroGold(-cost);
            MobileParty.MainParty.MemberRoster.AddToCounts(
                troop,
                count,
                false,
                0,
                0,
                false,
                0);

            _nextVaegirRecruitmentTime = CampaignTime.DaysFromNow(VaegirRecruitmentCooldownDays);
            GameMenu.SwitchToMenu("town");
        }
    }
}
