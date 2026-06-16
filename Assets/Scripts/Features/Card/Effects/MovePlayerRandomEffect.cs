using Core.Systems;
using Features.Combat.Targeting;
using Features.Combat.View.Board;
using Features.Hero.Model;

namespace Features.Card.Effects
{
    public class MovePlayerRandomEffect : Effect
    {
        public override void Execute(ITargetable[] targets, ITargetable caster)
        {
            IHeroModel hero = Ctx.HeroModel;
            BoardView board = Ctx.BoardAccess.Board;
            int oldSlot = hero.CurSlotIndex.Value;
            int slotCount = board.SlotCount;
            int targetSlot = Ctx.RandomSystem.Range(0, slotCount, RandomModuleIds.Combat);

            if (targetSlot == oldSlot)
                return;

            board.ShiftEnemies(oldSlot, targetSlot);
            hero.CurSlotIndex.Value = targetSlot;
        }
    }
}