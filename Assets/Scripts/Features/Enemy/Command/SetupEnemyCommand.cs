using Features.Enemy.Model;
using QFramework;

namespace Features.Enemy.Command
{
    public class SetupEnemyCommand : AbstractCommand
    {
        private readonly EnemyDefine mDefine;

        public SetupEnemyCommand(EnemyDefine define)
        {
            mDefine = define;
        }

        protected override void OnExecute()
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();
            model.MonsterId.Value = mDefine.MonsterId;
            model.MaxHealth.Value = mDefine.MaxHealth;
            model.Health.Value = mDefine.MaxHealth;
            model.Damage.Value = mDefine.Damage;
        }
    }
}