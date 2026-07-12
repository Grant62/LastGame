using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Event;
using Features.Card.Model;
using Features.Combat.Event;
using QFramework;

namespace Features.Sword.System
{
    public class SwordSystem : AbstractSystem, ISwordSystem
    {
        private readonly Dictionary<CardData, int> mOriginalCosts = new();

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

        protected override void OnInit()
        {
            this.RegisterEvent<BattleEndCleanupEvent>(_ => RestoreCardCosts());
        }
    }
}