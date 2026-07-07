using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.Combat.Targeting;
using Features.Combat.Utility;
using Features.Combat.View.Board;
using Features.Enemy.Data;
using Features.Enemy.Define;
using Features.Enemy.Event;
using Features.Enemy.Model;
using Features.Enemy.View;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Enemy.System
{
    public interface IEnemyAISystem : ISystem
    {
        void CalculateIntents(int heroSlot);

        UniTask MovePhase(int heroSlot);

        UniTask AttackPhase(int heroSlot);

        UniTask ExecuteCachedActions();
    }

    public class EnemyAISystem : AbstractSystem, IEnemyAISystem
    {
        protected override void OnInit() { }

        public void CalculateIntents(int heroSlot)
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();
            model.CachedIntents.Clear();

            foreach (EnemyRuntimeData enemy in model.ActiveEnemies)
            {
                if (enemy.HP <= 0)
                    continue;

                (EnemyIntentType intent, _) = DecideAction(enemy, heroSlot);

                if (intent == EnemyIntentType.Move)
                {
                    int nextSlot = enemy.SlotIndex + (enemy.SlotIndex < heroSlot ? 1 : -1);
                    if (Math.Abs(nextSlot - heroSlot) == 1)
                        intent = EnemyIntentType.MoveAttack;
                }

                enemy.CurrentIntent = intent;
                model.CachedIntents[enemy.SlotIndex] = intent;
            }
        }

        public async UniTask MovePhase(int heroSlot)
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();
            BoardView board = this.GetUtility<IBoardAccess>().Board;

            HashSet<int> occupied = new();
            List<EnemyRuntimeData> mobileEnemies = new();

            foreach (EnemyRuntimeData enemy in model.ActiveEnemies)
            {
                if (enemy.HP <= 0)
                    continue;

                occupied.Add(enemy.SlotIndex);

                (EnemyIntentType intent, _) = DecideAction(enemy, heroSlot);
                if (intent == EnemyIntentType.Move)
                    mobileEnemies.Add(enemy);
            }

            mobileEnemies.Sort((a, b) =>
                Math.Abs(a.SlotIndex - heroSlot).CompareTo(Math.Abs(b.SlotIndex - heroSlot)));

            List<(int from, int to, bool faceRight)> moves = new();

            foreach (EnemyRuntimeData enemy in mobileEnemies)
            {
                (EnemyIntentType intent, int direction) = DecideAction(enemy, heroSlot);
                if (intent != EnemyIntentType.Move && intent != EnemyIntentType.MoveAttack)
                    continue;

                int target = enemy.SlotIndex + direction;
                if (target < 0 || target > 8)
                    continue;

                if (occupied.Contains(target))
                    continue;

                occupied.Remove(enemy.SlotIndex);
                occupied.Add(target);

                bool faceRight = heroSlot > enemy.SlotIndex;
                moves.Add((enemy.SlotIndex, target, faceRight));
            }

            foreach ((int from, int to, bool faceRight) in moves)
            {
                EnemyView view = board.GetEnemyAtSlot(from);
                EnemyRuntimeData data = model.GetEnemyAtSlot(from);
                if (view != null && data != null)
                {
                    data.IsFacingRight = faceRight;
                    view.SetFacing(faceRight);
                }
            }

            List<UniTask> moveTasks = new();
            foreach ((int from, int to, bool _) in moves)
            {
                board.MoveEnemy(from, to);
                this.SendEvent(new EnemyMovedEvent { FromSlotIndex = from, NewSlotIndex = to });
                moveTasks.Add(UniTask.Delay(250));
            }

            if (moveTasks.Count > 0)
                await UniTask.WhenAll(moveTasks);

            foreach ((int from, int to, bool _) in moves)
            {
                EnemyRuntimeData data = model.GetEnemyAtSlot(from);
                if (data != null)
                    data.SlotIndex = to;
            }
        }

        public async UniTask AttackPhase(int heroSlot)
        {
            IEnemyModel model = this.GetModel<IEnemyModel>();
            BoardView board = this.GetUtility<IBoardAccess>().Board;

            List<int> attackers = new();

            foreach (EnemyRuntimeData enemy in model.ActiveEnemies)
            {
                if (enemy.HP <= 0)
                    continue;

                (EnemyIntentType intent, _) = DecideAction(enemy, heroSlot);

                if (intent == EnemyIntentType.Attack)
                {
                    attackers.Add(enemy.SlotIndex);
                }
            }

            foreach (int slot in attackers)
            {
                EnemyRuntimeData data = model.GetEnemyAtSlot(slot);
                EnemyView view = board.GetEnemyAtSlot(slot);
                if (data == null || view == null || data.HP <= 0)
                    continue;

                IHeroModel hero = this.GetModel<IHeroModel>();

                int damage = data.Damage;
                if (StatusHelper.HasStatus(hero.Statuses, StatusType.Weak))
                    damage = Mathf.FloorToInt(damage * 0.75f);

                int remaining = damage;
                if (hero.Armor.Value > 0)
                {
                    int absorbed = Mathf.Min(hero.Armor.Value, remaining);
                    hero.Armor.Value -= absorbed;
                    remaining -= absorbed;
                }

                if (remaining > 0 && hero.Invincible.Value)
                    remaining = 0;

                if (remaining > 0)
                    hero.Health.Value -= remaining;

                await UniTask.Delay(400);

                if (hero.Health.Value <= 0)
                    break;
            }
        }

        public async UniTask ExecuteCachedActions()
        {
            int heroSlot = this.GetModel<IHeroModel>().CurSlotIndex.Value;
            await MovePhase(heroSlot);
            await AttackPhase(heroSlot);
        }

        private static (EnemyIntentType intent, int direction) DecideAction(EnemyRuntimeData enemy, int heroSlot)
        {
            int slot = enemy.SlotIndex;

            if (Math.Abs(slot - heroSlot) == 1)
                return (EnemyIntentType.Attack, 0);

            int dir = slot < heroSlot ? 1 : -1;
            int next = slot + dir;

            if (next < 0 || next > 8)
                return (EnemyIntentType.None, 0);

            return (EnemyIntentType.Move, dir);
        }
    }
}