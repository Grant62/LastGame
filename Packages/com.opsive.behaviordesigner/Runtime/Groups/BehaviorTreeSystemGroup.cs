using Opsive.BehaviorDesigner.Runtime.Systems;
using Unity.Entities;
using UnityEngine;

#if GRAPH_DESIGNER
/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Runtime.Groups
{
    /// <summary>
    ///     System group which runs all of the behavior tree systems.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial class BehaviorTreeSystemGroup : ComponentSystemGroup
    {
        public bool Alive { get; private set; }

        /// <summary>
        ///     Disable the group if there are no behavior trees.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (BehaviorTree.BehaviorTreeCount > 0)
            {
                return;
            }

            World.NoAllocReadOnlyCollection<World> worlds = World.All;
            for (int i = 0; i < worlds.Count; ++i)
            {
                BehaviorTreeSystemGroup systemGroup = worlds[i].GetExistingSystemManaged<BehaviorTreeSystemGroup>();
                if (systemGroup == null)
                {
                    continue;
                }

                systemGroup.Enabled = false;
            }
        }

        /// <summary>
        ///     The group has been created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            Alive = true;
        }

        /// <summary>
        ///     Update the system group.
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();

            // Stop running if all trees have completed.
            SystemHandle systemHandle = World.GetExistingSystem<DetermineEvaluationSystem>();
            DetermineEvaluationSystem system = EntityManager.WorldUnmanaged.GetUnsafeSystemRef<DetermineEvaluationSystem>(systemHandle);
            if (!system.Active)
            {
                Enabled = false;
            }
        }

        /// <summary>
        ///     The group has been destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Alive = false;
        }
    }
}
#endif