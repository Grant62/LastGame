using Core.SceneManagement.Event;
using Features.Combat.Targeting.Event;
using Features.Combat.Utility;
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
            this.RegisterEvent<RoomReadyEvent>(OnRoomReady);
        }

        private void OnRoomReady(RoomReadyEvent @event)
        {
            mArrow = this.GetUtility<IArrowDisplay>();
            mCursor = this.GetUtility<ICursorDisplay>();
            mTargetSelector = this.GetUtility<ITargetSelector>();
            mTargetResolver = this.GetUtility<ITargetResolver>();
        }

        private void StartUpdate()
        {
            mOnUpdateUnregister = ActionKit.OnUpdate.Register(OnUpdate);
        }

        private void StopUpdate()
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

        private void OnTargetingStarted(TargetingStartedEvent @event)
        {
            mIsTargeting = true;
            mArrow.Show(@event.StartPosition);
            StartUpdate();
        }

        private void OnTargetingEnded(TargetingEndedEvent @event)
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