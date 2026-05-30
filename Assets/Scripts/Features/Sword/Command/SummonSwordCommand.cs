using Features.Hero.Model;
using Features.Sword.Model;
using QFramework;

namespace Features.Sword.Command
{
    public class SummonSwordCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            ISwordModel sword = this.GetModel<ISwordModel>();
            int slot = hero.CurrentSlotIndex.Value;

            sword.CurrentSlotIndex.Value = -1;
            sword.CurrentSlotIndex.Value = slot;
            sword.IsSummoned.Value = true;
            sword.IsFollowingPlayer.Value = true;
        }
    }
}