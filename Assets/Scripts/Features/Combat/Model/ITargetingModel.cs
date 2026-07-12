using QFramework;
using UnityEngine;

namespace Features.Combat.Model
{
    public interface ITargetingModel : IModel
    {
        BindableProperty<bool> IsTargeting { get; }
        Vector3 StartPosition { get; set; }
    }
}