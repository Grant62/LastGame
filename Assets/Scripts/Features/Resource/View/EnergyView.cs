using Core.Architecture;
using Features.Resource.Model;
using QFramework;

namespace Features.Resource.View
{
    public partial class EnergyView : ViewController, IController
    {
        private IResourceModel mResourceModel;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            mResourceModel = this.GetModel<IResourceModel>();

            mResourceModel.CurEnergy.RegisterWithInitValue(OnEnergyChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEnergyChanged(int current)
        {
            Amount.text = current.ToString();
        }
    }
}