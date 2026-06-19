using System.Collections.Generic;
using Core.Systems;
using Features.Card.Data;
using Features.Card.Event;
using Features.Card.Model;
using Features.Combat.Event;
using QFramework;

namespace Features.Card.System
{
    public class CardSystem : AbstractSystem, ICardSystem
    {
        private readonly Dictionary<CardData, int> mOriginalCosts = new();

        protected override void OnInit()
        {
            this.RegisterEvent<PlayerTurnStartEvent>(OnPlayerTurnStart);
            this.RegisterEvent<PlayerTurnEndEvent>(OnPlayerTurnEnd);
            this.RegisterEvent<BattleVictoryEvent>(OnBattleVictory);
            this.RegisterEvent<BattleDefeatEvent>(OnBattleDefeat);
        }

        private void OnBattleVictory(BattleVictoryEvent e)
        {
            RestoreCardCosts();
        }

        private void OnBattleDefeat(BattleDefeatEvent e)
        {
            RestoreCardCosts();
        }

        private void OnPlayerTurnStart(PlayerTurnStartEvent e)
        {
            DrawCards(5);
        }

        private void OnPlayerTurnEnd(PlayerTurnEndEvent e)
        {
            ICardModel model = this.GetModel<ICardModel>();
            if (model.KeepHandOnTurnEnd)
                return;
            if (model.HandPile.Count == 0)
                return;

            model.DiscardPile.AddRange(model.HandPile);
            model.HandPile.Clear();
            model.OnHandPileChanged.Trigger();
            model.OnDiscardPileChanged.Trigger();
        }

        public void InitLibrary(List<CardData> cards)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.Library.Clear();
            model.Library.AddRange(cards);
            model.OnLibraryChanged.Trigger();
        }

        public void StartBattleDraw()
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.DrawPile.Clear();
            model.DrawPile.AddRange(model.Library);
            model.ConsumePile.Clear();
            model.OnConsumePileChanged.Trigger();
            ShuffleDrawPile();
        }

        public void DrawCards(int count)
        {
            ICardModel model = this.GetModel<ICardModel>();
            const int MaxHandSize = 10;
            bool discardChanged = false;

            for (int i = 0; i < count; i++)
            {
                if (model.HandPile.Count >= MaxHandSize)
                {
                    if (model.DrawPile.Count == 0)
                        break;

                    CardData overflow = model.DrawPile[0];
                    model.DrawPile.RemoveAt(0);
                    model.DiscardPile.Add(overflow);
                    discardChanged = true;
                    continue;
                }

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
            if (discardChanged)
                model.OnDiscardPileChanged.Trigger();
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

        public void AddToHand(CardData card)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.HandPile.Add(card);
            model.OnHandPileChanged.Trigger();
        }

        public void AddToConsume(CardData card)
        {
            ICardModel model = this.GetModel<ICardModel>();
            model.HandPile.Remove(card);
            model.DrawPile.Remove(card);
            model.DiscardPile.Remove(card);
            model.ConsumePile.Add(card);
            model.OnConsumePileChanged.Trigger();
        }

        public void ShuffleDrawPile()
        {
            ICardModel model = this.GetModel<ICardModel>();
            Shuffle(model.DrawPile);
            model.OnDrawPileChanged.Trigger();
        }

        private void ShuffleDiscardIntoDrawPile(ICardModel model)
        {
            if (model.DiscardPile.Count == 0)
                return;

            model.DrawPile.AddRange(model.DiscardPile);
            model.DiscardPile.Clear();
            Shuffle(model.DrawPile);
            model.OnDiscardPileChanged.Trigger();
            model.OnDrawPileChanged.Trigger();
        }

        public void ReduceSpinCardCosts()
        {
            ICardModel model = this.GetModel<ICardModel>();
            foreach (CardData card in model.HandPile)
            {
                if (card.HasSpinEffect && card.Cost > 0)
                {
                    if (!mOriginalCosts.ContainsKey(card))
                        mOriginalCosts[card] = card.Cost;

                    card.Cost -= 1;
                }
            }

            this.SendEvent<HandCardCostChangedEvent>();
        }

        public void RestoreCardCosts()
        {
            foreach (KeyValuePair<CardData, int> kv in mOriginalCosts)
                kv.Key.Cost = kv.Value;

            mOriginalCosts.Clear();
        }

        private void Shuffle(List<CardData> list)
        {
            IRandomSystem random = this.GetSystem<IRandomSystem>();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Range(0, i + 1, RandomModuleIds.Combat);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}