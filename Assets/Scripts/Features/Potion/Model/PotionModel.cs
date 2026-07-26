using System.Collections.Generic;
using QFramework;

namespace Features.Potion.Model
{
    public class PotionModel : AbstractModel, IPotionModel
    {
        private readonly List<cfg.PotionInfo> mInventory = new();

        public IReadOnlyList<cfg.PotionInfo> Inventory { get => mInventory; }

        public int MaxSlots { get => 3; }

        public bool IsFull { get => mInventory.Count >= MaxSlots; }

        public EasyEvent OnInventoryChanged { get; } = new();

        public void AddPotion(cfg.PotionInfo potion)
        {
            if (IsFull)
                return;

            mInventory.Add(potion);
            OnInventoryChanged.Trigger();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= mInventory.Count)
                return;

            mInventory.RemoveAt(index);
            OnInventoryChanged.Trigger();
        }

        public cfg.PotionInfo GetPotionAt(int index)
        {
            if (index < 0 || index >= mInventory.Count)
                return null;

            return mInventory[index];
        }

        protected override void OnInit() { }
    }
}