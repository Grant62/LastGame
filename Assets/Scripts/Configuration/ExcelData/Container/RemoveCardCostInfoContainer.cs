using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(RemoveCardCostInfo))]
    public class RemoveCardCostInfoContainer
    {
        private readonly Dictionary<int, RemoveCardCostInfo> mData = new();

        public IReadOnlyDictionary<int, RemoveCardCostInfo> DataDic { get => mData; }
    }
}