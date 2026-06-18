using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Card.Model
{
    public interface ICardModel : IModel
    {
        List<CardData> Library { get; }
        List<CardData> DrawPile { get; }
        List<CardData> HandPile { get; }
        List<CardData> DiscardPile { get; }
        List<CardData> ConsumePile { get; }

        EasyEvent OnLibraryChanged { get; }
        EasyEvent OnDrawPileChanged { get; }
        EasyEvent OnHandPileChanged { get; }
        EasyEvent OnDiscardPileChanged { get; }
        EasyEvent OnConsumePileChanged { get; }

        bool KeepHandOnTurnEnd { get; set; }
        int PendingDiscardCount { get; set; }
    }
}