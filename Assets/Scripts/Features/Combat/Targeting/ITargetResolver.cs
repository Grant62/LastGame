using QFramework;

namespace Features.Combat.Targeting
{
    public interface ITargetResolver : IUtility
    {
        ITargetable[] Resolve(TargetType type, ITargetable caster);
    }
}