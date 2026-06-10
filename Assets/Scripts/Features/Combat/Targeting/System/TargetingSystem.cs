using Features.Combat.Event;
using Features.Combat.Targeting.Event;
using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.System
{
    public class TargetingSystem : AbstractSystem, ITargetingSystem
    {
        private bool mIsTargeting;
        private IArrowDisplay mArrow;
        private ICursorDisplay mCursor;
        private ITargetSelector mTargetSelector;
        private ITargetResolver mTargetResolver;
        private IUnRegister mOnUpdateUnregister;

        protected override void OnInit()
        {
            this.RegisterEvent<TargetingStartedEvent>(OnTargetingStarted);
            this.RegisterEvent<TargetingEndedEvent>(OnTargetingEnded);
            this.RegisterEvent<GameReadyEvent>(OnGameReady);
        }

        private void OnGameReady(GameReadyEvent e)
        {
            mArrow = this.GetUtility<IArrowDisplay>();
            mCursor = this.GetUtility<ICursorDisplay>();
            mTargetSelector = this.GetUtility<ITargetSelector>();
            mTargetResolver = this.GetUtility<ITargetResolver>();
        }

        public void StartUpdate()
        {
            mOnUpdateUnregister = ActionKit.OnUpdate.Register(OnUpdate);
        }

        public void StopUpdate()
        {
            mOnUpdateUnregister?.UnRegister();
            mOnUpdateUnregister = null;
        }

        public ITargetable GetTargetAtPosition(Vector3 position)
        {
            return mTargetSelector.GetTargetAtPosition(position);
        }

        public ITargetable GetTargetAtMousePosition()
        {
            return mTargetSelector.GetTargetAtMousePosition();
        }

        public ITargetable[] ResolveTargets(TargetType type, ITargetable caster)
        {
            return mTargetResolver.Resolve(type, caster);
        }

        private void OnTargetingStarted(TargetingStartedEvent e)
        {
            mIsTargeting = true;
            mArrow.Show(e.StartPosition);
            StartUpdate();
        }

        private void OnTargetingEnded(TargetingEndedEvent e)
        {
            mIsTargeting = false;
            mArrow.Hide();
            mCursor.Hide();
            StopUpdate();
        }

        private void OnUpdate()
        {
            if (!mIsTargeting)
                return;

            ITargetable target = mTargetSelector.GetTargetAtMousePosition();
            if (target != null)
                mCursor.ShowAt(target.Position);
            else
                mCursor.Hide();

            mArrow.UpdateMouse(Input.mousePosition);
        }
    }
}