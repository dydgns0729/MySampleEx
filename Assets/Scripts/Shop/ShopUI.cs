using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MySampleEx
{
    public class ShopUI : InventoryUI
    {
        #region Variables
        public GameObject[] staticSlots;        //아이템 슬롯 오브젝트       
        public ItemObject[] itemObject;         //npc가 판매하는 아이템 목록

        public GameObject buyButton;            //구매 버튼
        private UIManager uIManager;

        public GameObject adButton;             //광고 버튼
        #endregion

        public override void CreateSlots()
        {
            slotUIs = new Dictionary<GameObject, ItemSlot>();
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject slotGo = staticSlots[i];

                //가져온 슬롯 오브젝트의 이벤트 트리거에 이벤트 등록
                AddEvent(slotGo, EventTriggerType.PointerClick, delegate { OnClick(slotGo); });

                inventoryObject.Slots[i].slotUI = slotGo;
                slotUIs.Add(slotGo, inventoryObject.Slots[i]);

                //npc가 판매하는 아이템 목록 슬롯에 셋팅
                Item shopItem = itemObject[i].CreateItem();
                slotUIs[slotGo].UpdateSlot(shopItem, 1);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (uIManager == null)
            {
                uIManager = UIManager.Instance;
            }
#if AD_MODE
            adButton.SetActive(true);
#endif
        }

        public override void OpenInventoryUI(Action closeMethod)
        {
            if (closeMethod != null)
                OnCloseInventoyUI += closeMethod;

            //Shop 셋팅

        }

        public override void UpdateSelectSlot(GameObject go)
        {
            base.UpdateSelectSlot(go);

            if (selectSlotObject == null)
            {
                buyButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                buyButton.GetComponent<Button>().interactable = true;
            }
        }

        public void BuyItem()
        {
            if (selectSlotObject == null)
                return;

            int price = slotUIs[selectSlotObject].ItemObject.shopPrice;

            if(uIManager.EnoughGold(price))
            {
                Item newItem = slotUIs[selectSlotObject].ItemObject.CreateItem();
                if(uIManager.AddItemInventory(newItem, 1))
                {
                    uIManager.UseGold(price);
                    UpdateSelectSlot(null);
                }
            }
            else
            {
                Debug.Log("돈이 부족합니다");
            }
        }

        public void ShowAd()
        {
#if AD_MODE
            //타이머 - 일정시간동안 광고를 보지 못하도록 한다

            AdManager.Instance.ShowRewardAd();
#endif
        }
    }
}
