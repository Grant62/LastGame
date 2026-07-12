using System.Collections.Generic;

namespace Main.GM
{
    public static class GmHelpContent
    {
        public static IReadOnlyList<string> Lines { get; } = new List<string>
        {
            "=== GM 控制台帮助 ===",
            "提示: 指令不区分大小写，上下箭头键可翻阅历史指令",
            "help - 显示此帮助信息",
            "clear - 清空控制台",
            "hotkeys - 显示快捷键",
            "",
            "── 卡牌 ──",
            "givecard [id/名称] [数量] [hand/deck/discard/draw] - 给予指定卡牌",
            "removecard [id/名称] [hand/deck/discard/draw] - 移除指定卡牌",
            "draw [数量] - 抽牌",
            "discard [数量] - 从手牌弃牌",
            "keephand - 切换回合结束时是否保留手牌",
            "",
            "── 英雄 ──",
            "herohp [数值] - 设置英雄血量",
            "heroarmor [数值] - 设置英雄护甲",
            "addarmor [数值] - 增加英雄护甲",
            "killhero - 杀死英雄",
            "invincible - 切换英雄无敌状态",
            "",
            "── 敌人 ──",
            "kill [槽位 1~9] - 杀死指定格子的敌人",
            "killall - 消灭所有敌人",
            "skip - 跳过当前关卡，直接进入商店界面",
            "",
            "── 资源 ──",
            "energy [数值] - 设置能量上限并回满",
            "gold [数值] - 设置金币"
        };
    }
}