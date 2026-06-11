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
            if (!model.IsSummoned.Value)
                return;

            int oldSlot = model.CurSlotIndex.Value;

            if (model.IsSpinning.Value && !model.KeepSpinningOnMove)
            {
                model.IsSpinning.Value = false;
                model.SpinDamage.Value = 0;
            }

            model.KeepSpinningOnMove = false;

            if (model.IsSpiritAttached)
                SpawnSpiritsAlongPath(oldSlot, mTargetSlotIndex, model);

            model.CurSlotIndex.Value = mTargetSlotIndex;
        }

        private void SpawnSpiritsAlongPath(int from, int to, ISwordModel model)
        {
            BoardPanel board = GameMain.Interface.GetUtility<IBoardAccess>().Board;
            if (board == null) return;

            int step = to > from ? 1 : -1;
            for (int i = from; i != to; i += step)
            {
                if (board.TryGetEnemyAtSlot(i, out EnemyUI enemy)
                    && enemy.IsValidTarget
                    && !model.SpiritSwordSlots.Contains(i))
                {
                    model.SpiritSwordSlots.Add(i);
                }
            }

            model.OnSpiritSwordsChanged.Trigger();
        }
    }
}