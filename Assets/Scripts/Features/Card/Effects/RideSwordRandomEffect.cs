using Core.Systems;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;

namespace Features.Card.Effects
{
    public class RideSwordRandomEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ISwordModel model = Ctx.SwordModel;
            int oldSlot = model.CurSlotIndex.Value;
            int targetSlot = Ctx.RandomSystem.Range(0, Ctx.BoardAccess.Board.SlotCount, RandomModuleIds.Combat);

            if (model.IsSpinning.Value && !model.KeepSpinningOnMove)
            {
                model.IsSpinning.Value = false;
                model.SpinDamage.Value = 0;
            }

            model.KeepSpinningOnMove = false;

            DealPathDamageAndSpirits(oldSlot, targetSlot, model);

            model.CurSlotIndex.Value = targetSlot;
        }

        private void DealPathDamageAndSpirits(int from, int to, ISwordModel model)
        {
            BoardView board = Ctx.BoardAccess.Board;
            bool suppressDmg = model.SuppressPathDamage;
            model.SuppressPathDamage = false;

            int step = to > from ? 1 : -1;
            for (int i = from; i != to + step; i += step)
            {
                if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                {
                    if (!suppressDmg)
                        enemy.TakeDamage(Ctx.Config.SwordPathDamage);

                    if (model.IsSpiritAttached.Value && !model.SpiritSwordSlots.Contains(i))
                        model.SpiritSwordSlots.Add(i);
                }
            }

            if (model.IsSpiritAttached.Value)
                model.OnSpiritSwordsChanged.Trigger();
        }
    }
}