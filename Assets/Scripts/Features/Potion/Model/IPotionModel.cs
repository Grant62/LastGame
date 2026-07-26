using System.Collections.Generic;
using QFramework;

namespace Features.Potion.Model
{
    public interface IPotionModel : IModel
    {
        IReadOnlyList<cfg.PotionInfo> Inventory { get; }
        int MaxSlots { get; }
        bool IsFull { get; }
        EasyEvent OnInventoryChanged { get; }

        void AddPotion(cfg.PotionInfo potion);
        void RemoveAt(int index);
        cfg.PotionInfo GetPotionAt(int index);
    }
}