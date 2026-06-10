using Features.Combat.Event;
using Features.Combat.Targeting;
using Features.Combat.UI.Board;
using Features.Hero.Model;
using QFramework;
using UnityEngine;

namespace Features.Combat.System
{
    public class StatusTickSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<PlayerTurnEndEvent>(OnPlayerTurnEnd);
            this.RegisterEvent<EnemyTurnEndEvent>(OnEnemyTurnEnd);
        }

        private void OnPlayerTurnEnd(PlayerTurnEndEvent e)
        {
            IHeroModel hero = this.GetModel<IHeroModel>();
            StatusHelper.TickStatuses(hero.Statuses);
        }

        private void OnEnemyTurnEnd(EnemyTurnEndEvent e)
        {
            BoardPanel board = Object.FindObjectOfType<BoardPanel>();
            if (board == null)
                return;

            foreach (EnemyUI enemy in board.EnemyViews)
            {
                if (enemy != null && enemy.isActiveAndEnabled)
                    StatusHelper.TickStatuses(enemy.Statuses);
            }
        }
    }
}