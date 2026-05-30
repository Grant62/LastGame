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
    [Description("Set the integer value.")]
    [Category("Math")]
    public class SetInt : Action
    {
        [Tooltip("The int value to set.")]
        [SerializeField] protected SharedVariable<int> m_Value;
        [Tooltip("The variable that should be set.")]
        [RequireShared] [SerializeField] protected SharedVariable<int> m_StoreResult;

        /// <summary>
        ///     Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            m_StoreResult.Value = m_Value.Value;
            return TaskStatus.Success;
        }
    }
}
#endif