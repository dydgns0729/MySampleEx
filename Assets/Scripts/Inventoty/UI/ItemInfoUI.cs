using UnityEngine;
using TMPro;

namespace MySampleEx
{
    /// <summary>
    /// 아이템 정보 보여주기
    /// </summary>
    public class ItemInfoUI : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescription;

        //아이템 속성 텍스트
        #endregion

        public void SetItemInfoUI(ItemSlot itemSlot)
        {
            ItemObject itemObejct = itemSlot.ItemObject;

            itemName.text = itemObejct.name;
            itemDescription.text = itemObejct.description;

            //속성값 설정
        }
    }
}
