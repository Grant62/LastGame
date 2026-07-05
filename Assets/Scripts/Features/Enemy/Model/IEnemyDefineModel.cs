using System.Collections.Generic;
using Features.Enemy.Define;
using QFramework;

namespace Features.Enemy.Model
{
    public interface IEnemyDefineModel : IModel
    {
        IReadOnlyDictionary<int, EnemyDefine> Defines { get; }

        void Register(EnemyDefine define);

        bool TryGet(int enemyId, out EnemyDefine define);

        EnemyDefine Get(int enemyId);
    }
}