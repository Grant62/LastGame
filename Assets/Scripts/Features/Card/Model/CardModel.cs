using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.Model
{
    public class CardModel : AbstractModel, ICardModel
    {
        public List<CardData> Library { get; } = new();
        public List<CardData> DrawPile { get; } = new();
        public List<CardData> HandPile { get; } = new();
        public List<CardData> DiscardPile { get; } = new();
        public List<CardData> ConsumePile { get; } = new();

        public EasyEvent OnLibraryChanged { get; } = new();
        public EasyEvent OnDrawPileChanged { get; } = new();
        public EasyEvent OnHandPileChanged { get; } = new();
        public EasyEvent OnDiscardPileChanged { get; } = new();
        public EasyEvent OnConsumePileChanged { get; } = new();

        public bool KeepHandOnTurnEnd { get; set; }
        public int PendingDiscardCount { get; set; }

        protected override void OnInit() { }
    }
}