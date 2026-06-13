using Core.Systems;
using Features.Combat.Event;
using Features.Combat.Targeting;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.View;
using Features.Hero.Model;
using Features.Sword.Model;
using QFramework;

namespace Features.Combat.System
{
    public class StatusTickSystem : AbstractSystem
    {
        private int mReactiveDestroyedCount;
        private int mReactivePrevCount;
        private bool mReactiveRegistered;

        protected override void OnInit()
        {
            this.RegisterEvent<PlayerTurnStartEvent>(OnPlayerTurnStart);
            this.RegisterEvent<PlayerTurnEndEvent>(OnPlayerTurnEnd);
            this.RegisterEvent<EnemyTurnEndEvent>(OnEnemyTurnEnd);
        }

        private void OnPlayerTurnStart(PlayerTurnStartEvent e)
        {
            ISwordModel sword = this.GetModel<ISwordModel>();
            IRandomSystem random = this.GetSystem<IRandomSystem>();

            if (sword.HasTurnStartSpiritSpawn)
            {
                SpawnRandomSpirit(sword, random);
            }

            if (sword.HasReactiveSpiritSpawn && !mReactiveRegistered)
            {
                mReactiveRegistered = true;
                mReactivePrevCount = sword.SpiritSwordSlots.Count;
                mReactiveDestroyedCount = 0;

                sword.OnSpiritSwordsChanged.Register(() =>
                {
                    int newCount = sword.SpiritSwordSlots.Count;
                    int destroyed = mReactivePrevCount - newCount;
                    if (destroyed > 0)
                        mReactiveDestroyedCount += destroyed;

                    while (mReactiveDestroyedCount >= 2)
                    {
                        mReactiveDestroyedCount -= 2;
                        SpawnRandomSpirit(sword, random);
                    }

                    mReactivePrevCount = newCount;
                });
            }
        }

        private void OnPlayerTurnEnd(PlayerTurnEndEvent e)
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            StatusHelper.TickStatuses(hero.Statuses);
            hero.Armor.Value = 0;

            ISwordModel sword = this.GetModel<ISwordModel>();
            sword.IsSpiritAttached = false;
        }

        private void OnEnemyTurnEnd(EnemyTurnEndEvent e)
        {
            BoardView board = this.GetUtility<IBoardAccess>().Board;

            foreach (EnemyView enemy in board.GetActiveEnemies())
                StatusHelper.TickStatuses(enemy.Statuses);
        }

        private static void SpawnRandomSpirit(ISwordModel sword, IRandomSystem random)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int slot = random.Range(0, 9, RandomModuleIds.Combat);
                if (!sword.SpiritSwordSlots.Contains(slot))
                {
                    sword.SpiritSwordSlots.Add(slot);
                    sword.OnSpiritSwordsChanged.Trigger();
                    return;
                }
            }
        }
    }
}