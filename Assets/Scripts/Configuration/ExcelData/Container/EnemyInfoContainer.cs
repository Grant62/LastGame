using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{
    [BinaryTable(DataType = typeof(EnemyInfo))]
    public class EnemyInfoContainer
    {
        private readonly Dictionary<int, EnemyInfo> mData = new();

        public IReadOnlyDictionary<int, EnemyInfo> DataDic { get => mData; }
    }
}