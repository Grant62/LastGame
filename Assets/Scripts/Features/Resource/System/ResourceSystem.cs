using Features.Combat.Event;
using Features.Resource.Model;
using QFramework;
using UnityEngine;

namespace Features.Resource.System
{
    public class ResourceSystem : AbstractSystem, IResourceSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<PlayerTurnStartEvent>(OnPlayerTurnStart);
        }

        private void OnPlayerTurnStart(PlayerTurnStartEvent e)
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.Current.Value = Mathf.Min(model.Current.Value + model.PerTurnIncome, model.Max.Value);
        }

        public bool CanSpend(int amount)
        {
            return this.GetModel<IResourceModel>().Current.Value >= amount;
        }

        public void Spend(int amount)
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.Current.Value = Mathf.Max(0, model.Current.Value - amount);
        }

        public void Gain(int amount)
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.Current.Value = Mathf.Min(model.Current.Value + amount, model.Max.Value);
        }
    }
}