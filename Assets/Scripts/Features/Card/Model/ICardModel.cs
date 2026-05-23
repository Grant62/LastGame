using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.Model
{
    public interface ICardModel : IModel
    {
        List<CardData> DrawPile { get; }
        List<CardData> HandPile { get; }
        List<CardData> DiscardPile { get; }

        EasyEvent OnDrawPileChanged { get; }
        EasyEvent OnHandPileChanged { get; }
        EasyEvent OnDiscardPileChanged { get; }
    }
}