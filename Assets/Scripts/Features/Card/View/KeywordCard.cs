using TMPro;
using UnityEngine;

namespace Features.Card.View
{
    public class KeywordCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;

        public void Setup(string name, string desc)
        {
            nameText.text = name;
            descText.text = desc;
        }

        public RectTransform DescRectTransform => descText.rectTransform;
        public TMP_Text DescText => descText;
    }
}