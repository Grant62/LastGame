using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(StartingCardInfo))]
    public class StartingCardInfoContainer
    {
        private readonly Dictionary<int, StartingCardInfo> mData = new();
        public IReadOnlyDictionary<int, StartingCardInfo> DataDic => mData;
    }
}