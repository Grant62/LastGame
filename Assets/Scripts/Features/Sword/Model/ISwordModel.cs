using System.Collections.Generic;
using QFramework;

namespace Features.Sword.Model
{
    public interface ISwordModel : IModel
    {
        BindableProperty<int> CurSlotIndex { get; }
        BindableProperty<bool> IsSummoned { get; }

        BindableProperty<bool> IsSpinning { get; }
        BindableProperty<int> SpinDamage { get; }
        bool KeepSpinningOnMove { get; set; }
        bool SpinHitsAdjacent { get; set; }

        List<int> SpiritSwordSlots { get; }
        EasyEvent OnSpiritSwordsChanged { get; }
    }
}