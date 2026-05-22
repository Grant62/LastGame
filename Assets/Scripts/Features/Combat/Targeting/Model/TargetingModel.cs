using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.Model
{
    public class TargetingModel : AbstractModel, ITargetingModel
    {
        public BindableProperty<bool> IsTargeting { get; } = new();
        public Vector3 StartPosition { get; set; }

        protected override void OnInit() { }
    }
}