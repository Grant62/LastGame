using Features.Combat.Targeting.Event;
using QFramework;
using UnityEngine;

namespace Features.Combat.Targeting.System
{
    public class TargetingSystem : AbstractSystem, ITargetingSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<TargetingStartedEvent>(OnTargetingStarted);
            this.RegisterEvent<TargetingEndedEvent>(OnTargetingEnded);
        }

        public ITargetable GetTargetAtPosition(Vector3 position)
        {
            return this.GetUtility<ITargetSelector>().GetTargetAtPosition(position);
        }

        public ITargetable GetTargetAtMousePosition()
        {
            return this.GetUtility<ITargetSelector>().GetTargetAtMousePosition();
        }

        public ITargetable[] ResolveTargets(TargetType type, ITargetable caster)
        {
            ITargetResolver resolver = this.GetUtility<ITargetResolver>();
            return resolver.Resolve(type, caster);
        }

        private void OnTargetingStarted(TargetingStartedEvent e)
        {
            IArrowDisplay arrow = this.GetUtility<IArrowDisplay>();
            arrow.Show(e.StartPosition);
        }

        private void OnTargetingEnded(TargetingEndedEvent e)
        {
            this.GetUtility<IArrowDisplay>().Hide();
            this.GetUtility<ICursorDisplay>().Hide();
        }
    }
}