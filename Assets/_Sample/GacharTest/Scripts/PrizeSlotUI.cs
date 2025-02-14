using System;
using TMPro;
using UnityEngine;

namespace MySampleEx
{
    [Serializable]
    public class Prize
    {
        public string name;
        public string item;
    }

    public class PrizeSlotUI : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI itemText;
        #endregion

        public void SetNameText(string name)
        {
            nameText.text = name;
            itemText.text = "";
        }

        public void SetItemText(string item)
        {
            itemText.text = item;
        }
    }
}