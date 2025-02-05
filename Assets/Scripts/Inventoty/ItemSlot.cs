using System;
using UnityEngine;

namespace MySampleEx
{
    [Serializable]
    public class ItemSlot
    {
        #region Variables
        public Item item;
        public int amount;

        public ItemType[] AllowedItems = new ItemType[0];    //슬롯에 넣을수 있는 아이템 타입 설정

        [NonSerialized]
        public InventoryObject parent;
        [NonSerialized]
        public GameObject slotUI;   //슬롯이 적용되는 UI 오브젝트

        [NonSerialized]
        public Action<ItemSlot> OnPreUpdate;    //슬롯에 아이템 내용이 적용되기 이전에 등록된 함수 호출
        [NonSerialized]
        public Action<ItemSlot> OnPostUpdate;   //슬롯에 아이템 내용이 적용된 후에 등록된 함수 호출

        //item 의 정보를 가지고 있는 ItemObject
        public ItemObject ItemObject
        {
            get
            {
                return item.id >= 0 ? parent.database.itemObjects[item.id] : null;
            }
        }
        #endregion

        //생성자
        public ItemSlot()
        {
            //아이템이 없는 슬롯 생성
            UpdateSlot(new Item(), 0);
        }

        public ItemSlot(Item item, int amount)
        {
            //매개변수로 들어온 아이템을 가진 슬롯 생성
            UpdateSlot(item, amount);
        }

        //슬롯에서 아이템 삭제
        public void RemoveItem()
        {
            UpdateSlot(new Item(), 0);
        }

        //아이템 수량 추가
        public void AddItemAmount(int value)
        {
            int addValue = amount + value;
            UpdateSlot(item, addValue);
        }

        //슬롯 업데이트
        public void UpdateSlot(Item item, int amount)
        {
            //빈슬롯 체크
            if(amount == 0)
            {
                item = new Item();
            }    

            OnPreUpdate?.Invoke(this);
            this.item = item;
            this.amount = amount;
            OnPostUpdate?.Invoke(this);
        }

        //슬롯에 아이템 장착이 가능여부 판단
        public bool CanPlaceInSlot(ItemObject itemObject)
        {
            if (AllowedItems.Length <= 0 || itemObject == null || itemObject.data.id <= -1)
            {
                return true;
            }

            //AllowedItem 체크
            foreach (var itemType in AllowedItems)
            {
                if(itemObject.type == itemType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
