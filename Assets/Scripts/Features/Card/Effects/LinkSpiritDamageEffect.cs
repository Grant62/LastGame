using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class LinkSpiritDamageEffect : Effect
    {
        private readonly int mDamagePerSpirit;

        public LinkSpiritDamageEffect(int damagePerSpirit = 6)
        {
            mDamagePerSpirit = damagePerSpirit;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel swordModel = Ctx.SwordModel;

            foreach (int slot in swordModel.SpiritSwordSlots)
            {
                BoardView board = Ctx.BoardAccess.Board;
                if (board.TryGetEnemyAtSlot(slot, out EnemyView enemy)
                    && enemy.IsValidTarget)
                    enemy.TakeDamage(mDamagePerSpirit);
            }
        }
    }
}