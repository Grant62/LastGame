using Features.Resource.Model;
using QFramework;

namespace Main.GM.Command
{
    public class SetEnergyCommand : AbstractCommand
    {
        private readonly int mEnergy;

        public SetEnergyCommand(int energy)
        {
            mEnergy = energy;
        }

        protected override void OnExecute()
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.MaxEnergy.Value = mEnergy;
            model.CurEnergy.Value = mEnergy;
        }
    }
}