using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.Model
{
    public interface ITargetingModel : IModel
    {
        BindableProperty<bool> IsTargeting { get; }
        Vector3 StartPosition { get; set; }
    }
}