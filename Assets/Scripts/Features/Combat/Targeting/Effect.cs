namespace Features.Combat.Targeting
{
    public abstract class Effect
    {
        public EffectContext Ctx { get; set; }

        public abstract void Execute(ITargetable[] targets, ITargetable caster);
    }
}