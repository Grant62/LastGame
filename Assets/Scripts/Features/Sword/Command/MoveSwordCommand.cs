using Core.Architecture;
using Features.Combat.UI;
using Features.Combat.UI.Board;
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

            int oldSlot = model.CurSlotIndex.Value;

            if (model.IsSpinning.Value && !model.KeepSpinningOnMove)
            {
                model.IsSpinning.Value = false;
                model.SpinDamage.Value = 0;
            }

            model.KeepSpinningOnMove = false;

            DealPathDamageAndSpirits(oldSlot, mTargetSlotIndex, model);

            model.CurSlotIndex.Value = mTargetSlotIndex;
        }

        private void DealPathDamageAndSpirits(int from, int to, ISwordModel model)
        {
            BoardPanel board = GameMain.Interface.GetUtility<IBoardAccess>().Board;
            bool suppressDmg = model.SuppressPathDamage;
            model.SuppressPathDamage = false;

            int step = to > from ? 1 : -1;
            for (int i = from; i != to + step; i += step)
            {
                if (board.TryGetEnemyAtSlot(i, out EnemyUI enemy) && enemy.IsValidTarget)
                {
                    if (!suppressDmg)
                        enemy.TakeDamage(4);

                    if (model.IsSpiritAttached && !model.SpiritSwordSlots.Contains(i))
                        model.SpiritSwordSlots.Add(i);
                }
            }

            if (model.IsSpiritAttached)
                model.OnSpiritSwordsChanged.Trigger();
        }
    }
}