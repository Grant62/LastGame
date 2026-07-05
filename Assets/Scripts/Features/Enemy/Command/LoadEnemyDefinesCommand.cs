using Configuration.ExcelData.Container;
using Configuration.ExcelData.DataClass;
using Features.Enemy.Define;
using Features.Enemy.Model;
using QFramework;
using Services.ExcelTool;

namespace Features.Enemy.Command
{
    public class LoadEnemyDefinesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IEnemyDefineModel model = this.GetModel<IEnemyDefineModel>();
            if (model.Defines.Count > 0)
                return;

            EnemyInfoContainer container = this.GetUtility<IBinaryDataMgr>().GetTable<EnemyInfoContainer>();
            if (container == null)
                return;

            foreach (EnemyInfo info in container.DataDic.Values)
            {
                model.Register(new EnemyDefine
                {
                    MonsterId = info.EnemyId,
                    MaxHealth = info.HP,
                    Damage = info.Damage
                });
            }
        }
    }
}