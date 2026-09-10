using System;
using System.Collections.Generic;
using System.Linq;
using SandBox;
using SandBox.Missions.MissionLogics;
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
    internal sealed class AltairInitiationTrialMission : MissionLogic
    {
        private readonly AltairInitiationTrialQuest _quest;
        private readonly CharacterObject _master;
        private readonly CharacterObject _assassin;
        private readonly CharacterObject _apprentice;
        private readonly CharacterObject _enemy;
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

        private AltairInitiationTrialMission(AltairInitiationTrialQuest quest, CharacterObject master, CharacterObject assassin, CharacterObject apprentice, CharacterObject enemy)
        {
            _quest = quest;
            _master = master;
            _assassin = assassin;
            _apprentice = apprentice;
            _enemy = enemy;
            _previousHitPoints = Hero.MainHero.HitPoints;
        }

        internal static Mission Open(string scene, Location tavern, AltairInitiationTrialQuest quest)
        {
            CharacterObject master = CharacterObject.Find(AltairInitiationTrialBehavior.MasterId);
            CharacterObject assassin = CharacterObject.Find(AltairInitiationTrialBehavior.AssassinId);
            CharacterObject apprentice = CharacterObject.Find(AltairInitiationTrialBehavior.ApprenticeId);
            CharacterObject enemy = CharacterObject.Find(AltairInitiationTrialBehavior.EnemyId);
            if (master == null || assassin == null || apprentice == null || enemy == null)
                throw new InvalidOperationException("Initiation trial characters are missing.");
            return MissionState.OpenNew("ArenaDuelMission",
                SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, DecalAtlasGroup.Town),
                mission => new MissionBehavior[]
                {
                    new MissionOptionsComponent(),
                    new MissionFacialAnimationHandler(),
                    new MissionAgentPanicHandler(),
                    new AgentHumanAILogic(),
                    new VisualTrackerMissionBehavior(),
                    new CampaignMissionComponent(),
                    new EquipmentControllerLeaveLogic(),
                    new MissionAgentHandler(),
                    new MissionLocationLogic(tavern),
                    new AltairInitiationTrialMission(quest, master, assassin, apprentice, enemy)
                });
        }

        public override void AfterStart()
        {
            try
            {
                List<MatrixFrame> frames = Mission.Scene.FindEntitiesWithTag("sp_tavern").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count < 2)
                    frames = Mission.Scene.FindEntitiesWithTag("sp_tavern_respawn").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count < 2)
                    frames = Mission.Scene.FindEntitiesWithTag("spawnpoint_tavernkeeper").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count < 2)
                    frames = Mission.Scene.FindEntitiesWithTag("spawnpoint_taverngamehost").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count < 2)
                    frames = Mission.Scene.FindEntitiesWithTag("sp_tavern_wench").Select(e => e.GetGlobalFrame()).ToList();
                if (frames.Count == 0)
                    throw new InvalidOperationException("Tavern requires a spawn point.");

                Mission.Teams.Add(BattleSideEnum.Defender, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2);
                Mission.Teams.Add(BattleSideEnum.Attacker, _enemy.Culture.Color, _enemy.Culture.Color2);
                Mission.PlayerTeam = Mission.Teams.Defender;

                MatrixFrame playerFrame = frames[0];
                _player = Spawn(CharacterObject.PlayerCharacter, Mission.PlayerTeam, playerFrame.origin + new Vec3(2f, 0f, 0f), playerFrame.origin, true);
                SpawnGroup(_master, 3, Mission.PlayerTeam, frames, 3f);
                SpawnGroup(_assassin, 3, Mission.PlayerTeam, frames, 5f);
                SpawnGroup(_apprentice, 2, Mission.PlayerTeam, frames, 7f);
                SpawnGroup(_enemy, 10, Mission.PlayerEnemyTeam, frames, 13f);
                _started = true;
            }
            catch (Exception ex)
            {
                Debug.Print("[MercenaryVariety] Initiation trial setup failed: " + ex);
                _interrupted = true;
                Finish(false, "{=MVAltairInitiationUnavailable}The operation could not begin. Your quest is unchanged; speak to Altair to try again.");
            }
        }

        private void SpawnGroup(CharacterObject character, int count, Team team, List<MatrixFrame> frames, float distance)
        {
            for (int i = 0; i < count; i++)
            {
                MatrixFrame frame = frames[i % frames.Count];
                Vec3 position = frame.origin + new Vec3(distance + (i % 3), (i / 3) * 1.5f, 0f);
                Spawn(character, team, position, frame.origin, false);
            }
        }

        private Agent Spawn(CharacterObject character, Team team, Vec3 position, Vec3 facing, bool player)
        {
            Vec2 direction = (facing - position).AsVec2.Normalized();
            Agent agent = Mission.SpawnAgent(new AgentBuildData(character)
                .BodyProperties(character.GetBodyPropertiesMax())
                .Team(team).InitialPosition(in position).InitialDirection(direction)
                .NoHorses(true).Equipment(character.FirstBattleEquipment)
                .TroopOrigin(new SimpleAgentOrigin(character))
                .Controller(player ? AgentControllerType.Player : AgentControllerType.AI));
            agent.Health = agent.HealthLimit;
            if (!player)
                agent.SetWatchState(Agent.WatchState.Alarmed);
            if (team == Mission.PlayerEnemyTeam)
                _opponents.Add(agent);
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
            bool playerStanding = _player != null && _player.IsActive() && _player.Health > 0f;
            if (_interrupted || !playerStanding)
                Finish(false, "{=MVAltairInitiationLost}You were defeated. Speak to Altair when you are ready to try again.");
            else if (_defeated.Count == _opponents.Count)
                Finish(true, "{=MVAltairInitiationWon}The Shadow's Children have been defeated. Return to Altair.");
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
                Finish(false, "{=MVAltairInitiationLost}You withdrew from the operation. Speak to Altair to try again.");
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
