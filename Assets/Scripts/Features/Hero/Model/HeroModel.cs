using System.Collections.Generic;
using Features.Combat.Define;
using QFramework;

namespace Features.Hero.Model
{
    public class HeroModel : AbstractModel, IHeroModel
    {
        public BindableProperty<int> Health { get; } = new();
        public BindableProperty<int> MaxHealth { get; } = new();
        public BindableProperty<bool> Invincible { get; } = new();
        public BindableProperty<int> CurSlotIndex { get; } = new();
        public BindableProperty<bool> IsFacingRight { get; } = new(true);
        public BindableProperty<int> Armor { get; } = new();

        public List<StatusModifier> Statuses { get; } = new();

        protected override void OnInit() { }
    }
}