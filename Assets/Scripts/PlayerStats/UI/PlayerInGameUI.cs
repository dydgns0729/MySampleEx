using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MySampleEx
{
    public class PlayerInGameUI : MonoBehaviour
    {
        #region Variables
        public StatsObject statsObject;

        public Image healthBar;
        public Image manaBar;

        public TextMeshProUGUI levelText;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI goldText;
        #endregion

        private void OnEnable()
        {
            statsObject.OnChagnedStats += OnChangedStats;
        }

        private void OnDisable()
        {
            statsObject.OnChagnedStats -= OnChangedStats;
        }

        private void Start()
        {
            OnChangedStats(statsObject);
        }

        private void OnChangedStats(StatsObject statsObject)
        {
            healthBar.fillAmount = statsObject.HealthPercentage;
            manaBar.fillAmount = statsObject.ManaPercentage;

            levelText.text = statsObject.Level.ToString();
            expText.text = statsObject.Exp.ToString() + " / " + statsObject.GetExpForLevelup(statsObject.Level).ToString();
            goldText.text = statsObject.Gold.ToString();
        }
    }
}
