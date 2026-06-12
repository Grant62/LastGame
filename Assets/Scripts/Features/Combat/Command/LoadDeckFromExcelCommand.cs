using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Features.Card.Data;
using Features.Card.Define;
using Features.Card.System;
using QFramework;
using Services.ExcelTool;

namespace Features.Combat.Command
{
    public class LoadDeckFromExcelCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IBinaryDataMgr dataMgr = this.GetUtility<IBinaryDataMgr>();
            CardInfoContainer cardContainer = dataMgr.GetTable<CardInfoContainer>();
            StartingCardInfoContainer startContainer = dataMgr.GetTable<StartingCardInfoContainer>();

            Dictionary<string, int> nameToId = new();
            foreach (CardInfo info in cardContainer.DataDic.Values)
                nameToId[info.Name] = info.CardId;

            ICardDefineModel defines = this.GetModel<ICardDefineModel>();
            List<CardData> deck = new();

            foreach (StartingCardInfo start in startContainer.DataDic.Values)
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