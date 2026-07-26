using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Model;
using Features.Card.System;
using QFramework;
using UnityEngine;

namespace Features.Card.Command
{
    public class InitDeckFromJsonCommand : AbstractCommand
    {
        private readonly TextAsset mJson;

        public InitDeckFromJsonCommand(TextAsset json)
        {
            mJson = json;
        }

        protected override void OnExecute()
        {
            CardAmountEntryList list = JsonUtility.FromJson<CardAmountEntryList>(
                "{\"entries\":" + mJson.text + "}");

            if (list?.entries == null || list.entries.Length == 0)
                return;

            ICardDefineModel defines = this.GetModel<ICardDefineModel>();
            List<CardData> deck = new();

            foreach (CardAmountEntry entry in list.entries)
            {
                if (!defines.TryGet(entry.cardId, out CardDefine define))
                    continue;

                for (int i = 0; i < entry.amount; i++)
                    deck.Add(define.CreateCardData());
            }

            this.GetSystem<ICardSystem>().InitLibrary(deck);
        }
    }
}