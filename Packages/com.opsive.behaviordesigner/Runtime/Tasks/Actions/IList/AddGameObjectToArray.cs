using System;
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
    [Description("Adds the GameObject to the array.")]
    [Category("Lists")]
    public class AddGameObjectToArray : TargetGameObjectAction
    {
        [Tooltip("The list of possible GameObjects.")]
        [RequireShared] [SerializeField] protected SharedVariable<GameObject[]> m_StoreResult;
        [Tooltip("Are duplicates allowed to be added?")]
        [SerializeField] protected SharedVariable<bool> m_AllowDuplicates = true;

        /// <summary>
        ///     Executes the task.
        /// </summary>
        /// <returns>The execution status of the task.</returns>
        public override TaskStatus OnUpdate()
        {
            if (!m_AllowDuplicates.Value && m_StoreResult.Value.Contains(m_ResolvedGameObject))
            {
                return TaskStatus.Failure;
            }

            GameObject[] array = m_StoreResult.Value;
            if (array == null)
            {
                array = new GameObject[1];
            }
            else
            {
                Array.Resize(ref array, array.Length + 1);
            }

            m_StoreResult.Value = array;
            return TaskStatus.Success;
        }
    }
}
#endif