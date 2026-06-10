namespace Features.Combat.Targeting
{
    public interface IEnemyTarget : ITargetable
    {
        int SlotIndex { get; }
    }
}