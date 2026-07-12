using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Enemy.View;

namespace Features.Card.Effects
{
    public class SpinFormulaDamageEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            int energySpent = Ctx.EnergySpent;

            int formulaDamage = 5 * (energySpent + 1);

            BoardView board = Ctx.BoardAccess.Board;
            int swordSlot = Ctx.SwordModel.CurSlotIndex.Value;

            if (board.TryGetEnemyAtSlot(swordSlot, out EnemyView enemy) && enemy.IsValidTarget)
                enemy.TakeDamage(formulaDamage);
        }
    }
}