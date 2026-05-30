using Core.Architecture;
using Features.Combat.Targeting.Model;
using Features.Combat.Targeting.Query;
using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.View
{
    public class TargetingInputController : MonoBehaviour, IController
    {
        private ITargetingModel mModel;
        private ICursorDisplay mCursorDisplay;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Awake()
        {
            mModel = this.GetModel<ITargetingModel>();
            mCursorDisplay = this.GetUtility<ICursorDisplay>();
        }

        private void Update()
        {
            if (!mModel.IsTargeting.Value)
                return;

            UpdateCursorPosition();
        }

        private void UpdateCursorPosition()
        {
            ITargetable target = this.SendQuery(new GetTargetAtMouseQuery());

            if (target is { IsValidTarget: true })
                mCursorDisplay.ShowAt(target.Position + Vector3.up * 2.35f);
            else
                mCursorDisplay.Hide();
        }
    }
}