using System.Collections.Generic;
using QFramework;

namespace Features.Sword.Model
{
    public interface ISwordModel : IModel
    {
        BindableProperty<int> CurSlotIndex { get; }

        BindableProperty<bool> IsSpinning { get; }
        BindableProperty<int> SpinDamage { get; }
        bool KeepSpinningOnMove { get; set; }
        bool SpinHitsAdjacent { get; set; }
        BindableProperty<bool> IsSpiritAttached { get; }
        bool HasTurnStartSpiritSpawn { get; set; }
        bool HasReactiveSpiritSpawn { get; set; }
        bool SuppressPathDamage { get; set; }
        bool SpinAffectsSpirits { get; set; }
        bool LinkAlwaysPenetrate { get; set; }
        bool RecallSpiritsOnSwordMove { get; set; }
        int RecallSpiritsDamagePerSpirit { get; set; }
        int RecallTargetSlot { get; set; }
        bool LastLinkPenetrated { get; set; }
        bool IsRecalling { get; set; }
        int CustomPathDamage { get; set; }

        List<int> SpiritSwordSlots { get; }
        EasyEvent OnSpiritSwordsChanged { get; }
    }
}