using System;
using Configuration.ExcelData.DataClass;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat.UI
{
    public class PotionPopup : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;
        [SerializeField] private Button useBtn;
        [SerializeField] private TMP_Text useBtnLabel;
        [SerializeField] private Button discardBtn;

        private Action mOnUse;
        private Action mOnThrow;
        private Action mOnDiscard;

        private void Awake()
        {
            canvas.enabled = false;
            discardBtn.onClick.AddListener(() =>
            {
                mOnDiscard?.Invoke();
                Hide();
            });
        }

        public void Show(PotionInfo potion, Sprite icon, Action onUse, Action onThrow, Action onDiscard)
        {
            nameText.text = potion.Name;
            descText.text = potion.Desc;
            iconImage.sprite = icon;
            mOnUse = onUse;
            mOnThrow = onThrow;
            mOnDiscard = onDiscard;

            bool needsTarget = potion.EffectType == "Damage";
            useBtnLabel.text = needsTarget ? "投掷" : "使用";
            useBtn.onClick.RemoveAllListeners();
            useBtn.onClick.AddListener(() =>
            {
                if (needsTarget)
                    mOnThrow?.Invoke();
                else
                    mOnUse?.Invoke();
                Hide();
            });

            canvas.enabled = true;
        }

        private void Hide()
        {
            canvas.enabled = false;
        }
    }
}