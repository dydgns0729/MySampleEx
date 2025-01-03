using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
namespace MySampleEx
{
    /// <summary>
    /// Scriptable 오브젝트의 정보를 받아와서 UI에 제공해주는 MonoBehaviour스크립트 파일
    /// </summary>
    public class DrawCard : MonoBehaviour
    {
        public Card card;

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        
        public TextMeshProUGUI manaText;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI attackText;

        public Image artImage;

        private void Start()
        {

            UpdateCard();
        }

        private void UpdateCard()
        {
            nameText.text = card.name;
            descriptionText.text = card.description;
            manaText.text = card.mana.ToString();
            healthText.text = card.health.ToString();
            attackText.text = card.attack.ToString();

            artImage.sprite = card.artImage;

        }
    }
}