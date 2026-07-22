using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(PotionInfo))]
    public class PotionInfoContainer
    {
        private readonly Dictionary<int, PotionInfo> mData = new();

        public IReadOnlyDictionary<int, PotionInfo> DataDic { get => mData; }
    }
}