namespace Features.Combat.Interfaces
{
    public interface IDamageable : ITargetable
    {
        void TakeDamage(int amount);
        void TakeHeal(int amount);
        void GainArmor(int amount);
    }
}