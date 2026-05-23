using Features.Card.Define;
using QFramework;

namespace Features.Card.Command
{
    public class LoadCardDefinesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ICardDefineModel model = this.GetModel<ICardDefineModel>();

            if (model.Defines.Count > 0)
                return;

            /*
             *  方案 A：从 JSON 加载
             *
             *  TextAsset json = Resources.Load<TextAsset>("CardDefines");
             *  CardDefine[] list = JsonUtility.FromJson<CardDefineList>(json.text).Defines;
             *  foreach (CardDefine d in list) model.Register(d);
             */

            /*
             *  方案 B：从 Excel 二进制加载
             *
             *  Dictionary<int, CardDefine> table = BinaryDataMgr.Load<CardDefineContainer>().DataDic;
             *  foreach (CardDefine d in table.Values) model.Register(d);
             */

            // 方案 C：硬编码（开发期）
            model.Register(new CardDefine { Id = 1, Name = "砍击", Cost = 1, Damage = 8, Desc = "造成8点伤害" });
            model.Register(new CardDefine { Id = 2, Name = "重斩", Cost = 2, Damage = 15, Desc = "造成15点伤害" });
            model.Register(new CardDefine { Id = 3, Name = "治疗", Cost = 1, Heal = 8, Desc = "回复8点生命" });
        }
    }
}