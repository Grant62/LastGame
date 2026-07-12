using System;
using Core.Infrastructure;
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

            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale = Vector3.one * (isCurrent ? CurrentScale : NormalScale);

            backgroundImage.color = isCurrent ? GameColors.RoomCurrent : GameColors.RoomNonCurrent;

            SetupActionButton(data);
            SetupShortRestButton(data);
            SetupBossPreview(data);
        }

        private void SetupActionButton(RoomPreviewData data)
        {
            switch (data.State)
            {
                case RoomBoxState.Cleared:
                    actionButtonLabel.text = "已被击败";
                    actionButton.interactable = false;
                    actionButton.gameObject.SetActive(true);
                    break;
                case RoomBoxState.Skipped:
                    actionButtonLabel.text = "已跳过";
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

        private void SetupBossPreview(RoomPreviewData data)
        {
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
            actionButton.onClick.AddListener(() => onClick());
        }

        public void SetOnShortRestClick(Action onClick)
        {
            shortRestButton.onClick.AddListener(() => onClick());
        }

        public void ShowShortRestTooltip()
        {
            shortRestTooltip.SetActive(true);
        }

        public void HideShortRestTooltip()
        {
            shortRestTooltip.SetActive(false);
        }
    }
}