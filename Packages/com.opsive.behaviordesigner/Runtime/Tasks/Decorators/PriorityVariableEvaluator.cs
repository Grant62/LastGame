using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.Utility;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Decorators
{
    /// <summary>
    ///     Implements the PriorityEvaluator returning a SharedVariable value.
    /// </summary>
    [Description("Sets the priority value to the specified SharedVariable float value.")]
    public class PriorityVariableEvaluator : PriorityEvaluator
    {
        [Tooltip("The priority of the decorator.")]
        [SerializeField]
        private SharedVariable<float> m_Priority;

        /// <summary>
        ///     Returns the priority of the decorator. The higher the priority the more likely the task will run next.
        /// </summary>
        /// <returns>The priority of the decorator.</returns>
        public override float GetPriorityValue() { return m_Priority.Value; }
    }
}
#endif