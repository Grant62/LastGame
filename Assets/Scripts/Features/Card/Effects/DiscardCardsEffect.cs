using Core.Architecture;
using Features.Card.Model;
using Features.Card.System;
using Features.Combat.Targeting;
using UnityEngine;

namespace Features.Card.Effects
{
    public class DiscardCardsEffect : Effect
    {
        private readonly int mCount;

        public DiscardCardsEffect(int count)
        {
            mCount = count;
        }

        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            ICardModel model = GameMain.Interface.GetModel<ICardModel>();
            ICardSystem system = GameMain.Interface.GetSystem<ICardSystem>();

            for (int i = 0; i < mCount && model.HandPile.Count > 0; i++)
            {
                int index = Random.Range(0, model.HandPile.Count);
                system.DiscardFromHand(model.HandPile[index]);
            }
        }
    }
}