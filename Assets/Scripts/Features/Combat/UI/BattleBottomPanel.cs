using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Combat.UI
{
    public class BattleBottomPanel : MonoBehaviour
    {
        [BoxGroup("子组件")]
        [SerializeField] private EndTurnBtn endTurnBtn;
        [BoxGroup("子组件")]
        [SerializeField] private PileEntry drawEntrance;
        [BoxGroup("子组件")]
        [SerializeField] private PileEntry discardEntrance;
        [BoxGroup("子组件")]
        [SerializeField] private PileEntry consumeEntrance;
    }
}