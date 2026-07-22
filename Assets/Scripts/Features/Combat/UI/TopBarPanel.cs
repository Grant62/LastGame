using Configuration.ExcelData.DataClass;
using Core.Architecture;
using DG.Tweening;
using Features.Card.Utility;
using Features.Combat.Event;
using Features.Combat.Interfaces;
using Features.Combat.System;
using Features.Hero.Model;
using Features.Potion.Command;
using Features.Potion.Model;
using Features.Resource.Model;
using Features.Run.Model;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat.UI
{
    public partial class TopBarPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelLabel;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Button[] potionSlots;
        [SerializeField] private PotionPopup potionPopup;

        private int mPendingThrowSlot = -1;
        private int mTargetingSlotIndex = -1;

        private void Start()
        {
            IHeroModel hero = GameMain.Interface.GetModel<IHeroModel>();

            hero.Health.RegisterWithInitValue(v => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            hero.MaxHealth.Register(_ => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            hero.Invincible.Register(_ => RefreshLifeLabel(hero))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IResourceModel resource = GameMain.Interface.GetModel<IResourceModel>();
            resource.Gold.RegisterWithInitValue(v => CoinLabel.text = $"{v}金")
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IRunModel run = GameMain.Interface.GetModel<IRunModel>();
            run.CurrentLayer.RegisterWithInitValue(_ => RefreshLevelLabel(run))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            IPotionModel potionModel = GameMain.Interface.GetModel<IPotionModel>();
            potionModel.OnInventoryChanged.Register(RefreshPotionSlots)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            GameMain.Interface.RegisterEvent<PotionThrowRequestedEvent>(OnPotionThrowRequested)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            for (int i = 0; i < potionSlots.Length; i++)
            {
                int capturedIndex = i;
                potionSlots[i].onClick.AddListener(() => OnPotionClicked(capturedIndex));
            }

            RefreshPotionSlots();
        }

        private void OnPotionClicked(int slotIndex)
        {
            IPotionModel model = GameMain.Interface.GetModel<IPotionModel>();
            PotionInfo potion = model.GetPotionAt(slotIndex);
            if (potion == null)
                return;

            Sprite icon = GameMain.Interface.GetUtility<ICardSpriteCache>().GetSprite(potion.Address);
            potionPopup.Show(
                potion,
                icon,
                () => { GameMain.Interface.SendCommand(new UsePotionCommand(slotIndex)); },
                () =>
                {
                    mPendingThrowSlot = slotIndex;
                    GameMain.Interface.SendEvent(new PotionThrowRequestedEvent { SlotIndex = slotIndex });
                },
                () => { model.RemoveAt(slotIndex); }
            );
        }

        private void OnPotionThrowRequested(PotionThrowRequestedEvent e)
        {
            mTargetingSlotIndex = e.SlotIndex;
        }

        private void Update()
        {
            if (mTargetingSlotIndex < 0)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                ITargetingSystem targeting = GameMain.Interface.GetSystem<ITargetingSystem>();
                ITargetable target = targeting.GetTargetAtMousePosition();
                if (target is IDamageable)
                {
                    GameMain.Interface.SendCommand(
                        new UsePotionCommand(mTargetingSlotIndex, target));
                    RefreshPotionSlots();
                }

                mTargetingSlotIndex = -1;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                mTargetingSlotIndex = -1;
            }
        }

        private void RefreshPotionSlots()
        {
            IPotionModel model = GameMain.Interface.GetModel<IPotionModel>();
            ICardSpriteCache spriteCache = GameMain.Interface.GetUtility<ICardSpriteCache>();
            for (int i = 0; i < potionSlots.Length; i++)
            {
                PotionInfo potion = model.GetPotionAt(i);
                Image image = potionSlots[i].GetComponent<Image>();
                if (potion != null)
                {
                    image.sprite = spriteCache.GetSprite(potion.Address);
                    image.color = Color.white;
                    image.enabled = true;
                }
                else
                {
                    image.sprite = null;
                    image.enabled = false;
                }
            }
        }

        private void RefreshLevelLabel(IRunModel run)
        {
            levelLabel.text = $"第{run.CurrentLayer.Value}层";
        }

        private void RefreshLifeLabel(IHeroModel hero)
        {
            LifeLabel.text = hero.Invincible.Value
                ? "\u221e"
                : $"{hero.Health.Value}/{hero.MaxHealth.Value}";

            float ratio = hero.Invincible.Value
                ? 1f
                : (float)hero.Health.Value / hero.MaxHealth.Value;

            healthBarFill.DOKill();
            healthBarFill.DOFillAmount(ratio, 0.3f);
        }
    }
}