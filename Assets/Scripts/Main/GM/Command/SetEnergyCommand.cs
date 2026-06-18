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
            this.GetModel<IResourceModel>().CurEnergy.Value = mEnergy;
        }
    }
}