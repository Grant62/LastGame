using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Combat.View
{
    public class ShieldView : MonoBehaviour
    {
        [SerializeField] private Image shieldIcon;
        [SerializeField] private TextMeshProUGUI shieldText;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetArmor(int armor)
        {
            bool hasArmor = armor > 0;
            gameObject.SetActive(hasArmor);
            if (hasArmor)
                shieldText.text = armor.ToString();
        }
    }
}
