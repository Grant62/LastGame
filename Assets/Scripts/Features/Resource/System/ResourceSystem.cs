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
            model.CurEnergy.Value = Mathf.Min(model.CurEnergy.Value + model.PerTurnIncome, model.MaxEnergy.Value);
        }

        public bool CanSpend(int amount)
        {
            return this.GetModel<IResourceModel>().CurEnergy.Value >= amount;
        }

        public void Spend(int amount)
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.CurEnergy.Value = Mathf.Max(0, model.CurEnergy.Value - amount);
        }

        public void Gain(int amount)
        {
            IResourceModel model = this.GetModel<IResourceModel>();
            model.CurEnergy.Value = Mathf.Min(model.CurEnergy.Value + amount, model.MaxEnergy.Value);
        }
    }
}