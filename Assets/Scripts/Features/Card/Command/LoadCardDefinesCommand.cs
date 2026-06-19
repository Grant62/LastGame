using System;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Features.Card.Define;
using Features.Card.Model;
using Features.Card.Utility;
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
                    Price = info.Price,
                    SlotActionStr = info.SlotAction ?? "None",
                    SlotDistance = info.SlotDistance,
                    EffectSlots = new[]
                    {
                        MakeSlot(info.Effect1_Type, info.Effect1_Target, info.Effect1_Param1, info.Effect1_Param2, info.Effect1_Condition),
                        MakeSlot(info.Effect2_Type, info.Effect2_Target, info.Effect2_Param1, info.Effect2_Param2, info.Effect2_Condition),
                        MakeSlot(info.Effect3_Type, info.Effect3_Target, info.Effect3_Param1, info.Effect3_Param2, info.Effect3_Condition),
                        MakeSlot(info.Effect4_Type, info.Effect4_Target, info.Effect4_Param1, info.Effect4_Param2, info.Effect4_Condition),
                        MakeSlot(info.Effect5_Type, info.Effect5_Target, info.Effect5_Param1, info.Effect5_Param2, info.Effect5_Condition),
                        MakeSlot(info.Effect6_Type, info.Effect6_Target, info.Effect6_Param1, info.Effect6_Param2, info.Effect6_Condition)
                    }
                });
            }
        }

        private static EffectSlot MakeSlot(string typeStr, string targetStr, string param1, string param2, string condStr)
        {
            if (string.IsNullOrEmpty(typeStr))
                return default;

            return new EffectSlot
            {
                Type = ParseEnum(typeStr, EffectType.None),
                Target = ParseEnum(targetStr, EffectTarget.Self),
                Param1 = param1 ?? "",
                Param2 = param2 ?? "",
                Condition = ParseEnum(condStr, EffectCondition.None)
            };
        }

        private static T ParseEnum<T>(string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            if (Enum.TryParse(value, out T result))
                return result;

            return defaultValue;
        }
    }
}