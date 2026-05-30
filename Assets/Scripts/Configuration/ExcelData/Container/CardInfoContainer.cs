using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(CardInfo))]
    public class CardInfoContainer
    {
        public Dictionary<int, CardInfo> DataDic = new();
    }
}