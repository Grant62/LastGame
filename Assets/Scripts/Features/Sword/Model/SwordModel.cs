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
        public BindableProperty<bool> IsSpiritAttached { get; } = new();
        public bool HasTurnStartSpiritSpawn { get; set; }
        public bool HasReactiveSpiritSpawn { get; set; }
        public bool SuppressPathDamage { get; set; }
        public bool SpinAffectsSpirits { get; set; }
        public bool LinkAlwaysPenetrate { get; set; }
        public bool RecallSpiritsOnSwordMove { get; set; }
        public bool LastLinkPenetrated { get; set; }
        public bool IsRecalling { get; set; }
        public int CustomPathDamage { get; set; }

        public List<int> SpiritSwordSlots { get; } = new();
        public EasyEvent OnSpiritSwordsChanged { get; } = new();

        protected override void OnInit() { }
    }
}