using System.Collections.Generic;
using QFramework;

namespace Configuration.Model
{
    public interface IDataTableModel : IModel
    {
        Dictionary<int, T> GetTable<T>() where T : class;
        bool TryGet<T>(int id, out T row) where T : class;
        void RegisterTable<T>(Dictionary<int, T> table) where T : class;
        void MarkInitialized();
        bool IsInitialized { get; }
    }
}
