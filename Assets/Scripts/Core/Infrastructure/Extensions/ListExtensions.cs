using System;
using System.Collections.Generic;

namespace Core.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random sRng = new();

        public static T DrawOrdered<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            T t = list[0];
            list.RemoveAt(0);
            return t;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (list is not { Count: > 1 }) return;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = sRng.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}