namespace Features.Combat.Targeting
{
    public interface IDamageable : ITargetable
    {
        void TakeDamage(int amount);
        void TakeHeal(int amount);
    }
}