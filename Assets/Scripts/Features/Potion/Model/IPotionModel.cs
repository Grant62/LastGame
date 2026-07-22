using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using QFramework;

namespace Features.Potion.Model
{
    public interface IPotionModel : IModel
    {
        IReadOnlyList<PotionInfo> Inventory { get; }
        int MaxSlots { get; }
        bool IsFull { get; }
        EasyEvent OnInventoryChanged { get; }

        void AddPotion(PotionInfo potion);
        void RemoveAt(int index);
        PotionInfo GetPotionAt(int index);
    }
}