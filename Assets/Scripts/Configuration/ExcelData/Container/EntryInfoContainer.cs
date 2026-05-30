using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(EntryInfo))]
    public class EntryInfoContainer
    {
        public Dictionary<int, EntryInfo> DataDic = new();
    }
}