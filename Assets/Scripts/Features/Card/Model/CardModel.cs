using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.Model
{
    public class CardModel : AbstractModel, ICardModel
    {
        public List<CardData> DrawPile { get; } = new();
        public List<CardData> HandPile { get; } = new();
        public List<CardData> DiscardPile { get; } = new();

        public EasyEvent OnDrawPileChanged { get; } = new();
        public EasyEvent OnHandPileChanged { get; } = new();
        public EasyEvent OnDiscardPileChanged { get; } = new();

        protected override void OnInit() { }
    }
}