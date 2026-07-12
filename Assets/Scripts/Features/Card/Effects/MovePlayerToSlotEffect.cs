using Features.Combat.Interfaces;
using Features.Combat.View.Board;
using Features.Hero.Model;

namespace Features.Card.Effects
{
    public class MovePlayerToSlotEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            int targetSlot = Ctx.SlotTargetIndex;
            if (targetSlot < 0)
                return;

            IHeroModel hero = Ctx.HeroModel;
            int oldSlot = hero.CurSlotIndex.Value;

            if (targetSlot == oldSlot)
                return;

            hero.CurSlotIndex.Value = targetSlot;

            BoardView board = Ctx.BoardAccess.Board;
            board.ShiftEnemies(oldSlot, targetSlot);
        }
    }
}