using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class SpiritPositionsDamageEffect : Effect
    {
        private readonly int mDamage;

        public SpiritPositionsDamageEffect(int damage)
        {
            mDamage = damage;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel sword = Ctx.SwordModel;
            BoardView board = Ctx.BoardAccess.Board;

            foreach (int slot in sword.SpiritSwordSlots)
            {
                if (board.TryGetEnemyAtSlot(slot, out EnemyView enemy) && enemy.IsValidTarget)
                    enemy.TakeDamage(mDamage);
            }
        }
    }
}
