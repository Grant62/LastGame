using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Features.Card.Define;
using Features.Card.Model;
using QFramework;
using Services.ExcelTool;

namespace Features.Card.Command
{
    public class LoadCardDefinesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ICardDefineModel model = this.GetModel<ICardDefineModel>();

            if (model.Defines.Count > 0)
                return;

            CardInfoContainer container = this.GetUtility<IBinaryDataMgr>().GetTable<CardInfoContainer>();

            foreach (CardInfo info in container.DataDic.Values)
            {
                model.Register(new CardDefine
                {
                    Id = info.CardId,
                    Name = info.Name,
                    Cost = info.Cost,
                    Desc = info.Desc,
                    Type = info.Type,
                    Rarity = info.Rarity,
                    IconAddress = info.IconAddress,
                    Price = info.Price
                });
            }
        }
    }
}