using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace MercenaryVariety
{
    public sealed class AltairWisdomTrialQuest : QuestBase
    {
        internal const int RequiredSkillLevel = 50;

        [SaveableField(1)]
        private bool _qualified;

        public override TextObject Title => new TextObject("{=MVAltairWisdomTitle}Hashashin Initiate Trial - Wisdom");
        public override bool IsRemainingTimeHidden => true;
        public bool IsReadyToTurnIn => _qualified || MeetsRequirement(GetPlayerHighestSkill());

        public AltairWisdomTrialQuest(Hero questGiver) : base("mv_altair_wisdom_trial", questGiver, CampaignTime.Never, 0) { }
        protected override void SetDialogs() { }
        protected override void InitializeQuestOnGameLoad() => RegisterListeners();

        private void RegisterListeners()
        {
            CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, RefreshObjective);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        protected override void OnStartQuest()
        {
            RegisterListeners();
            AddLog(new TextObject("{=MVAltairWisdomLog}Altair asks you to prove that you can lead a company. Your own Medicine, Steward, Charm, Leadership, Scouting or Roguery must reach at least 50. Any one skill is enough; existing skill levels count. Return to Altair when you qualify."), false);
            AddDiscreteLog(new TextObject("{=MVAltairWisdomObjective}Reach 50 in any one of Medicine, Steward, Charm, Leadership, Scouting or Roguery"),
                new TextObject("{=MVAltairWisdomProgress}Highest qualifying skill"), 0, RequiredSkillLevel, null, false);
            RefreshObjective();
        }

        private void OnSessionLaunched(CampaignGameStarter starter) => RefreshObjective();

        private void OnHeroGainedSkill(Hero hero, SkillObject skill, int change, bool shouldNotify)
        {
            if (hero == Hero.MainHero)
                RefreshObjective();
        }

        internal static bool MeetsRequirement(int highestSkill) => highestSkill >= RequiredSkillLevel;

        internal static int GetHighestQualifyingSkill(Func<SkillObject, int> getSkillValue)
        {
            return Math.Max(Math.Max(Math.Max(getSkillValue(DefaultSkills.Medicine), getSkillValue(DefaultSkills.Steward)),
                Math.Max(getSkillValue(DefaultSkills.Charm), getSkillValue(DefaultSkills.Leadership))),
                Math.Max(getSkillValue(DefaultSkills.Scouting), getSkillValue(DefaultSkills.Roguery)));
        }

        private static int GetPlayerHighestSkill()
        {
            Hero player = Hero.MainHero;
            return player == null ? 0 : GetHighestQualifyingSkill(player.GetSkillValue);
        }

        internal void RefreshObjective()
        {
            if (!IsOngoing || _qualified)
                return;
            int highest = GetPlayerHighestSkill();
            foreach (JournalLog log in JournalEntries)
                if (log.TaskName?.GetID() == "MVAltairWisdomProgress")
                    log.UpdateCurrentProgress(Math.Min(highest, RequiredSkillLevel));
            if (MeetsRequirement(highest))
            {
                _qualified = true;
                AddLog(new TextObject("{=MVAltairWisdomReturn}Your skill meets Altair's requirement. Return to him and say: I think I can prove it."), false);
            }
        }

        protected override void OnCompleteWithSuccess()
        {
            AltairWisdomTrialBehavior.Instance?.MarkCompleted();
        }
    }
}
