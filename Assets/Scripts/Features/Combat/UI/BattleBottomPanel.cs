using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Combat.UI
{
    public class BattleBottomPanel : UIPanel
    {
        [BoxGroup("子组件")]
        [SerializeField] private EndTurnBtn endTurnBtn;
        [BoxGroup("子组件")]
        [SerializeField] private PileEntry drawEntrance;
        [BoxGroup("子组件")]
        [SerializeField] private PileEntry discardEntrance;
        [BoxGroup("子组件")]
        [SerializeField] private PileEntry consumeEntrance;

        protected override void OnInit(IUIData uiData = null) { }

        protected override void OnClose() { }
    }
}