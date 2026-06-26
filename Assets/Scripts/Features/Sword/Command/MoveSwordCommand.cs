using System.Collections.Generic;
using Features.Combat.Model;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Sword.Model;
using QFramework;

namespace Features.Sword.Command
{
    public class MoveSwordCommand : AbstractCommand
    {
        private readonly int mTargetSlotIndex;

        public MoveSwordCommand(int targetSlotIndex)
        {
            mTargetSlotIndex = targetSlotIndex;
        }

        protected override void OnExecute()
        {
            ISwordModel model = this.GetModel<ISwordModel>();
            BoardView board = this.GetUtility<IBoardAccess>().Board;
            int pathDamage = this.GetModel<IGameConfigModel>().SwordPathDamage + model.CustomPathDamage;
            int oldSlot = model.CurSlotIndex.Value;

            if (model.IsSpinning.Value && !model.KeepSpinningOnMove)
            {
                model.IsSpinning.Value = false;
                model.SpinDamage.Value = 0;
            }

            model.KeepSpinningOnMove = false;

            if (model.RecallSpiritsOnSwordMove && model.SpiritSwordSlots.Count > 0)
            {
                List<int> spiritSlots = new(model.SpiritSwordSlots);
                int spiritDmg = model.RecallSpiritsDamagePerSpirit;

                int spiritPath = this.GetModel<IGameConfigModel>().SpiritPathDamage + model.CustomPathDamage;

                foreach (int fromSlot in spiritSlots)
                {
                    int step = mTargetSlotIndex > fromSlot ? 1 : -1;
                    for (int i = fromSlot; i != mTargetSlotIndex + step; i += step)
                    {
                        if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                            enemy.TakeDamage(spiritPath);
                    }
                }

                model.SpiritSwordSlots.Clear();

                if (board.TryGetEnemyAtSlot(mTargetSlotIndex, out EnemyView targetEnemy) && targetEnemy.IsValidTarget)
                {
                    for (int i = 0; i < spiritSlots.Count; i++)
                        targetEnemy.TakeDamage(spiritDmg);
                }

                model.IsRecalling = true;
                model.RecallTargetSlot = mTargetSlotIndex;
                model.OnSpiritSwordsChanged.Trigger();
            }

            DealPathDamageAndSpirits(oldSlot, mTargetSlotIndex, model, pathDamage);

            model.CurSlotIndex.Value = mTargetSlotIndex;
        }

        private void DealPathDamageAndSpirits(int from, int to, ISwordModel model, int pathDamage)
        {
            BoardView board = this.GetUtility<IBoardAccess>().Board;
            bool suppressDmg = model.SuppressPathDamage;
            model.SuppressPathDamage = false;

            int step = to > from ? 1 : -1;
            for (int i = from; i != to + step; i += step)
            {
                if (board.TryGetEnemyAtSlot(i, out EnemyView enemy) && enemy.IsValidTarget)
                {
                    if (!suppressDmg)
                        enemy.TakeDamage(pathDamage);

                    if (model.IsSpiritAttached.Value && !model.SpiritSwordSlots.Contains(i))
                        model.SpiritSwordSlots.Add(i);
                }
            }

            if (model.IsSpiritAttached.Value)
                model.OnSpiritSwordsChanged.Trigger();
        }
    }
}