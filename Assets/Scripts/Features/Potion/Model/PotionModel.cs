using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using QFramework;

namespace Features.Potion.Model
{
    public class PotionModel : AbstractModel, IPotionModel
    {
        private readonly List<PotionInfo> mInventory = new();

        public IReadOnlyList<PotionInfo> Inventory { get => mInventory; }

        public int MaxSlots { get => 3; }

        public bool IsFull { get => mInventory.Count >= MaxSlots; }

        public EasyEvent OnInventoryChanged { get; } = new();

        public void AddPotion(PotionInfo potion)
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

        public PotionInfo GetPotionAt(int index)
        {
            if (index < 0 || index >= mInventory.Count)
                return null;

            return mInventory[index];
        }

        protected override void OnInit() { }
    }
}