using System;
using System.Linq;
using Features.Card.Define;
using Features.Card.Model;
using Features.Card.System;
using Features.Hero.Model;
using Main.GM.Command;
using QFramework;

namespace Main.GM
{
    public class GmCommandExecutor
    {
        public void Execute(string command, IController controller, Action<string> onOutput)
        {
            string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string cmd = parts[0].ToLowerInvariant();

            switch (cmd)
            {
                case "help":
                    foreach (string line in GmHelpContent.Lines)
                        onOutput(line);
                    break;

                case "clear":
                    onOutput("__CLEAR__");
                    break;

                case "givecard":
                    HandleGiveCard(parts, controller, onOutput);
                    break;

                case "removecard":
                    HandleRemoveCard(parts, controller, onOutput);
                    break;

                case "draw":
                    HandleDraw(parts, controller, onOutput);
                    break;

                case "discard":
                    HandleDiscard(parts, controller, onOutput);
                    break;

                case "herohp":
                    HandleHeroHp(parts, onOutput, controller);
                    break;

                case "heroarmor":
                    HandleHeroArmor(parts, onOutput, controller);
                    break;

                case "addarmor":
                    HandleAddArmor(parts, onOutput, controller);
                    break;

                case "killhero":
                    controller.SendCommand<KillHeroCommand>();
                    onOutput("已杀死英雄");
                    break;

                case "energy":
                    HandleEnergy(parts, onOutput, controller);
                    break;

                case "gold":
                    HandleGold(parts, onOutput, controller);
                    break;

                case "keephand":
                    controller.SendCommand<ToggleKeepHandCommand>();
                    bool isKept = controller.GetModel<ICardModel>().KeepHandOnTurnEnd;
                    onOutput($"回合结束保留手牌: {(isKept ? "开启" : "关闭")}");
                    break;

                case "killall":
                    controller.SendCommand<KillAllEnemiesCommand>();
                    onOutput("已消灭所有敌人");
                    break;

                case "kill":
                    HandleKillSlot(parts, onOutput, controller);
                    break;

                case "hotkeys":
                    onOutput("=== 快捷键 ===");
                    onOutput("~ - 打开/关闭 GM 控制台");
                    break;

                default:
                    onOutput($"未知指令: {cmd}，输入 'help' 查看可用指令");
                    break;
            }
        }

        private static void HandleGiveCard(string[] parts, IController controller, Action<string> onOutput)
        {
            if (parts.Length < 4)
            {
                onOutput("参数不足！用法: givecard [id/名称] [数量] [hand/deck/discard/draw]");
                return;
            }

            string idOrName = parts[1];
            if (!int.TryParse(parts[2], out int count) || count <= 0)
            {
                onOutput("参数错误！数量必须为正整数");
                return;
            }

            string pileStr = parts[3].ToLowerInvariant();
            if (!IsValidPile(pileStr))
            {
                onOutput("参数错误！目标堆: hand/deck/discard/draw");
                return;
            }

            if (int.TryParse(idOrName, out int cardId))
            {
                if (!CardExists(cardId, controller))
                {
                    onOutput($"未找到卡牌 ID: {cardId}");
                    return;
                }

                controller.SendCommand(new GiveCardCommand(cardId, count, pileStr));
                onOutput($"已给予 {count} 张卡牌 ID:{cardId} 到 {pileStr}");
            }
            else
            {
                ICardDefineModel defineModel = controller.GetModel<ICardDefineModel>();
                CardDefine match = defineModel.Defines.Values
                    .FirstOrDefault(d => d.Name.Contains(idOrName, StringComparison.OrdinalIgnoreCase));

                if (match.Name == null)
                {
                    onOutput($"未找到卡牌: {idOrName}");
                    return;
                }

                controller.SendCommand(new GiveCardCommand(match.Id, count, pileStr));
                onOutput($"已给予 {count} 张卡牌 {match.Name}(ID:{match.Id}) 到 {pileStr}");
            }
        }

        private static void HandleRemoveCard(string[] parts, IController controller, Action<string> onOutput)
        {
            if (parts.Length < 3)
            {
                onOutput("参数不足！用法: removecard [id/名称] [hand/deck/discard/draw]");
                return;
            }

            string idOrName = parts[1];
            string pileStr = parts[2].ToLowerInvariant();
            if (!IsValidPile(pileStr))
            {
                onOutput("参数错误！目标堆: hand/deck/discard/draw");
                return;
            }

            if (int.TryParse(idOrName, out int cardId))
            {
                controller.SendCommand(new RemoveCardCommand(cardId, pileStr));
                onOutput($"已从 {pileStr} 移除卡牌 ID:{cardId}");
            }
            else
            {
                ICardDefineModel defineModel = controller.GetModel<ICardDefineModel>();
                CardDefine match = defineModel.Defines.Values
                    .FirstOrDefault(d => d.Name.Contains(idOrName, StringComparison.OrdinalIgnoreCase));

                if (match.Name == null)
                {
                    onOutput($"未找到卡牌: {idOrName}");
                    return;
                }

                controller.SendCommand(new RemoveCardCommand(match.Id, pileStr));
                onOutput($"已从 {pileStr} 移除卡牌 {match.Name}(ID:{match.Id})");
            }
        }

        private static void HandleDraw(string[] parts, IController controller, Action<string> onOutput)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int count) || count <= 0)
            {
                onOutput("参数错误！用法: draw [数量]");
                return;
            }

            controller.GetSystem<ICardSystem>().DrawCards(count);
            onOutput($"已触发抽牌 {count} 张");
        }

        private static void HandleDiscard(string[] parts, IController controller, Action<string> onOutput)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int count) || count <= 0)
            {
                onOutput("参数错误！用法: discard [数量]");
                return;
            }

            ICardModel cardModel = controller.GetModel<ICardModel>();
            ICardSystem cardSystem = controller.GetSystem<ICardSystem>();

            if (cardModel.HandPile.Count == 0)
            {
                onOutput("手牌为空，无法弃牌");
                return;
            }

            int actualCount = Math.Min(count, cardModel.HandPile.Count);
            for (int i = 0; i < actualCount; i++)
                cardSystem.DiscardFromHand(cardModel.HandPile[0]);

            onOutput($"已弃掉 {actualCount} 张手牌");
        }

        private static void HandleHeroHp(string[] parts, Action<string> onOutput, IController controller)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int hp) || hp <= 0)
            {
                onOutput("参数错误！用法: herohp [数值]");
                return;
            }

            controller.SendCommand(new SetHeroHealthCommand(hp));
            onOutput($"英雄血量已设置为: {hp}");
        }

        private static void HandleHeroArmor(string[] parts, Action<string> onOutput, IController controller)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int armor) || armor < 0)
            {
                onOutput("参数错误！用法: heroarmor [数值]");
                return;
            }

            controller.SendCommand(new SetHeroArmorCommand(armor));
            onOutput($"英雄护甲已设置为: {armor}");
        }

        private static void HandleAddArmor(string[] parts, Action<string> onOutput, IController controller)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int amount))
            {
                onOutput("参数错误！用法: addarmor [数值]");
                return;
            }

            controller.SendCommand(new AddHeroArmorCommand(amount));
            int currentArmor = controller.GetModel<IHeroModel>().Armor.Value;
            onOutput($"英雄护甲 +{amount} (当前: {currentArmor})");
        }

        private static void HandleEnergy(string[] parts, Action<string> onOutput, IController controller)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int energy) || energy < 0)
            {
                onOutput("参数错误！用法: energy [数值]");
                return;
            }

            controller.SendCommand(new SetEnergyCommand(energy));
            onOutput($"当前能量已设置为: {energy}");
        }

        private static void HandleGold(string[] parts, Action<string> onOutput, IController controller)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int gold) || gold < 0)
            {
                onOutput("参数错误！用法: gold [数值]");
                return;
            }

            controller.SendCommand(new SetGoldCommand(gold));
            onOutput($"金币已设置为: {gold}");
        }

        private static bool IsValidPile(string pile)
        {
            return pile is "hand" or "deck" or "discard" or "draw";
        }

        private static void HandleKillSlot(string[] parts, Action<string> onOutput, IController controller)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int slot) || slot < 1 || slot > 9)
            {
                onOutput("参数错误！用法: kill [槽位 1~9]");
                return;
            }

            int slotIndex = slot - 1;
            controller.SendCommand(new KillEnemyAtSlotCommand(slotIndex));
            onOutput($"已消灭槽位 {slot} 的敌人");
        }

        private static bool CardExists(int cardId, IController controller)
        {
            return controller.GetModel<ICardDefineModel>().TryGet(cardId, out _);
        }
    }
}