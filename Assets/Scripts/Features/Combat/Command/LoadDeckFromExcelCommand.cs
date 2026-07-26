using System.Collections.Generic;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.Model;
using Features.Card.System;
using Features.Configuration.Model;
using QFramework;

namespace Features.Combat.Command
{
    public class LoadDeckFromExcelCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            cfg.Tables tables = this.GetUtility<ILubanDataModel>().Tables;

            Dictionary<string, int> nameToId = new();
            foreach (cfg.CardInfo info in tables.TbCardInfo.DataList)
                nameToId[info.Name] = info.CardId;

            ICardDefineModel defines = this.GetModel<ICardDefineModel>();
            List<CardData> deck = new();

            foreach (cfg.StartingCardInfo start in tables.TbStartingCardInfo.DataList)
            {
                if (nameToId.TryGetValue(start.CardName, out int cardId)
                    && defines.TryGet(cardId, out CardDefine define))
                {
                    deck.Add(define.CreateCardData());
                }
            }

            this.GetSystem<ICardSystem>().InitLibrary(deck);
        }
    }
}