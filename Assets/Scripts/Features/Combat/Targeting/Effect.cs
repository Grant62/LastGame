namespace Features.Combat.Targeting
{
    public abstract class Effect
    {
        public abstract void Execute(ITargetable[] targets, ITargetable caster);
    }
}