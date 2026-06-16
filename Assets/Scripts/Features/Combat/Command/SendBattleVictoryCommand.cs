using Features.Combat.Event;
using QFramework;

namespace Features.Combat.Command
{
    public class SendBattleVictoryCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<BattleVictoryEvent>();
        }
    }
}