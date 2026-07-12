using System.Collections.Generic;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using QFramework;
using Services.ExcelTool;

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
            IBinaryDataMgr mgr = this.GetUtility<IBinaryDataMgr>();

            GameBalanceInfoContainer balance = mgr.GetTable<GameBalanceInfoContainer>();
            if (balance?.DataDic != null && balance.DataDic.TryGetValue(1, out GameBalanceInfo b))
            {
                InitialEnergy = b.InitialEnergy;
                CardsPerTurn = b.CardsPerTurn;
                MaxStepsPerLayer = b.MaxStepsPerLayer;
                MaxLayers = b.MaxLayers;
                ShortRestHealPercent = b.ShortRestHealPercent;
                ShortRestMaxCount = b.ShortRestMaxCount;
                WeakMultiplier = b.WeakMultiplier;
                VulnerableMultiplier = b.VulnerableMultiplier;
            }

            FloorClearGoldInfoContainer goldConfig = mgr.GetTable<FloorClearGoldInfoContainer>();
            if (goldConfig?.DataDic != null)
            {
                foreach (FloorClearGoldInfo info in goldConfig.DataDic.Values)
                    mFloorClearGold[info.Step] = info.Gold;
            }

            EntryInfoContainer container = mgr.GetTable<EntryInfoContainer>();
            if (container?.DataDic != null)
            {
                IReadOnlyDictionary<int, EntryInfo> dic = container.DataDic;
                if (dic.TryGetValue(6, out EntryInfo e))
                    SwordPathDamage = e.Value;
                if (dic.TryGetValue(10, out e))
                    LinkBlockPerSword = e.Value;
                if (dic.TryGetValue(11, out e))
                    SpinBaseDamage = e.Value;
                if (dic.TryGetValue(12, out e))
                    SpiritPathDamage = e.Value;
            }
        }

        public int GetFloorClearGold(int step)
        {
            return mFloorClearGold.TryGetValue(step, out int gold) ? gold : 0;
        }
    }
}