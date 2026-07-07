using System.Collections.Generic;
using Core.Systems;
using Features.Card.Data;
using Features.Card.Model;
using QFramework;

namespace Features.Combat.Command
{
    public class ShuffleAllToDrawPileCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ICardModel model = this.GetModel<ICardModel>();
            IRandomSystem random = this.GetSystem<IRandomSystem>();

            model.DrawPile.AddRange(model.HandPile);
            model.HandPile.Clear();
            model.OnHandPileChanged.Trigger();

            model.DrawPile.AddRange(model.DiscardPile);
            model.DiscardPile.Clear();
            model.OnDiscardPileChanged.Trigger();

            model.DrawPile.AddRange(model.ConsumePile);
            model.ConsumePile.Clear();
            model.OnConsumePileChanged.Trigger();

            Shuffle(model.DrawPile, random);
            model.OnDrawPileChanged.Trigger();
        }

        private static void Shuffle(List<CardData> list, IRandomSystem random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Range(0, i + 1, 0);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}