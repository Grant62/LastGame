using System;
using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Components;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Editor;
using Opsive.GraphDesigner.Editor.Events;
using Opsive.GraphDesigner.Runtime;
using Opsive.Shared.Editor.UIElements.Controls;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Editor.Controls.NodeViews
{
    /// <summary>
    ///     Implements TypeControlBase for the Wait type.
    /// </summary>
    [ControlType(typeof(Wait))]
    public class WaitNodeViewControl : TaskNodeViewControl
    {
        private BehaviorTree m_BehaviorTree;
        private ILogicNode m_Node;
        private ushort m_WaitComponentIndex = ushort.MaxValue;
        private ProgressBar m_ProgressBar;

        /// <summary>
        ///     Addes the UIElements for the specified runtime node to the editor Node within the graph.
        /// </summary>
        /// <param name="graphWindow">A reference to the GraphWindow.</param>
        /// <param name="parent">The parent UIElement that should contain the node UIElements.</param>
        /// <param name="node">The node that the control represents.</param>
        public override void AddNodeView(GraphWindow graphWindow, VisualElement parent, object node)
        {
            base.AddNodeView(graphWindow, parent, node);

            if (!Application.isPlaying)
            {
                return;
            }

            m_BehaviorTree = graphWindow.Graph as BehaviorTree;
            m_Node = node as ILogicNode;

            parent.RegisterCallback<AttachToPanelEvent>(c => { GraphEventHandler.RegisterEvent(GraphEventType.WindowUpdate, UpdateWaitProgress); });
            parent.RegisterCallback<DetachFromPanelEvent>(c => { GraphEventHandler.UnregisterEvent(GraphEventType.WindowUpdate, UpdateWaitProgress); });

            m_ProgressBar = new ProgressBar();
            parent.Add(m_ProgressBar);
        }

        /// <summary>
        ///     Updates the wait progress bar.
        /// </summary>
        private void UpdateWaitProgress()
        {
            if (m_BehaviorTree == null || m_BehaviorTree.Entity == Entity.Null || m_Node.RuntimeIndex == ushort.MaxValue)
            {
                m_ProgressBar.style.display = DisplayStyle.None;
                return;
            }

            DynamicBuffer<WaitComponent> waitComponents = m_BehaviorTree.World.EntityManager.GetBuffer<WaitComponent>(m_BehaviorTree.Entity);
            if (m_WaitComponentIndex == ushort.MaxValue)
            {
                // Find the corresponding index of the WaitComponent.
                for (int i = 0; i < waitComponents.Length; ++i)
                {
                    if (waitComponents[i].Index == m_Node.RuntimeIndex)
                    {
                        m_WaitComponentIndex = (ushort)i;
                        break;
                    }
                }

                if (m_WaitComponentIndex == ushort.MaxValue)
                {
                    return;
                }
            }

            WaitComponent waitComponent = waitComponents[m_WaitComponentIndex];
            if (waitComponent.PauseTime != 0)
            {
                return;
            }

            m_ProgressBar.highValue = (float)waitComponent.WaitDuration;

            DynamicBuffer<TaskComponent> taskComponents = m_BehaviorTree.World.EntityManager.GetBuffer<TaskComponent>(m_BehaviorTree.Entity);
            float elapsed = -1f;
            if (taskComponents[m_Node.RuntimeIndex].Status == TaskStatus.Running)
            {
                elapsed = Mathf.Clamp(Time.time - (float)waitComponent.StartTime, 0, (float)waitComponent.WaitDuration);
                m_ProgressBar.value = elapsed;
            }
            else if (taskComponents[m_Node.RuntimeIndex].Status == TaskStatus.Success)
            {
                elapsed = (float)waitComponent.WaitDuration;
                m_ProgressBar.value = elapsed;
            }
            else if (taskComponents[m_Node.RuntimeIndex].Status == TaskStatus.Inactive)
            {
                m_ProgressBar.value = 0;
            }

            m_ProgressBar.title = (elapsed >= 0 ? Math.Round(elapsed, 2) + "/" : string.Empty) + Math.Round(waitComponent.WaitDuration, 2) + "s";
            if (m_ProgressBar.style.display == DisplayStyle.None)
            {
                m_ProgressBar.style.display = DisplayStyle.Flex;
            }
        }
    }
}
#endif