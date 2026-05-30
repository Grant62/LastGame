using Opsive.BehaviorDesigner.Runtime.Systems;
using Opsive.GraphDesigner.Runtime;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Tasks.Composites
{
    /// <summary>
    ///     A TaskObject implementation of the Composite task.
    /// </summary>
    [NodeIcon("3afb3814c40717440b175b6fde4e73c2", "7fb12c74939f50b41b1679eb8f9e79ab")]
    public abstract class CompositeNode : Task, ITreeLogicNode, IParentNode, IComposite, ITaskObjectParentNode
    {
        [Tooltip("The index of the node.")]
        [SerializeField]
        private ushort m_Index;
        [Tooltip("The parent index of the node. ushort.MaxValue indicates no parent.")]
        [SerializeField]
        private ushort m_ParentIndex;
        [Tooltip("The sibling index of the node. ushort.MaxValue indicates no sibling.")]
        [SerializeField]
        private ushort m_SiblingIndex;

        public ushort Index { get => m_Index; set => m_Index = value; }

        public ushort ParentIndex { get => m_ParentIndex; set => m_ParentIndex = value; }

        public ushort SiblingIndex { get => m_SiblingIndex; set => m_SiblingIndex = value; }

        public ushort RuntimeIndex { get; set; }

        public virtual int MaxChildCount { get => int.MaxValue; }

        public virtual ushort NextChildIndex { get => (ushort)(RuntimeIndex + 1); }
    }
}
#endif