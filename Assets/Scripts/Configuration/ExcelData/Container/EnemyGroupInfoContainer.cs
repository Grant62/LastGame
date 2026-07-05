using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(EnemyGroupInfo))]
    public class EnemyGroupInfoContainer
    {
        public Dictionary<int, EnemyGroupInfo> DataDic = new();
    }
}