using System.Collections.Generic;
using Features.Configuration.Model;
using QFramework;

namespace Features.Combat.Model
{
    public class GameConfigModel : AbstractModel, IGameConfigModel
    {
        private readonly Dictionary<int, int> mFloorClearGold = new();

        public int SwordPathDamage { get; private set; } = 4;
        public int SpiritPathDamage { get; private set; } = 7;
        public int SpinBaseDamage { get; private set; } = 3;
        public int LinkBlockPerSword { get; private set; } = 8;

        public int InitialEnergy { get; private set; } = 3;
        public int CardsPerTurn { get; private set; } = 5;
        public int MaxStepsPerLayer { get; private set; } = 3;
        public int MaxLayers { get; private set; } = 8;
        public float ShortRestHealPercent { get; private set; } = 0.25f;
        public int ShortRestMaxCount { get; private set; } = 2;
        public float WeakMultiplier { get; private set; } = 0.75f;
        public float VulnerableMultiplier { get; private set; } = 1.25f;

        protected override void OnInit()
        {
            cfg.Tables tables = this.GetUtility<ILubanDataModel>().Tables;

            var balanceList = tables.TbGameBalanceInfo.DataList;
            if (balanceList.Count > 0)
            {
                cfg.GameBalanceInfo b = balanceList[0];
                InitialEnergy = b.InitialEnergy;
                CardsPerTurn = b.CardsPerTurn;
                MaxStepsPerLayer = b.MaxStepsPerLayer;
                MaxLayers = b.MaxLayers;
                ShortRestHealPercent = b.ShortRestHealPercent;
                ShortRestMaxCount = b.ShortRestMaxCount;
                WeakMultiplier = b.WeakMultiplier;
                VulnerableMultiplier = b.VulnerableMultiplier;
            }

            foreach (cfg.FloorClearGoldInfo info in tables.TbFloorClearGoldInfo.DataList)
                mFloorClearGold[info.Step] = info.Gold;

            var entryList = tables.TbEntryInfo.DataList;
            foreach (cfg.EntryInfo e in entryList)
            {
                if (e.Id == 6)
                    SwordPathDamage = e.Value;
                if (e.Id == 10)
                    LinkBlockPerSword = e.Value;
                if (e.Id == 11)
                    SpinBaseDamage = e.Value;
                if (e.Id == 12)
                    SpiritPathDamage = e.Value;
            }
        }

        public int GetFloorClearGold(int step)
        {
            return mFloorClearGold.TryGetValue(step, out int gold) ? gold : 0;
        }
    }
}