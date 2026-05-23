using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Model;
using Features.Combat.Event;
using QFramework;
using UnityEngine;

namespace Features.Card.System
{
    public class CardSystem : AbstractSystem, ICardSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<PlayerTurnStartEvent>(OnPlayerTurnStart);
        }

        private void OnPlayerTurnStart(PlayerTurnStartEvent e)
        {
            DrawCards(5);
        }

        public void InitDrawPile(List<CardData> cards)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.DrawPile.Clear();
            model.DrawPile.AddRange(cards);
            ShuffleDrawPile();
            model.OnDrawPileChanged.Trigger();
        }

        public void DrawCards(int count)
        {
            ICardModel model = this.GetModel<ICardModel>();

            for (int i = 0; i < count; i++)
            {
                if (model.DrawPile.Count == 0)
                    ShuffleDiscardIntoDrawPile(model);

                if (model.DrawPile.Count == 0)
                    break;

                CardData drawn = model.DrawPile[0];
                model.DrawPile.RemoveAt(0);
                model.HandPile.Add(drawn);
            }

            model.OnDrawPileChanged.Trigger();
            model.OnHandPileChanged.Trigger();
        }

        public void RemoveFromHand(CardData card)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.HandPile.Remove(card);
            model.OnHandPileChanged.Trigger();
        }

        public void DiscardFromHand(CardData card)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.HandPile.Remove(card);
            model.DiscardPile.Add(card);
            model.OnHandPileChanged.Trigger();
            model.OnDiscardPileChanged.Trigger();
        }

        public void AddToDiscard(CardData card)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.DiscardPile.Add(card);
            model.OnDiscardPileChanged.Trigger();
        }

        public void ShuffleDrawPile()
        {
            ICardModel model = this.GetModel<ICardModel>();
            Shuffle(model.DrawPile);
            model.OnDrawPileChanged.Trigger();
        }

        private static void ShuffleDiscardIntoDrawPile(ICardModel model)
        {
            if (model.DiscardPile.Count == 0)
                return;

            model.DrawPile.AddRange(model.DiscardPile);
            model.DiscardPile.Clear();
            Shuffle(model.DrawPile);
            model.OnDiscardPileChanged.Trigger();
            model.OnDrawPileChanged.Trigger();
        }

        private static void Shuffle(List<CardData> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}