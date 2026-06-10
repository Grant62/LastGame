using System.Collections.Generic;

namespace Features.Combat.Targeting
{
    public enum StatusType
    {
        Weak,
        Vulnerable
    }

    public class StatusModifier
    {
        public StatusType Type { get; }
        public int Stacks { get; set; }

        public StatusModifier(StatusType type, int stacks)
        {
            Type = type;
            Stacks = stacks;
        }
    }

    public static class StatusHelper
    {
        public static void ApplyStatus(List<StatusModifier> statuses, StatusType type, int stacks)
        {
            StatusModifier existing = statuses.Find(s => s.Type == type);
            if (existing != null)
            {
                existing.Stacks += stacks;
            }
            else
            {
                statuses.Add(new StatusModifier(type, stacks));
            }
        }

        public static void TickStatuses(List<StatusModifier> statuses)
        {
            for (int i = statuses.Count - 1; i >= 0; i--)
            {
                statuses[i].Stacks--;
                if (statuses[i].Stacks <= 0)
                    statuses.RemoveAt(i);
            }
        }

        public static bool HasStatus(List<StatusModifier> statuses, StatusType type)
        {
            return statuses.Exists(s => s.Type == type && s.Stacks > 0);
        }
    }
}