using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(StartingCardInfo))]
    public class StartingCardInfoContainer
    {
        public Dictionary<int, StartingCardInfo> DataDic = new();
    }
}