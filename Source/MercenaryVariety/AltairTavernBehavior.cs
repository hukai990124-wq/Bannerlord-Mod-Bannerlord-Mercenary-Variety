using System.Collections.Generic;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MercenaryVariety
{
    public class AltairTavernBehavior : CampaignBehaviorBase
    {
        private const string AltairId = "mv_altair";
        private const string QuyazId = "town_A1";
        private const string TavernLocationId = "tavern";
        private const int IntelContactsPerRequest = 10;
        private const float IntelCooldownDays = 15f;
        private const float SneakNetworkDurationDays = 30f;
        private const int TrialRequiredEnemyDefeats = 50;
        private const string AssassinApprenticeTroopId = "mv_assassin_apprentice_t4";
        private const string DesertBanditTroopId = "desert_bandits_bandit";
        private const float CompanionRequestCooldownDays = 10f;
        private const int DesertBanditCount = 10;

        private bool _trialCompleted;
        private int _trialEnemyDefeats;
        private float _trialStartRenown;
        private CampaignTime _nextIntelRequestTime = CampaignTime.Zero;
        private CampaignTime _sneakNetworkUntil = CampaignTime.Zero;
        private CampaignTime _nextCompanionRequestTime = CampaignTime.Zero;
        private int _pendingCompanionOffer = -1;

        public static AltairTavernBehavior Instance { get; private set; }

        public int TrialEnemyDefeats => _trialEnemyDefeats;
        public bool IsTrialCompleted => _trialCompleted;
        public bool IsSneakNetworkActive => _sneakNetworkUntil.IsFuture;
        public int TrialRenownGained
        {
            get
            {
                if (Clan.PlayerClan == null)
                {
                    return 0;
                }

                int gained = (int)(Clan.PlayerClan.Renown - _trialStartRenown);
                return gained < 0 ? 0 : gained;
            }
        }

        public override void RegisterEvents()
        {
            Instance = this;
            CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(
                this,
                OnLocationCharactersAreReadyToSpawn);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("mv_altair_trial_completed", ref _trialCompleted);
            dataStore.SyncData("mv_altair_trial_enemy_defeats", ref _trialEnemyDefeats);
            dataStore.SyncData("mv_altair_trial_start_renown", ref _trialStartRenown);
            dataStore.SyncData("mv_altair_next_intel_request_time", ref _nextIntelRequestTime);
            dataStore.SyncData("mv_altair_sneak_network_until", ref _sneakNetworkUntil);
            dataStore.SyncData("mv_altair_next_companion_request_time", ref _nextCompanionRequestTime);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddDialogLine(
                "mv_altair_greeting",
                "start",
                "mv_altair_greeting",
                "{=MVAltairGreeting}Stranger, do I know you?",
                CanUseInitialGreeting,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_altair_completed_greeting",
                "start",
                "mv_altair_completed_greeting",
                "{=MVAltairCompletedGreeting}Most of the ignorant do not know of our existence, yet you chose a dangerous contact.",
                CanUseCompletedGreeting,
                null,
                210);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_shadow_question",
                "mv_altair_greeting",
                "mv_altair_shadow_reply",
                "{=MVAltairShadowQuestion}Not all matters can be buried in shadow.",
                CanDiscussInitiation,
                null,
                200);

            AddLeaveLine(campaignGameStarter, "mv_altair_greeting", "mv_altair_leave_greeting");
            AddLeaveLine(campaignGameStarter, "mv_altair_completed_greeting", "mv_altair_leave_completed_greeting");

            campaignGameStarter.AddPlayerLine(
                "mv_altair_request_dark_power",
                "mv_altair_completed_greeting",
                "mv_altair_dark_power_reply",
                "{=MVAltairRequestDarkPower}I need power from the darkness.",
                CanUseCompletedGreeting,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_altair_dark_power_reply",
                "mv_altair_dark_power_reply",
                "mv_altair_apprentice_services",
                "{=MVAltairDarkPowerReply}What do you require?",
                null,
                null,
                200);

            AddLeaveLine(campaignGameStarter, "mv_altair_apprentice_services", "mv_altair_leave_apprentice_services");

            campaignGameStarter.AddPlayerLine(
                "mv_altair_request_intel",
                "mv_altair_apprentice_services",
                "mv_altair_intel_reply",
                "{=MVAltairRequestIntel}I want to know about those important people.",
                CanRequestIntel,
                RequestIntel,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_altair_intel_reply",
                "mv_altair_intel_reply",
                "close_window",
                "{=MVAltairIntelReply}Those who stand in the open always make their information easy to find.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_request_sneak_network",
                "mv_altair_apprentice_services",
                "mv_altair_sneak_network_reply",
                "{=MVAltairRequestSneakNetwork}I hope the Assassin brothers and sisters will receive me in the cities.",
                CanRequestSneakNetwork,
                RequestSneakNetwork,
                190);

            campaignGameStarter.AddDialogLine(
                "mv_altair_sneak_network_reply",
                "mv_altair_sneak_network_reply",
                "close_window",
                "{=MVAltairSneakNetworkReply}I have sent word. The comrades in shadow will show you the road into the cities.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_request_companions",
                "mv_altair_apprentice_services",
                "mv_altair_companion_offer",
                "{=MVAltairRequestCompanions}I would welcome more companions to join me.",
                CanRequestCompanions,
                RequestCompanions,
                185);

            campaignGameStarter.AddDialogLine(
                "mv_altair_companion_offer_apprentice",
                "mv_altair_companion_offer",
                "mv_altair_companion_choice",
                "{=MVAltairCompanionOfferApprentice}Few of the Assassins would bind themselves to a single master, yet one apprentice seeks a place to prove himself.",
                IsApprenticeOffer,
                null,
                200);

            campaignGameStarter.AddDialogLine(
                "mv_altair_companion_offer_bandit",
                "mv_altair_companion_offer",
                "mv_altair_companion_choice",
                "{=MVAltairCompanionOfferBandit}Heh. There are those who have taken to banditry and would be glad of a full belly. Would you take them in?",
                IsBanditOffer,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_accept_companions",
                "mv_altair_companion_choice",
                "close_window",
                "{=MVAltairAcceptCompanions}I accept.",
                CanAcceptCompanions,
                AcceptCompanions,
                200);

            AddLeaveLine(campaignGameStarter, "mv_altair_companion_choice", "mv_altair_leave_companion_choice");

            campaignGameStarter.AddDialogLine(
                "mv_altair_shadow_reply",
                "mv_altair_shadow_reply",
                "mv_altair_origins",
                "{=MVAltairShadowReply}Indeed, but people often see only where the light falls. You seem able to perceive what lies deep in shadow.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_origins",
                "mv_altair_origins",
                "mv_altair_understanding",
                "{=MVAltairOrigins}A different upbringing reveals what others cannot see.",
                null,
                null,
                200);

            AddLeaveLine(campaignGameStarter, "mv_altair_origins", "mv_altair_leave_origins");

            campaignGameStarter.AddDialogLine(
                "mv_altair_understanding",
                "mv_altair_understanding",
                "mv_altair_choice",
                "{=MVAltairUnderstanding}Then you understand: seeing us and knowing us are not the same thing.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_join_shadows",
                "mv_altair_choice",
                "mv_altair_prove",
                "{=MVAltairJoinShadows}I am willing to devote myself to the shadows.",
                null,
                null,
                200);

            AddLeaveLine(campaignGameStarter, "mv_altair_choice", "mv_altair_leave_choice");

            campaignGameStarter.AddDialogLine(
                "mv_altair_prove",
                "mv_altair_prove",
                "mv_altair_trial_accept",
                "{=MVAltairProve}We do not speak with the ordinary. You must prove yourself.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_accept_trial",
                "mv_altair_trial_accept",
                "mv_altair_trial_begun",
                "{=MVAltairAcceptTrial}I accept.",
                CanStartTrial,
                StartTrial,
                200);

            AddLeaveLine(campaignGameStarter, "mv_altair_trial_accept", "mv_altair_leave_trial_accept");

            campaignGameStarter.AddDialogLine(
                "mv_altair_trial_begun",
                "mv_altair_trial_begun",
                "close_window",
                "{=MVAltairTrialBegun}Even in the shadows, God watches over all things.",
                null,
                null,
                200);

            campaignGameStarter.AddPlayerLine(
                "mv_altair_turn_in_trial",
                "mv_altair_greeting",
                "mv_altair_trial_complete",
                "{=MVAltairTurnInTrial}With the sword in my hand, I have proven my strength.",
                CanTurnInTrial,
                CompleteTrial,
                210);

            campaignGameStarter.AddDialogLine(
                "mv_altair_trial_complete",
                "mv_altair_trial_complete",
                "close_window",
                "{=MVAltairTrialComplete}God has witnessed all that you have done. We have heard of someone's recent affairs.",
                null,
                null,
                210);
        }

        public void MarkTrialCompleted()
        {
            _trialCompleted = true;
        }

        public void BeginTrial()
        {
            _trialEnemyDefeats = 0;
            _trialStartRenown = Clan.PlayerClan != null ? Clan.PlayerClan.Renown : 0f;
        }

        public void RecordEnemyDefeats(int count)
        {
            if (count <= 0)
            {
                return;
            }

            _trialEnemyDefeats += count;
            if (_trialEnemyDefeats > TrialRequiredEnemyDefeats)
            {
                _trialEnemyDefeats = TrialRequiredEnemyDefeats;
            }
        }

        private static bool CanRequestIntel()
        {
            return IsTalkingToAltair() &&
                   Instance != null &&
                   Instance._trialCompleted &&
                   Instance._nextIntelRequestTime.IsPast &&
                   GetUnknownHeroes().Count > 0;
        }

        private static void RequestIntel()
        {
            if (!CanRequestIntel())
            {
                return;
            }

            List<Hero> unknownHeroes = GetUnknownHeroes();
            int contactsToReveal = unknownHeroes.Count;
            if (contactsToReveal > IntelContactsPerRequest)
            {
                contactsToReveal = IntelContactsPerRequest;
            }

            for (int i = 0; i < contactsToReveal; i++)
            {
                int index = MBRandom.RandomInt(unknownHeroes.Count);
                Hero hero = unknownHeroes[index];
                hero.SetHasMet();
                unknownHeroes.RemoveAt(index);
            }

            Instance._nextIntelRequestTime = CampaignTime.DaysFromNow(IntelCooldownDays);
        }

        private static bool CanRequestSneakNetwork()
        {
            return IsTalkingToAltair() &&
                   Instance != null &&
                   Instance._trialCompleted &&
                   !Instance._sneakNetworkUntil.IsFuture;
        }

        private static void RequestSneakNetwork()
        {
            if (!CanRequestSneakNetwork())
            {
                return;
            }

            Instance._sneakNetworkUntil = CampaignTime.DaysFromNow(SneakNetworkDurationDays);
            InformationManager.DisplayMessage(
                new InformationMessage(
                    new TextObject("{=MVAltairSneakNetworkActivated}The comrades in shadow will receive you for thirty days.").ToString()));
        }

        private static bool CanRequestCompanions()
        {
            return IsTalkingToAltair() &&
                   Instance != null &&
                   Instance._trialCompleted &&
                   Instance._nextCompanionRequestTime.IsPast;
        }

        private static void RequestCompanions()
        {
            if (!CanRequestCompanions())
            {
                return;
            }

            Instance._pendingCompanionOffer = MBRandom.RandomInt(2);
            Instance._nextCompanionRequestTime = CampaignTime.DaysFromNow(CompanionRequestCooldownDays);
        }

        private static bool IsApprenticeOffer()
        {
            return Instance != null && Instance._pendingCompanionOffer == 0;
        }

        private static bool IsBanditOffer()
        {
            return Instance != null && Instance._pendingCompanionOffer == 1;
        }

        private static bool CanAcceptCompanions()
        {
            return Instance != null && Instance._pendingCompanionOffer >= 0;
        }

        private static void AcceptCompanions()
        {
            if (!CanAcceptCompanions())
            {
                return;
            }

            if (Instance._pendingCompanionOffer == 0)
            {
                CharacterObject apprentice = CharacterObject.Find(AssassinApprenticeTroopId);
                if (apprentice != null)
                {
                    MobileParty.MainParty.MemberRoster.AddToCounts(apprentice, 1, false, 0, 0, false, 0);
                }

                InformationManager.DisplayMessage(
                    new InformationMessage(
                        new TextObject("{=MVAltairApprenticeJoined}An Assassin apprentice has joined your company.").ToString()));
            }
            else
            {
                CharacterObject bandit = CharacterObject.Find(DesertBanditTroopId);
                if (bandit != null)
                {
                    MobileParty.MainParty.MemberRoster.AddToCounts(bandit, DesertBanditCount, false, 0, 0, false, 0);
                }

                InformationManager.DisplayMessage(
                    new InformationMessage(
                        new TextObject("{=MVAltairBanditsJoined}Ten desert bandits have joined your company.").ToString()));
            }

            Instance._pendingCompanionOffer = -1;
        }

        private static List<Hero> GetUnknownHeroes()
        {
            List<Hero> unknownHeroes = new List<Hero>();
            foreach (Hero hero in Hero.AllAliveHeroes)
            {
                if (hero != null && hero != Hero.MainHero && !hero.HasMet)
                {
                    unknownHeroes.Add(hero);
                }
            }

            return unknownHeroes;
        }

        private static bool IsTalkingToAltair()
        {
            CharacterObject character = CharacterObject.OneToOneConversationCharacter;
            return character != null && character.StringId == AltairId;
        }

        private static bool CanUseInitialGreeting()
        {
            return IsTalkingToAltair() &&
                   (Instance == null || !Instance._trialCompleted);
        }

        private static bool CanUseCompletedGreeting()
        {
            return IsTalkingToAltair() &&
                   Instance != null &&
                   Instance._trialCompleted;
        }

        private static bool CanDiscussInitiation()
        {
            return IsTalkingToAltair() &&
                   Instance != null &&
                   !Instance._trialCompleted &&
                   FindActiveTrial() == null;
        }

        private static bool CanStartTrial()
        {
            return IsTalkingToAltair() &&
                   Instance != null &&
                   !Instance._trialCompleted &&
                   FindActiveTrial() == null;
        }

        private static bool CanTurnInTrial()
        {
            AltairTrialQuest quest = FindActiveTrial();
            return IsTalkingToAltair() && quest != null && quest.IsReadyToTurnIn;
        }

        private static void StartTrial()
        {
            if (CanStartTrial())
            {
                // QuestBase requires a Hero quest giver; Altair remains a fixed non-hero location character.
                Instance.BeginTrial();
                new AltairTrialQuest(Hero.MainHero).StartQuest();
            }
        }

        private static void CompleteTrial()
        {
            AltairTrialQuest quest = FindActiveTrial();
            if (quest != null && quest.IsReadyToTurnIn)
            {
                quest.CompleteQuestWithSuccess();
            }
        }

        private static AltairTrialQuest FindActiveTrial()
        {
            if (Campaign.Current?.QuestManager == null)
            {
                return null;
            }

            foreach (QuestBase quest in Campaign.Current.QuestManager.Quests)
            {
                AltairTrialQuest trial = quest as AltairTrialQuest;
                if (trial != null)
                {
                    return trial;
                }
            }

            return null;
        }

        private static void AddLeaveLine(
            CampaignGameStarter campaignGameStarter,
            string inputToken,
            string lineId)
        {
            campaignGameStarter.AddPlayerLine(
                lineId,
                inputToken,
                "close_window",
                "{=MVAltairLeave}Leave.",
                IsTalkingToAltair,
                null,
                100);
        }

        private static void OnLocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
        {
            Settlement settlement = Settlement.CurrentSettlement;
            if (settlement == null || settlement.StringId != QuyazId)
            {
                return;
            }

            Location location = settlement.LocationComplex?.GetLocationWithId(TavernLocationId);
            if (location == null)
            {
                return;
            }

            foreach (LocationCharacter locationCharacter in location.GetCharacterList())
            {
                if (locationCharacter.Character?.StringId == AltairId)
                {
                    return;
                }
            }

            CharacterObject altair = CharacterObject.Find(AltairId);
            if (altair == null)
            {
                return;
            }

            AgentData agentData = new AgentData(new SimpleAgentOrigin(altair));
            LocationCharacter altairLocationCharacter = new LocationCharacter(
                agentData,
                SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors,
                null,
                true,
                LocationCharacter.CharacterRelations.Neutral,
                null,
                false,
                true);

            location.AddCharacter(altairLocationCharacter);
        }
    }
}
