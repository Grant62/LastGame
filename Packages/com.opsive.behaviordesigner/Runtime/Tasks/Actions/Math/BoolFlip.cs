using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.Utility;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Actions.Math
{
    [Description("Flips the value of the boolean.")]
    [Category("Math")]
    public class BoolFlip : Action
    {
        [Tooltip("The bool that should be flipped.")]
        [SerializeField] protected SharedVariable<bool> m_Bool;

        /// <summary>
        ///     Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            m_Bool.Value = !m_Bool.Value;
            return TaskStatus.Success;
        }
    }
}
#endif