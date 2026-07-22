using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(EnemyGroupInfo))]
    public class EnemyGroupInfoContainer
    {
        private readonly Dictionary<int, EnemyGroupInfo> mData = new();

        public IReadOnlyDictionary<int, EnemyGroupInfo> DataDic { get => mData; }
    }
}