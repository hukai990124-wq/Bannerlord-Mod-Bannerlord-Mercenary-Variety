using System;
using System.Collections.Generic;
using System.Linq;
using SandBox;
using SandBox.Missions.MissionLogics;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace MercenaryVariety
{
    internal sealed class AltairMartialTrialMission : MissionLogic
    {
        private readonly AltairMartialTrialQuest _quest;
        private readonly CharacterObject _apprentice;
        private readonly HashSet<Agent> _opponents = new HashSet<Agent>();
        private readonly HashSet<Agent> _defeated = new HashSet<Agent>();
        private Agent _player;
        private bool _started;
        private bool _interrupted;
        private bool _finished;
        private bool _won;
        private bool _reported;
        private float _endDelay;
        private readonly int _previousHitPoints;

        private AltairMartialTrialMission(AltairMartialTrialQuest quest, CharacterObject apprentice)
        {
            _quest = quest;
            _apprentice = apprentice;
            _previousHitPoints = Hero.MainHero.HitPoints;
        }

        internal static Mission Open(string scene, Location arena, AltairMartialTrialQuest quest)
        {
            CharacterObject apprentice = CharacterObject.Find(AltairMartialTrialBehavior.ApprenticeId);
            if (apprentice == null)
                throw new InvalidOperationException("Assassin Apprentice is missing.");
            // Reuse the native duel's views/support behaviors, but not its one-opponent win logic.
            return MissionState.OpenNew("ArenaDuelMission",
                SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, DecalAtlasGroup.Town),
                mission => new MissionBehavior[]
                {
                    new MissionOptionsComponent(),
                    new MissionFacialAnimationHandler(),
                    new MissionAgentPanicHandler(),
                    new AgentHumanAILogic(),
                    new ArenaAgentStateDeciderLogic(),
                    new VisualTrackerMissionBehavior(),
                    new CampaignMissionComponent(),
                    new EquipmentControllerLeaveLogic(),
                    new MissionAgentHandler(),
                    new MissionLocationLogic(arena),
                    new AltairMartialTrialMission(quest, apprentice)
                });
        }

        public override void AfterStart()
        {
            try
            {
                var tournamentSet = Mission.Scene.FindEntityWithTag("tournament_fight");
                if (tournamentSet != null)
                    TournamentBehavior.DeleteTournamentSetsExcept(tournamentSet);
                List<MatrixFrame> frames = Mission.Scene.FindEntitiesWithTag("sp_arena").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count < 3)
                    frames = Mission.Scene.FindEntitiesWithTag("sp_arena_respawn").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count < 3)
                    throw new InvalidOperationException("Arena requires three spawn points.");

                Mission.Teams.Add(BattleSideEnum.Defender, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2);
                Mission.Teams.Add(BattleSideEnum.Attacker, _apprentice.Culture.Color, _apprentice.Culture.Color2);
                Mission.PlayerTeam = Mission.Teams.Defender;

                MatrixFrame playerFrame = frames[MBRandom.RandomInt(frames.Count)];
                frames.Remove(playerFrame);
                // Use separate arena markers, placing the opponents away from the player.
                frames = frames.OrderByDescending(f => f.origin.DistanceSquared(playerFrame.origin)).ToList();
                _player = Spawn(CharacterObject.PlayerCharacter, Mission.PlayerTeam, playerFrame, frames[0].origin, true);
                _opponents.Add(Spawn(_apprentice, Mission.PlayerEnemyTeam, frames[0], playerFrame.origin, false));
                _opponents.Add(Spawn(_apprentice, Mission.PlayerEnemyTeam, frames[1], playerFrame.origin, false));
                _started = true;
            }
            catch (Exception ex)
            {
                Debug.Print("[MercenaryVariety] Martial trial setup failed: " + ex);
                _interrupted = true;
                Finish(false, "{=MVAltairMartialUnavailable}The trial could not begin. Your quest is unchanged; speak to Altair to try again.");
            }
        }

        private Agent Spawn(CharacterObject character, Team team, MatrixFrame frame, Vec3 facing, bool player)
        {
            Vec2 direction = (facing - frame.origin).AsVec2.Normalized();
            Agent agent = Mission.SpawnAgent(new AgentBuildData(character)
                .BodyProperties(character.GetBodyPropertiesMax())
                .Team(team).InitialPosition(in frame.origin).InitialDirection(direction)
                .NoHorses(true).Equipment(character.FirstBattleEquipment)
                .TroopOrigin(new SimpleAgentOrigin(character))
                .Controller(player ? AgentControllerType.Player : AgentControllerType.AI));
            agent.Health = agent.HealthLimit;
            if (!player)
                agent.SetWatchState(Agent.WatchState.Alarmed);
            agent.FadeIn();
            return agent;
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
        {
            if (_finished || !_opponents.Contains(affectedAgent))
                return;
            if (agentState == AgentState.Unconscious || agentState == AgentState.Killed)
                _defeated.Add(affectedAgent);
            else
                _interrupted = true;
        }

        internal static bool? EvaluateOutcome(bool playerStanding, int defeatedCount, bool interrupted)
        {
            if (interrupted || !playerStanding)
                return false;
            return defeatedCount == 2 ? (bool?)true : null;
        }

        public override void OnMissionTick(float dt)
        {
            if (_finished)
            {
                _endDelay += dt;
                if (_endDelay >= 3f)
                    Mission.EndMission();
                return;
            }
            if (!_started)
                return;
            bool? outcome = EvaluateOutcome(_player != null && _player.IsActive() && _player.Health > 0f, _defeated.Count, _interrupted);
            if (outcome.HasValue)
                Finish(outcome.Value, outcome.Value
                    ? "{=MVAltairMartialWon}You defeated both apprentices. Return to Altair to report your success."
                    : "{=MVAltairMartialLost}You did not pass this trial. Speak to Altair when you are ready to try again.");
        }

        private void Finish(bool won, string message)
        {
            _finished = true;
            _won = won;
            InformationManager.DisplayMessage(new InformationMessage(new TextObject(message).ToString()));
        }

        public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
        {
            canPlayerLeave = true;
            if (!_finished)
            {
                _interrupted = true;
                Finish(false, "{=MVAltairMartialLost}You did not pass this trial. Speak to Altair when you are ready to try again.");
            }
            return null;
        }

        protected override void OnEndMission()
        {
            if (_reported)
                return;
            _reported = true;
            Hero.MainHero.HitPoints = _previousHitPoints;
            if (_started && _finished && _won && !_interrupted)
                _quest.RecordVictory();
        }
    }
}
