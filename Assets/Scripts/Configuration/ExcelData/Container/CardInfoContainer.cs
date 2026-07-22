using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(CardInfo))]
    public class CardInfoContainer
    {
        private readonly Dictionary<int, CardInfo> mData = new();

        public IReadOnlyDictionary<int, CardInfo> DataDic { get => mData; }
    }
}