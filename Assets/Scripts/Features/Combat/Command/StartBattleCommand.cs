using Features.Combat.System;
using QFramework;

namespace Features.Combat.Command
{
    public class StartBattleCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ITurnSystem turnSystem = this.GetSystem<ITurnSystem>();
            turnSystem.StartBattle();
        }
    }
}