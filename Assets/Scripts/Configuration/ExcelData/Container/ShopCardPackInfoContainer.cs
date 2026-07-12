using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(ShopCardPackInfo))]
    public class ShopCardPackInfoContainer
    {
        private readonly Dictionary<int, ShopCardPackInfo> mData = new();
        public IReadOnlyDictionary<int, ShopCardPackInfo> DataDic => mData;
    }
}