using QFramework;
using UnityEngine;

namespace Features.Combat.Utility
{
    public interface IDamageTextSpawner : IUtility
    {
        void Spawn(int value, Vector2 screenPos, Color color);
        void Spawn(string text, Vector2 screenPos, Color color);
        void ClearAll();
    }
}