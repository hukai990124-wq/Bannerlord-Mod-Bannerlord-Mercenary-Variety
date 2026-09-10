using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace MercenaryVariety
{
    public sealed class AltairDisguiseDetectionModel : DefaultDisguiseDetectionModel
    {
        private const float SneakSuccessBonus = 0.60f;
        private const float MaximumSneakSuccessChance = 0.95f;

        public override float CalculateDisguiseDetectionProbability(Settlement settlement)
        {
            float detectionProbability = base.CalculateDisguiseDetectionProbability(settlement);
            if (settlement == null ||
                !settlement.IsTown ||
                AltairTavernBehavior.Instance == null ||
                !AltairTavernBehavior.Instance.IsSneakNetworkActive)
            {
                return detectionProbability;
            }

            float successChance = 1f - detectionProbability;
            successChance = MBMath.ClampFloat(
                successChance + SneakSuccessBonus,
                0f,
                MaximumSneakSuccessChance);

            return 1f - successChance;
        }
    }
}
