using Features.Combat.Event;
using QFramework;

namespace Features.Combat.Command
{
    public class SendBattleDefeatCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<BattleDefeatEvent>();
        }
    }
}