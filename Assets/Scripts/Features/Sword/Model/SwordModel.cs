using System.Collections.Generic;
using QFramework;

namespace Features.Sword.Model
{
    public class SwordModel : AbstractModel, ISwordModel
    {
        public BindableProperty<int> CurSlotIndex { get; } = new(-1);

        public BindableProperty<bool> IsSpinning { get; } = new();
        public BindableProperty<int> SpinDamage { get; } = new();
        public bool KeepSpinningOnMove { get; set; }
        public bool SpinHitsAdjacent { get; set; }
        public bool IsSpiritAttached { get; set; }
        public bool HasTurnStartSpiritSpawn { get; set; }
        public bool HasReactiveSpiritSpawn { get; set; }
        public bool SuppressPathDamage { get; set; }

        public List<int> SpiritSwordSlots { get; } = new();
        public EasyEvent OnSpiritSwordsChanged { get; } = new();

        protected override void OnInit() { }
    }
}