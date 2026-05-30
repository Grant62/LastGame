using Features.Card.Model;
using Features.Card.System;
using Features.Combat.System;
using Features.Resource.Model;
using QFramework;

namespace Features.Combat.Command
{
    public class StartBattleCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IResourceModel resource = this.GetModel<IResourceModel>();
            resource.MaxEnergy.Value = 99;
            resource.CurEnergy.Value = 99;

            ICardModel cardModel = this.GetModel<ICardModel>();
            this.GetSystem<ICardSystem>().DrawCards(cardModel.DrawPile.Count);

            ITurnSystem turnSystem = this.GetSystem<ITurnSystem>();
            turnSystem.StartBattle();
        }
    }
}