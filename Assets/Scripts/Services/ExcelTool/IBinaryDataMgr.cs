using QFramework;

namespace Services.ExcelTool
{
    public interface IBinaryDataMgr : IUtility
    {
        T GetTable<T>() where T : class;
    }
}