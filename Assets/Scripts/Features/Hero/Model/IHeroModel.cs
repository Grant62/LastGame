using System.Collections.Generic;
using Features.Combat.Targeting;
using QFramework;

namespace Features.Hero.Model
{
    public interface IHeroModel : IModel
    {
        BindableProperty<int> Health { get; }
        BindableProperty<int> MaxHealth { get; }
        BindableProperty<bool> Invincible { get; }
        BindableProperty<int> CurSlotIndex { get; }
        BindableProperty<bool> IsFacingRight { get; }

        List<StatusModifier> Statuses { get; }
    }
}