using Features.Card.Model;
using QFramework;

namespace Main.GM.Command
{
    public class ToggleKeepHandCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.KeepHandOnTurnEnd = !model.KeepHandOnTurnEnd;
        }
    }
}