using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(GameBalanceInfo))]
    public class GameBalanceInfoContainer
    {
        private readonly Dictionary<int, GameBalanceInfo> mData = new();
        public IReadOnlyDictionary<int, GameBalanceInfo> DataDic => mData;
    }
}