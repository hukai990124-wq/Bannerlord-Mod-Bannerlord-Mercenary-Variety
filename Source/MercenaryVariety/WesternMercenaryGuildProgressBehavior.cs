using TaleWorlds.CampaignSystem;

namespace MercenaryVariety
{
    public class WesternMercenaryGuildProgressBehavior : CampaignBehaviorBase
    {
        public static WesternMercenaryGuildProgressBehavior Instance { get; private set; }

        private bool _isBasicMember;
        private bool _isAdvancedMember;
        private bool _isGuildSponsor;
        private bool _isHonoraryGuildmaster;
        private bool _isGoldenBoarBoughtOut;

        public bool IsBasicMember => _isBasicMember;
        public bool IsAdvancedMember => _isAdvancedMember;
        public bool IsGuildSponsor => _isGuildSponsor;
        public bool IsHonoraryGuildmaster => _isHonoraryGuildmaster;
        public bool IsGoldenBoarBoughtOut => _isGoldenBoarBoughtOut;

        public WesternMercenaryGuildProgressBehavior()
        {
            Instance = this;
        }

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(
                "mv_western_mercenary_guild_basic_membership",
                ref _isBasicMember);
            dataStore.SyncData(
                "mv_western_mercenary_guild_advanced_membership",
                ref _isAdvancedMember);
            dataStore.SyncData(
                "mv_western_mercenary_guild_sponsorship",
                ref _isGuildSponsor);
            dataStore.SyncData(
                "mv_western_mercenary_guild_honorary_guildmaster",
                ref _isHonoraryGuildmaster);
            dataStore.SyncData(
                "mv_western_mercenary_golden_boar_bought_out",
                ref _isGoldenBoarBoughtOut);
        }

        public void GrantBasicMembership()
        {
            _isBasicMember = true;
        }

        public void GrantAdvancedMembership()
        {
            _isBasicMember = true;
            _isAdvancedMember = true;
        }

        public void GrantGuildSponsorship()
        {
            _isBasicMember = true;
            _isAdvancedMember = true;
            _isGuildSponsor = true;
        }

        public void GrantHonoraryGuildmaster()
        {
            _isBasicMember = true;
            _isAdvancedMember = true;
            _isGuildSponsor = true;
            _isHonoraryGuildmaster = true;
        }

        public void MarkGoldenBoarBoughtOut()
        {
            _isGoldenBoarBoughtOut = true;
        }
    }
}
