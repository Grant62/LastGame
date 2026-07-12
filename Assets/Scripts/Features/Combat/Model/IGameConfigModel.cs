using QFramework;

namespace Features.Combat.Model
{
    public interface IGameConfigModel : IModel
    {
        int SwordPathDamage { get; }
        int SpiritPathDamage { get; }
        int SpinBaseDamage { get; }
        int LinkBlockPerSword { get; }

        int InitialEnergy { get; }
        int CardsPerTurn { get; }
        int MaxStepsPerLayer { get; }
        int MaxLayers { get; }
        float ShortRestHealPercent { get; }
        int ShortRestMaxCount { get; }
        float WeakMultiplier { get; }
        float VulnerableMultiplier { get; }

        int GetFloorClearGold(int step);
    }
}