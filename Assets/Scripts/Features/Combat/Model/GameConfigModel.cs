using System.Text.RegularExpressions;
using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using QFramework;
using Services.ExcelTool;

namespace Features.Combat.Model
{
    public class GameConfigModel : AbstractModel, IGameConfigModel
    {
        private static readonly Regex ValueRegex = new(
            @"(造成|获得|施加|恢复)(\d+)点?【?(伤害|护甲|能量|生命值)】?");

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
                        SwordPathDamage = ParseValue(entry.Desc, "伤害", 4);
                        break;
                    case 12:
                        SpiritPathDamage = ParseValue(entry.Desc, "伤害", 7);
                        break;
                    case 11:
                        SpinBaseDamage = ParseValue(entry.Desc, "伤害", 3);
                        break;
                    case 10:
                        LinkBlockPerSword = ParseValue(entry.Desc, "护甲", 8);
                        break;
                }
            }
        }

        private static int ParseValue(string desc, string type, int fallback)
        {
            if (string.IsNullOrEmpty(desc))
                return fallback;

            Match match = ValueRegex.Match(desc);
            if (match.Success && match.Groups[3].Value == type && int.TryParse(match.Groups[2].Value, out int result))
                return result;

            return fallback;
        }
    }
}