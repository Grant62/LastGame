using System.Collections.Generic;
using Features.Enemy.Define;
using QFramework;

namespace Features.Enemy.Model
{
    public class EnemyDefineModel : AbstractModel, IEnemyDefineModel
    {
        private readonly Dictionary<int, EnemyDefine> mDefines = new();

        public IReadOnlyDictionary<int, EnemyDefine> Defines
        {
            get => mDefines;
        }

        protected override void OnInit() { }

        public void Register(EnemyDefine define)
        {
            mDefines[define.MonsterId] = define;
        }

        public bool TryGet(int enemyId, out EnemyDefine define)
        {
            return mDefines.TryGetValue(enemyId, out define);
        }

        public EnemyDefine Get(int enemyId)
        {
            return mDefines[enemyId];
        }
    }
}