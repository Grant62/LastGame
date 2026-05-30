using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.Utility;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Actions.UnityObjects
{
    [Description("Set the GameObject value.")]
    [Category("Unity")]
    public class SetGameObject : TargetGameObjectAction
    {
        [Tooltip("The variable that should be set.")]
        [RequireShared] [SerializeField] protected SharedVariable<GameObject> m_StoreResult;

        /// <summary>
        ///     Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            InitializeTarget();

            m_StoreResult.Value = m_ResolvedGameObject;
            return TaskStatus.Success;
        }
    }
}
#endif