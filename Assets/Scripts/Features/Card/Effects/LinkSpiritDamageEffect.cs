using Features.Combat.Targeting;
using Features.Combat.UI.Board;
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
                BoardPanel board = Ctx.BoardAccess.Board;
                if (board.TryGetEnemyAtSlot(slot, out EnemyUI enemy)
                    && enemy.IsValidTarget)
                    enemy.TakeDamage(mDamagePerSpirit);
            }
        }
    }
}