using Opsive.BehaviorDesigner.Runtime.Components;
using Opsive.BehaviorDesigner.Runtime.Groups;
using Opsive.GraphDesigner.Runtime;
using Opsive.Shared.Utility;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Events
{
    [AllowMultipleTypes]
    [NodeIcon("10ed9753a0870c84889dc42a7de397a8", "98f584ca47ddad64d9878314395ce160")]
    [Description("EventNode that is invoked when an interrupt occurs.")]
    public class OnInterrupt : IEventNode, IEventNodeEntityReceiver
    {
        [Tooltip("The index of the ITreeLogicNode that the IEventNode is connected to. ushort.MaxValue indicates no connection.")]
        [SerializeField] protected ushort m_ConnectedIndex;
        [Tooltip("The node that caused the interruption.")]
        [SerializeField]
        private ILogicNode m_InterruptionSource;

        public ushort ConnectedIndex { get => m_ConnectedIndex; set => m_ConnectedIndex = value; }

        /// <summary>
        ///     Adds the IBufferElementData to the entity.
        /// </summary>
        /// <param name="world">The world that the entity exists in.</param>
        /// <param name="entity">The entity that the IBufferElementData should be assigned to.</param>
        /// <param name="gameObject">The GameObject that the entity is attached to.</param>
        /// <param name="taskOffset">The offset between the connected index and the runtime index.</param>
        public void AddBufferElement(World world, Entity entity, GameObject gameObject, ushort taskOffset)
        {
            if (m_InterruptionSource == null || m_InterruptionSource.Index < 0)
            {
                Debug.LogError("Error: An Interruption Source task must be specified within the OnInterrupt node.");
                return;
            }

            DynamicBuffer<OnInterruptEventComponent> buffer;
            if (world.EntityManager.HasBuffer<OnInterruptEventComponent>(entity))
            {
                buffer = world.EntityManager.GetBuffer<OnInterruptEventComponent>(entity);
            }
            else
            {
                buffer = world.EntityManager.AddBuffer<OnInterruptEventComponent>(entity);
            }

            buffer.Add(new OnInterruptEventComponent
            {
                ConnectedIndex = (ushort)(m_ConnectedIndex - taskOffset),
                InterruptionSourceIndex = m_InterruptionSource.RuntimeIndex
            });

            InterruptTaskSystemGroup interruptSystemGroup = world.GetOrCreateSystemManaged<InterruptTaskSystemGroup>();
            interruptSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<OnInterruptSystem>());
        }

        /// <summary>
        ///     Clears the IBufferElementData from the entity.
        /// </summary>
        /// <param name="world">The world that the entity exists in.</param>
        /// <param name="entity">The entity that the IBufferElementData should be cleared from.</param>
        public void ClearBufferElement(World world, Entity entity)
        {
            DynamicBuffer<OnInterruptEventComponent> buffer;
            if (world.EntityManager.HasBuffer<OnInterruptEventComponent>(entity))
            {
                buffer = world.EntityManager.GetBuffer<OnInterruptEventComponent>(entity);
                buffer.Clear();
            }
        }
    }

    /// <summary>
    ///     The DOTS data structure for the OnInterrupt class.
    /// </summary>
    public struct OnInterruptEventComponent : IBufferElementData
    {
        [Tooltip("The index of the ILogicNode that the IEventNode is connected to.")]
        public ushort ConnectedIndex;
        [Tooltip("The index of the node that can invoke the interrupt.")]
        public ushort InterruptionSourceIndex;
    }

    /// <summary>
    ///     Processes any interrupts.
    /// </summary>
    [DisableAutoCreation]
    public partial struct OnInterruptSystem : ISystem
    {
        /// <summary>
        ///     Updates the logic.
        /// </summary>
        /// <param name="state">The current state of the system.</param>
        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            foreach ((DynamicBuffer<BranchComponent> branchComponents, DynamicBuffer<TaskComponent> taskComponents, DynamicBuffer<OnInterruptEventComponent> onInterruptEvents, Entity entity) in
                     SystemAPI.Query<DynamicBuffer<BranchComponent>, DynamicBuffer<TaskComponent>, DynamicBuffer<OnInterruptEventComponent>>().WithAll<InterruptFlag>().WithEntityAccess())
            {
                for (int i = 0; i < branchComponents.Length; ++i)
                {
                    BranchComponent branchComponent = branchComponents[i];
                    if (branchComponent.InterruptType != InterruptType.None)
                    {
                        // The branch is going to cause an interrupt. 
                        for (int j = 0; j < onInterruptEvents.Length; ++j)
                        {
                            OnInterruptEventComponent onInterruptEvent = onInterruptEvents[j];
                            if (branchComponent.ActiveIndex >= onInterruptEvent.InterruptionSourceIndex &&
                                branchComponent.ActiveIndex < taskComponents[onInterruptEvent.InterruptionSourceIndex].SiblingIndex)
                            {
                                // Trigger the callback.
                                TaskComponent startTask = taskComponents[onInterruptEvent.ConnectedIndex];
                                if (startTask.Status != TaskStatus.Queued && startTask.Status != TaskStatus.Running)
                                {
                                    startTask.Status = TaskStatus.Queued;
                                    DynamicBuffer<TaskComponent> taskComponentsBuffer = taskComponents;
                                    taskComponentsBuffer[onInterruptEvent.ConnectedIndex] = startTask;

                                    ComponentType activeTag = taskComponents[onInterruptEvent.ConnectedIndex].FlagComponentType;
                                    state.EntityManager.SetComponentEnabled(entity, activeTag, true);

                                    ushort connectedBranchIndex = taskComponents[onInterruptEvent.ConnectedIndex].BranchIndex;
                                    branchComponent = branchComponents[connectedBranchIndex];
                                    branchComponent.ActiveIndex = branchComponent.NextIndex = onInterruptEvent.ConnectedIndex;
                                    branchComponent.ActiveFlagComponentType = activeTag;
                                    DynamicBuffer<BranchComponent> branchComponentsBuffer = branchComponents;
                                    branchComponentsBuffer[connectedBranchIndex] = branchComponent;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif