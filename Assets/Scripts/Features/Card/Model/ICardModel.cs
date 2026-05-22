using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.Model
{
    public interface ICardModel : IModel
    {
        List<CardData> DrawPile { get; }
        List<CardData> Hand { get; }
        List<CardData> DiscardPile { get; }

        EasyEvent OnDrawPileChanged { get; }
        EasyEvent OnHandChanged { get; }
        EasyEvent OnDiscardPileChanged { get; }
    }
}