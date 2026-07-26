using System;
using Features.Card.Define;
using Features.Card.Model;
using Features.Card.Utility;
using Features.Configuration.Model;
using QFramework;

namespace Features.Card.Command
{
    public class LoadCardDefinesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ICardDefineModel model = this.GetModel<ICardDefineModel>();

            if (model.Defines.Count > 0)
                return;

            cfg.TbCardInfo table = this.GetUtility<ILubanDataModel>().Tables.TbCardInfo;

            foreach (cfg.CardInfo info in table.DataList)
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
                        MakeSlot(info.Effect1Type, info.Effect1Target, info.Effect1Param1, info.Effect1Param2, info.Effect1Condition),
                        MakeSlot(info.Effect2Type, info.Effect2Target, info.Effect2Param1, info.Effect2Param2, info.Effect2Condition),
                        MakeSlot(info.Effect3Type, info.Effect3Target, info.Effect3Param1, info.Effect3Param2, info.Effect3Condition)
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