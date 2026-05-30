using Core.Architecture;
using Features.Card.Model;
using Features.Combat.Interaction;
using QFramework;

namespace Features.Combat.View
{
    public partial class CardPileBtn : ViewController, IController
    {
        protected ICardModel mModel;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected virtual void Start()
        {
            mModel = this.GetModel<ICardModel>();

            mModel.OnDrawPileChanged.Register(RefreshDisplay)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mModel.OnDiscardPileChanged.Register(RefreshDisplay)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mModel.OnHandPileChanged.Register(RefreshDisplay)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            RefreshDisplay();
        }

        protected virtual int GetPileCount()
        {
            return 0;
        }

        protected virtual void RefreshDisplay() { }

        protected virtual void OnMouseDown()
        {
            if (!this.GetSystem<IInteractionSystem>().CanInteract())
                return;
        }
    }
}