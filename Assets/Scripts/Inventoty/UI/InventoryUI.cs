using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MySampleEx
{
    /// <summary>
    /// 인벤토리 UI 부모 (추상)클래스
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public abstract class InventoryUI : MonoBehaviour
    {
        #region Variables
        public InventoryObject inventoryObject;

        public Dictionary<GameObject, ItemSlot> slotUIs = new Dictionary<GameObject, ItemSlot>();

        //슬롯 선택기능
        protected GameObject selectSlotObject = null;      //현재 선택된 슬롯 오브젝트
        public Action<GameObject> OnUpdateSelectSlot;

        //퀘스트 진행 종료시 실행될 이벤트
        public Action OnCloseInventoyUI;
        #endregion

        private void Awake()
        {
            CreateSlots();

            //inventoryObject Slots 설정
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].parent = inventoryObject;
                //아이템 내용 변경시 호출되는 UI 함수 등록
                inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;      
            }

            //이벤트 트리거 이벤트 등록
            AddEvent(this.gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(this.gameObject);});
            AddEvent(this.gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(this.gameObject); });
        }

        protected virtual void OnEnable()
        {
            OnCloseInventoyUI = null;
        }

        protected virtual void Start()
        {
            //ui 슬롯 갱신
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].UpdateSlot(inventoryObject.Slots[i].item, inventoryObject.Slots[i].amount);
            }
        }

        public abstract void CreateSlots();

        public void OnPostUpdate(ItemSlot itemSlot)
        {
            //아이템 슬롯 체크
            if(itemSlot == null || itemSlot.slotUI == null)
            {
                return;
            }

            itemSlot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite 
                = itemSlot.item.id < 0 ? null : itemSlot.ItemObject.icon;
            itemSlot.slotUI.transform.GetChild(0).GetComponent<Image>().color 
                = itemSlot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
            itemSlot.slotUI.GetComponentInChildren<TextMeshProUGUI>().text 
                = itemSlot.item.id < 0 ? string.Empty : 
                (itemSlot.amount == 1 ? string.Empty : itemSlot.amount.ToString());
        }

        //이벤트 트리거에 이벤트 등록
        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                Debug.LogWarning("No EventTrigger component found!!!");
                return;
            }

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        //InventoryUI 오브젝트에 마우스가 들어갈때 호출
        public void OnEnterInterface(GameObject go)
        {
            MouseData.interfaceMouseIsOver = go.GetComponent<InventoryUI>();
        }

        //InventoryUI 오브젝트에 마우스가 나갈때 호출
        public void OnExitInterface(GameObject go)
        {
            MouseData.interfaceMouseIsOver = null;
        }

        //슬롯 오브젝트에 마우스가 들어갈때 호출
        public void OnEnter(GameObject go)
        {
            MouseData.slotHoveredOver = go;
            MouseData.interfaceMouseIsOver = GetComponentInParent<InventoryUI>();
        }

        //슬롯 오브젝트에 마우스가 들어갈때 호출
        public void OnExit(GameObject go)
        {
            MouseData.slotHoveredOver = null;
        }

        //슬롯 오브젝트에 마우스를 드래그 시작할때 호출
        public void OnStartDrag(GameObject go)
        {
            MouseData.tempItemBeginDragged = CraeteDragImage(go);

            OnUpdateSelectSlot?.Invoke(null);
            UpdateSelectSlot(null);
        }

        //마우스 드래그 시작시 마우스 포인터에 달고 다니는 이미지 오브젝트 생성
        private GameObject CraeteDragImage(GameObject go)
        {
            if (slotUIs[go].item.id <= -1)
            {
                return null;
            }

            GameObject dragImage = new GameObject();

            RectTransform rectTransform = dragImage.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);
            dragImage.transform.SetParent(transform.parent);
            Image image = dragImage.AddComponent<Image>();
            image.sprite = slotUIs[go].ItemObject.icon;
            image.raycastTarget = false;

            dragImage.name = "Drag Image";

            return dragImage;
        }

        //마우스 드래그시 마우스 포인터에 달고 다니는 이미지 위치를 마우스 위치로 설정
        public void OnDrag(GameObject go)
        {
            if(MouseData.tempItemBeginDragged == null)
            {
                return;
            }
            MouseData.tempItemBeginDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        //슬롯 오브젝트에 마우스를 드래그 끝날때 호출
        public void OnEndDrag(GameObject go)
        {
            Destroy(MouseData.tempItemBeginDragged);

            //마우스의 위치가 인벤토리 UI 밖에 있을 경우
            if (MouseData.interfaceMouseIsOver == null)
            {
                //아이템 버리기
                slotUIs[go].RemoveItem();
            }
            else if(MouseData.slotHoveredOver != null) //마우스의 위치가 슬롯 게임오브젝트 위에 있을 경우
            {
                //아이템 바꾸기
                //마우스가 위치한 게임 오브젝트의 슬롯
                ItemSlot mouseHoverSlot = MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];
                inventoryObject.SwapItems(slotUIs[go], mouseHoverSlot);
            }
        }

        //슬롯 오브젝트를 마우스로 클릭할때 호출
        public void OnClick(GameObject go)
        {
            OnUpdateSelectSlot?.Invoke(null);

            ItemSlot slot = slotUIs[go];

            //아이템 존재 여부 체크
            if(slot.item.id >= 0)
            {
                //선택되어 있는 슬롯 다시 선택
                if(selectSlotObject == go)
                {
                    UpdateSelectSlot(null);
                }
                else
                {
                    UpdateSelectSlot(go);
                }
            }
        }

        public virtual void UpdateSelectSlot(GameObject go)
        {
            selectSlotObject = go;

            foreach (KeyValuePair<GameObject, ItemSlot> slot in slotUIs)
            {
                if(slot.Key == go)
                {
                    slot.Value.slotUI.transform.GetChild(1).GetComponent<Image>().enabled = true;
                }
                else
                {
                    slot.Value.slotUI.transform.GetChild(1).GetComponent<Image>().enabled = false;
                }
            }
        }

        public virtual void OpenInventoryUI(Action closeMethod)
        {
            if (closeMethod != null)
                OnCloseInventoyUI += closeMethod;
        }

        public virtual void CloseInventoryUI()
        {
            UpdateSelectSlot(null);
            OnCloseInventoyUI?.Invoke();        //창 닫기 등록
        }

    }
}
