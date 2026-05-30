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
    [Description("Compares two Vector3 values.")]
    [Category("Math")]
    public class Vector3Comparison : Conditional
    {
        [Tooltip("The first Vector3.")]
        [SerializeField] protected SharedVariable<Vector3> m_Vector1;
        [Tooltip("The second Vector3.")]
        [SerializeField] protected SharedVariable<Vector3> m_Vector2;

        /// <summary>
        ///     Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            return m_Vector1.Value == m_Vector2.Value ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
#endif