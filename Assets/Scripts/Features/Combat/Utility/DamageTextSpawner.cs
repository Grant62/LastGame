using System.Collections.Generic;
using Presentation.Effects;
using QFramework;
using UnityEngine;

namespace Features.Combat.Utility
{
    public class DamageTextSpawner : IDamageTextSpawner
    {
        private readonly SimpleObjectPool<DamageTextUI> mPool;
        private readonly Transform mPoolRoot;
        private readonly Transform mCanvasTransform;
        private readonly HashSet<DamageTextUI> mActiveTexts = new();

        public DamageTextSpawner(DamageTextUI prefab, Transform canvasTransform)
        {
            mCanvasTransform = canvasTransform;

            GameObject root = new("[Pool] DamageText");
            root.SetActive(false);
            mPoolRoot = root.transform;

            mPool = new SimpleObjectPool<DamageTextUI>(
                () => Object.Instantiate(prefab, mPoolRoot, false),
                null,
                10
            );
        }

        public void Spawn(int value, Vector2 screenPos, Color color)
        {
            Spawn($"-{value}", screenPos, color);
        }

        public void Spawn(string text, Vector2 screenPos, Color color)
        {
            DamageTextUI ui = mPool.Allocate();
            ui.transform.SetParent(mCanvasTransform, false);
            ui.gameObject.SetActive(true);
            mActiveTexts.Add(ui);
            ui.Play(text, color, screenPos, () =>
            {
                mActiveTexts.Remove(ui);
                ui.transform.SetParent(mPoolRoot, false);
                mPool.Recycle(ui);
            });
        }

        public void ClearAll()
        {
            foreach (DamageTextUI text in mActiveTexts)
            {
                if (text != null)
                {
                    text.gameObject.SetActive(false);
                    text.transform.SetParent(mPoolRoot, false);
                }
            }

            mActiveTexts.Clear();
        }

        public void Dispose()
        {
            ClearAll();
            Object.Destroy(mPoolRoot.gameObject);
        }
    }
}