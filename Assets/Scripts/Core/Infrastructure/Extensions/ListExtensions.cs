using System.Collections.Generic;
using UnityEngine;

namespace Core.Infrastructure.Extensions
{
    public static class ListExtensions
    {
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
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}