using Features.Combat.Define;
using Features.Combat.Interfaces;
using QFramework;

namespace Features.Combat.Utility
{
    public interface ITargetResolver : IUtility
    {
        ITargetable[] Resolve(TargetType type, ITargetable caster);
    }
}