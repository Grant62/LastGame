using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using QFramework;
using Services;
using Services.ExcelTool;

namespace Features.Combat.Model
{
    public class GameConfigModel : AbstractModel, IGameConfigModel
    {
        public int SwordPathDamage { get; private set; } = 4;
        public int SpiritPathDamage { get; private set; } = 7;
        public int SpinBaseDamage { get; private set; } = 3;
        public int LinkBlockPerSword { get; private set; } = 8;

        protected override void OnInit()
        {
            IBinaryDataMgr mgr = this.GetUtility<IBinaryDataMgr>();
            EntryInfoContainer container = mgr.GetTable<EntryInfoContainer>();
            if (container?.DataDic == null)
                return;

            foreach (EntryInfo entry in container.DataDic.Values)
            {
                switch (entry.Id)
                {
                    case 5:
                    case 6:
                        SwordPathDamage = ParseDamage(entry.Desc, 4);
                        break;
                    case 12:
                        SpiritPathDamage = ParseDamage(entry.Desc, 7);
                        break;
                    case 11:
                        SpinBaseDamage = ParseDamage(entry.Desc, 3);
                        break;
                    case 10:
                        LinkBlockPerSword = ParseBlock(entry.Desc, 8);
                        break;
                }
            }
        }

        private static int ParseDamage(string desc, int fallback)
        {
            int v = CardDescriptionParser.ParseDamage(desc);
            return v > 0 ? v : fallback;
        }

        private static int ParseBlock(string desc, int fallback)
        {
            int v = CardDescriptionParser.ParseBlock(desc);
            return v > 0 ? v : fallback;
        }
    }
}