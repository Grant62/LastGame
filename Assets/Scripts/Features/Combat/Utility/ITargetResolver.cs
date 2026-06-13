using Features.Combat.Targeting;
using QFramework;

namespace Features.Combat.Utility
{
    public interface ITargetResolver : IUtility
    {
        ITargetable[] Resolve(TargetType type, ITargetable caster);
    }
}