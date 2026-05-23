using Configuration.Model;
using QFramework;

namespace Configuration.Command
{
    public class InitDataTableCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IDataTableModel model = this.GetModel<IDataTableModel>();

            /*
             *  示例：加载 Excel 二进制表
             *
             *  model.RegisterTable(BinaryDataMgr.Load<MonsterConfigContainer>().DataDic);
             *  model.RegisterTable(BinaryDataMgr.Load<CardConfigContainer>().DataDic);
             *  model.RegisterTable(BinaryDataMgr.Load<RoomConfigContainer>().DataDic);
             */

            model.MarkInitialized();
        }
    }
}
