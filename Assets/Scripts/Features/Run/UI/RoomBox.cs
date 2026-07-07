using System;
using Features.Run.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Run.UI
{
    public class RoomBox : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text stepLabel;
        [SerializeField] private TMP_Text bossPreviewText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text actionButtonLabel;
        [SerializeField] private Button shortRestButton;
        [SerializeField] private TMP_Text shortRestButtonLabel;
        [SerializeField] private GameObject shortRestTooltip;

        private const float CurrentScale = 1.1f;
        private const float NormalScale = 1f;

        public void Render(RoomPreviewData data)
        {
            stepLabel.text = $"Level {data.Layer}-{data.Step}\n{data.StepTypeText}";

            bool isCurrent = data.State == RoomBoxState.Current;
            bool isBoss = data.StepTypeText.Contains("Boss");

            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale = Vector3.one * (isCurrent ? CurrentScale : NormalScale);

            if (backgroundImage != null)
                backgroundImage.color = isCurrent ? new Color(0.2f, 0.2f, 0.3f, 1f) : new Color(0.1f, 0.1f, 0.12f, 1f);

            SetupActionButton(data);
            SetupShortRestButton(data);
            SetupBossPreview(data, isBoss);
        }

        private void SetupActionButton(RoomPreviewData data)
        {
            if (actionButton == null || actionButtonLabel == null)
                return;

            switch (data.State)
            {
                case RoomBoxState.Cleared:
                    actionButtonLabel.text = "已被击败";
                    actionButton.interactable = false;
                    actionButton.gameObject.SetActive(true);
                    break;
                case RoomBoxState.Rested:
                    actionButtonLabel.text = "已短休";
                    actionButton.interactable = false;
                    actionButton.gameObject.SetActive(true);
                    break;
                case RoomBoxState.Current:
                    actionButtonLabel.text = "迎战";
                    actionButton.interactable = true;
                    actionButton.gameObject.SetActive(true);
                    break;
                default:
                    actionButton.gameObject.SetActive(false);
                    break;
            }
        }

        private void SetupShortRestButton(RoomPreviewData data)
        {
            if (shortRestButton == null)
                return;

            if (!data.CanShortRest)
            {
                shortRestButton.gameObject.SetActive(false);
                return;
            }

            bool show = data.State == RoomBoxState.Current && data.ShortRestCount > 0;
            shortRestButton.gameObject.SetActive(show);
            if (show)
                shortRestButton.interactable = true;
        }

        private void SetupBossPreview(RoomPreviewData data, bool isBoss)
        {
            if (bossPreviewText == null)
                return;

            if (!string.IsNullOrEmpty(data.BossPreview))
            {
                bossPreviewText.text = data.BossPreview;
                bossPreviewText.gameObject.SetActive(true);
            }
            else
            {
                bossPreviewText.gameObject.SetActive(false);
            }
        }

        public void SetOnActionClick(Action onClick)
        {
            if (actionButton != null)
                actionButton.onClick.AddListener(() => onClick());
        }

        public void SetOnShortRestClick(Action onClick)
        {
            if (shortRestButton != null)
                shortRestButton.onClick.AddListener(() => onClick());
        }

        public void ShowShortRestTooltip()
        {
            if (shortRestTooltip != null)
                shortRestTooltip.SetActive(true);
        }

        public void HideShortRestTooltip()
        {
            if (shortRestTooltip != null)
                shortRestTooltip.SetActive(false);
        }
    }
}