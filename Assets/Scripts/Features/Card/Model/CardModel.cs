using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.Model
{
    public class CardModel : AbstractModel, ICardModel
    {
        public List<CardData> DrawPile { get; } = new();
        public List<CardData> Hand { get; } = new();
        public List<CardData> DiscardPile { get; } = new();

        public EasyEvent OnDrawPileChanged { get; } = new();
        public EasyEvent OnHandChanged { get; } = new();
        public EasyEvent OnDiscardPileChanged { get; } = new();

        protected override void OnInit() { }
    }
}