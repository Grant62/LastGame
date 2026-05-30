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
    ///     Implements the UtilityEvaluator returning a SharedVariable value.
    /// </summary>
    [Description("Sets the utility value to the specified SharedVariable float value.")]
    public class UtilityVariableEvaluator : UtilityEvaluator
    {
        [Tooltip("The utility of the decorator.")]
        [SerializeField]
        private SharedVariable<float> m_Utility;

        /// <summary>
        ///     Returns the utility of the decorator. The higher the utility the more likely the task will run next.
        /// </summary>
        /// <returns>The utility of the decorator.</returns>
        public override float GetUtilityValue() { return m_Utility.Value; }
    }
}
#endif