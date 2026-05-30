using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.Utility;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals.Math
{
    [Description("Compares two boolean values.")]
    [Category("Math")]
    public class BoolComparison : Conditional
    {
        [Tooltip("The first boolean.")]
        [SerializeField] protected SharedVariable<bool> m_Bool1;
        [Tooltip("The second boolean.")]
        [SerializeField] protected SharedVariable<bool> m_Bool2;

        /// <summary>
        ///     Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            return m_Bool1.Value == m_Bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
#endif