using System.Collections.Generic;
using UnityEngine;

namespace Core.Infrastructure
{
    public static class GameColors
    {
        public static readonly Color RarityCommon = new(0.8f, 0.898f, 0.306f);
        public static readonly Color RarityUncommon = new(0.31f, 0.765f, 0.969f);
        public static readonly Color RarityEpic = new(1f, 0.596f, 0f);

        public static readonly Color RoomCurrent = new(0.357f, 0.659f, 0.627f);
        public static readonly Color RoomNonCurrent = new(0.239f, 0.137f, 0.322f);

        private static readonly Dictionary<string, string> sRarityColorTags = new()
        {
            { "普通", $"<color=#{ColorUtility.ToHtmlStringRGB(RarityCommon)}>普通</color>" },
            { "罕见", $"<color=#{ColorUtility.ToHtmlStringRGB(RarityUncommon)}>罕见</color>" },
            { "史诗", $"<color=#{ColorUtility.ToHtmlStringRGB(RarityEpic)}>史诗</color>" }
        };

        public static string ColorizeRarity(string text)
        {
            foreach (KeyValuePair<string, string> kv in sRarityColorTags)
                text = text.Replace(kv.Key, kv.Value);
            return text;
        }
    }
}