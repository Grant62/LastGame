using Opsive.BehaviorDesigner.Runtime.Components;
using Opsive.BehaviorDesigner.Runtime.Groups;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Utility;
using Opsive.GraphDesigner.Runtime;
using Unity.Entities;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Systems
{
    /// <summary>
    ///     Specifies that the node is an object task which can specify the next child that should run.
    /// </summary>
    public interface ITaskObjectParentNode
    {
        /// <summary>
        ///     Returns the index of the next child that should run. Set to ushort.MaxValue to ignore.
        /// </summary>
        ushort NextChildIndex { get; }
    }

    /// <summary>
    ///     The DOTS data structure for the TaskObject class.
    /// </summary>
    public struct TaskObjectComponent : IBufferElementData
    {
        [Tooltip("The index of the task.")]
        public ushort Index;
    }

    /// <summary>
    ///     A DOTS flag indicating when an TaskObject node is active.
    /// </summary>
    public struct TaskObjectFlag : IComponentData, IEnableableComponent { }

    /// <summary>
    ///     Runs the TaskObject logic.
    /// </summary>
    [DisableAutoCreation]
    [UpdateInGroup(typeof(TraversalTaskSystemGroup), OrderLast = true)]
    public partial struct TaskObjectSystem : ISystem
    {
        /// <summary>
        ///     Updates the logic.
        /// </summary>
        /// <param name="state">The current state of the system.</param>
        private void OnUpdate(ref SystemState state)
        {
            // When the task is interrupted there is no callback which prevents Task.OnEnd from being called. Track the status within the referenced task object and if the status is different then
            // the task was aborted and OnEnd needs to be called.
            foreach ((DynamicBuffer<TaskObjectComponent> taskObjectComponents, DynamicBuffer<TaskComponent> taskComponents, Entity entity) in
                     SystemAPI.Query<DynamicBuffer<TaskObjectComponent>, DynamicBuffer<TaskComponent>>().WithAll<InterruptedFlag>().WithEntityAccess())
            {
                BehaviorTree behaviorTree = BehaviorTree.GetBehaviorTree(entity);
                if (behaviorTree == null)
                {
                    continue;
                }

                for (int i = 0; i < taskObjectComponents.Length; ++i)
                {
                    TaskComponent taskComponent = taskComponents[taskObjectComponents[i].Index];
                    if (taskComponent.Status == TaskStatus.Success || taskComponent.Status == TaskStatus.Failure)
                    {
                        Task task = behaviorTree.GetTask(taskObjectComponents[i].Index) as Task;
                        if (task.Status != taskComponent.Status)
                        {
                            task.OnEnd();
                            task.Status = taskComponent.Status;
                        }
                    }
                }
            }

            // Update the task objects.
            foreach ((DynamicBuffer<TaskObjectComponent> taskObjectComponents, DynamicBuffer<TaskComponent> taskComponents, DynamicBuffer<BranchComponent> branchComponents, Entity entity) in
                     SystemAPI.Query<DynamicBuffer<TaskObjectComponent>, DynamicBuffer<TaskComponent>, DynamicBuffer<BranchComponent>>().WithAll<TaskObjectFlag, EvaluateFlag>()
                         .WithEntityAccess())
            {
                BehaviorTree behaviorTree = BehaviorTree.GetBehaviorTree(entity);
                if (behaviorTree == null)
                {
                    continue;
                }

                for (int i = 0; i < taskObjectComponents.Length; ++i)
                {
                    TaskComponent taskComponent = taskComponents[taskObjectComponents[i].Index];
                    Task task = behaviorTree.GetTask(taskObjectComponents[i].Index) as Task;
                    if (taskComponent.Status == TaskStatus.Queued)
                    {
                        task.Status = taskComponent.Status = TaskStatus.Running;
                        DynamicBuffer<TaskComponent> buffer = taskComponents;
                        buffer[taskComponent.Index] = taskComponent;

                        task.OnStart();
                    }

                    if (taskComponent.Status != TaskStatus.Running)
                    {
                        continue;
                    }

                    TaskStatus status = task.OnUpdate();
                    // Update the status if has changed.
                    if (status != taskComponent.Status)
                    {
                        task.Status = taskComponent.Status = status;
                        DynamicBuffer<TaskComponent> buffer = taskComponents;
                        buffer[taskComponent.Index] = taskComponent;

                        // End the task if it is done running.
                        if (status != TaskStatus.Running)
                        {
                            task.OnEnd();

                            BranchComponent branchComponent = branchComponents[taskComponent.BranchIndex];
                            branchComponent.NextIndex = taskComponent.ParentIndex;
                            DynamicBuffer<BranchComponent> branchComponentBuffer = branchComponents;
                            branchComponentBuffer[taskComponent.BranchIndex] = branchComponent;
                        }
                    }

                    if (task is IParentNode && task is ITaskObjectParentNode taskObjectParentNode)
                    {
                        if (status == TaskStatus.Running)
                        {
                            // Parent object tasks do not have a direct way to set the next child. Use the ITaskObjectParentNode to switch the child task.
                            if (taskObjectParentNode.NextChildIndex != ushort.MaxValue && taskComponents[taskObjectParentNode.NextChildIndex].Status != TaskStatus.Running)
                            {
                                BranchComponent branchComponent = branchComponents[taskComponent.BranchIndex];
                                branchComponent.NextIndex = taskObjectParentNode.NextChildIndex;
                                DynamicBuffer<BranchComponent> branchComponentBuffer = branchComponents;
                                branchComponentBuffer[taskComponent.BranchIndex] = branchComponent;

                                TaskComponent nextTaskComponent = taskComponents[taskObjectParentNode.NextChildIndex];
                                nextTaskComponent.Status = TaskStatus.Queued;
                                DynamicBuffer<TaskComponent> taskComponentBuffer = taskComponents;
                                taskComponentBuffer[taskObjectParentNode.NextChildIndex] = nextTaskComponent;
                            }
                        }
                        else if (status == TaskStatus.Success || status == TaskStatus.Failure)
                        {
                            // An interrupt should occur if the parent returns a success or failure status before the children.
                            DynamicBuffer<TaskComponent> taskComponentBuffer = taskComponents;
                            int childCount = TraversalUtility.GetChildCount(taskComponent.Index, ref taskComponentBuffer);
                            DynamicBuffer<BranchComponent> branchComponentBuffer = branchComponents;
                            for (ushort j = (ushort)(taskComponent.Index + 1); j < taskComponent.Index + 1 + childCount; ++j)
                            {
                                TaskComponent childTaskComponent = taskComponents[j];
                                if (childTaskComponent.Status == TaskStatus.Running || childTaskComponent.Status == TaskStatus.Queued)
                                {
                                    childTaskComponent.Status = status;
                                    taskComponentBuffer[j] = childTaskComponent;

                                    BranchComponent branchComponent = branchComponents[childTaskComponent.BranchIndex];
                                    if (!SystemAPI.HasComponent<InterruptFlag>(entity))
                                    {
                                        ComponentUtility.AddInterruptComponents(behaviorTree.World.EntityManager, entity);
                                    }

                                    SystemAPI.SetComponentEnabled<InterruptedFlag>(entity, true);
                                    if (branchComponent.ActiveIndex == childTaskComponent.Index)
                                    {
                                        branchComponent.NextIndex = ushort.MaxValue;
                                        branchComponentBuffer[childTaskComponent.BranchIndex] = branchComponent;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     A DOTS tag indicating when an TaskObject node needs to be reevaluated.
    /// </summary>
    public struct TaskObjectReevaluateFlag : IComponentData, IEnableableComponent { }

    /// <summary>
    ///     Runs the TaskObject reevaluation logic.
    /// </summary>
    [DisableAutoCreation]
    public partial struct TaskObjectReevaluateSystem : ISystem
    {
        /// <summary>
        ///     Updates the reevaluation logic.
        /// </summary>
        /// <param name="state">The current state of the system.</param>
        private void OnUpdate(ref SystemState state)
        {
            foreach ((DynamicBuffer<TaskComponent> taskComponents, DynamicBuffer<TaskObjectComponent> taskObjectComponents, Entity entity) in
                     SystemAPI.Query<DynamicBuffer<TaskComponent>, DynamicBuffer<TaskObjectComponent>>().WithAll<TaskObjectReevaluateFlag, EvaluateFlag>().WithEntityAccess())
            {
                for (int i = 0; i < taskObjectComponents.Length; ++i)
                {
                    TaskObjectComponent taskObjectComponent = taskObjectComponents[i];
                    TaskComponent taskComponent = taskComponents[taskObjectComponent.Index];
                    if (!taskComponent.Reevaluate)
                    {
                        continue;
                    }

                    BehaviorTree behaviorTree = BehaviorTree.GetBehaviorTree(entity);
                    if (behaviorTree == null)
                    {
                        continue;
                    }

                    IConditionalReevaluation task = behaviorTree.GetTask(taskObjectComponent.Index) as IConditionalReevaluation;
                    TaskStatus status = task.OnReevaluateUpdate();
                    if (status != taskComponent.Status)
                    {
                        taskComponent.Status = status;
                        DynamicBuffer<TaskComponent> buffer = taskComponents;
                        buffer[taskComponent.Index] = taskComponent;
                    }
                }
            }
        }
    }
}
#endif