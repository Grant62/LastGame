using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(FloorClearGoldInfo))]
    public class FloorClearGoldInfoContainer
    {
        private readonly Dictionary<int, FloorClearGoldInfo> mData = new();

        public IReadOnlyDictionary<int, FloorClearGoldInfo> DataDic { get => mData; }
    }
}