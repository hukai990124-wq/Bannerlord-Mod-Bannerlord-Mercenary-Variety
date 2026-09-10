using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MercenaryVariety
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
#if !MV_EDITOR
            EquipmentNameColors.TryInstall();
#endif
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
#if !MV_EDITOR
            EquipmentNameColors.TryInstall();
#endif

            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (game.GameType is Campaign && campaignGameStarter != null)
            {
                campaignGameStarter.AddBehavior(new HodophylakesProgressBehavior());
                campaignGameStarter.AddBehavior(new HodophylakesDialogBehavior());
                campaignGameStarter.AddBehavior(new OldVaegirGuardsProgressBehavior());
                campaignGameStarter.AddBehavior(new OldVaegirGuardsDialogBehavior());
                campaignGameStarter.AddBehavior(new HodophylakesGuildMenuBehavior());
                campaignGameStarter.AddBehavior(new VaegirShelterMenuBehavior());
                campaignGameStarter.AddBehavior(new SeaRaiderGuildMenuBehavior());
                campaignGameStarter.AddBehavior(new SeaRaiderSiegeBehavior());
                campaignGameStarter.AddBehavior(new WesternMercenaryGuildProgressBehavior());
                campaignGameStarter.AddBehavior(new WesternMercenaryOutpostMenuBehavior());
                campaignGameStarter.AddBehavior(new AltairTavernBehavior());
                campaignGameStarter.AddBehavior(new AltairMartialTrialBehavior());
                campaignGameStarter.AddBehavior(new AltairWisdomTrialBehavior());
                campaignGameStarter.AddBehavior(new AltairInitiationTrialBehavior());
                campaignGameStarter.AddModel(new AltairDisguiseDetectionModel());
            }
        }
    }
}
