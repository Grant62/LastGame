using Features.Configuration.Model;
using Features.Enemy.Define;
using Features.Enemy.Model;
using QFramework;

namespace Features.Enemy.Command
{
    public class LoadEnemyDefinesCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IEnemyDefineModel model = this.GetModel<IEnemyDefineModel>();
            if (model.Defines.Count > 0)
                return;

            cfg.TbEnemyInfo table = this.GetUtility<ILubanDataModel>().Tables.TbEnemyInfo;

            foreach (cfg.EnemyInfo info in table.DataList)
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