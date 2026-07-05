using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(EnemyInfo))]
    public class EnemyInfoContainer
    {
        public Dictionary<int, EnemyInfo> DataDic = new();
    }
}