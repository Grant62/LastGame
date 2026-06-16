using QFramework;

namespace Features.Combat.Model
{
    public interface IGameConfigModel : IModel
    {
        int SwordPathDamage { get; }
        int SpiritPathDamage { get; }
        int SpinBaseDamage { get; }
        int LinkBlockPerSword { get; }
    }
}