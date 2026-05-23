using System;
using System.Collections.Generic;
using QFramework;

namespace Configuration.Model
{
    public class DataTableModel : AbstractModel, IDataTableModel
    {
        private readonly Dictionary<Type, object> mTables = new();

        public bool IsInitialized { get; private set; }

        protected override void OnInit() { }

        public void MarkInitialized()
        {
            IsInitialized = true;
        }

        public void RegisterTable<T>(Dictionary<int, T> table) where T : class
        {
            mTables[typeof(T)] = table;
        }

        public Dictionary<int, T> GetTable<T>() where T : class
        {
            if (mTables.TryGetValue(typeof(T), out object obj) && obj is Dictionary<int, T> dict)
                return dict;

            return new Dictionary<int, T>();
        }

        public bool TryGet<T>(int id, out T row) where T : class
        {
            row = null;
            Dictionary<int, T> table = GetTable<T>();
            return table.TryGetValue(id, out row);
        }
    }
}
