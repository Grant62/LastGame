namespace Features.Combat.Interfaces
{
    public interface IEnemyTarget : ITargetable
    {
        int SlotIndex { get; }
    }
}