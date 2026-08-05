using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MercenaryVariety
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (game.GameType is Campaign && campaignGameStarter != null)
            {
                campaignGameStarter.AddBehavior(new HodophylakesDialogBehavior());
            }
        }
    }
}
