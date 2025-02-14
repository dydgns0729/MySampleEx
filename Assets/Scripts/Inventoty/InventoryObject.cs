using System.Linq;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MySampleEx
{
    /// <summary>
    /// Inventory 데이터 컨테이너를 가지고 있는 스크터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        #region Variables
        public ItemDataBase database;                   //아이템 정보를 가지고 있는 스크립터블 오브젝트
        public InterfaceType type;                      //인벤토리 타입

        public Inventory container = new Inventory();
        //인벤토리 슬롯(읽기 전용 프로퍼티)
        public ItemSlot[] Slots => container.slots;

        //아이템 사용시 등록된 메서드 호출
        public Action<ItemObject> OnUseItem;

        //현재 빈 슬롯 갯수
        public int EmptySlotCount
        {
            get
            {
                int count = 0;
                foreach (var slot in Slots)
                {
                    if(slot.item.id <= -1)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        #endregion

        //인벤토리에 아이템 추가
        public bool AddItem(Item item, int amount)
        {
            ItemSlot slot = FindItemInInventoy(item);
            if (!database.itemObjects[item.id].stackable || slot == null)
            {
                //인벤풀 체크
                if(EmptySlotCount <= 0)
                {
                    Debug.Log("Inven Full");
                    return false;
                }

                //빈 슬롯 아이템 추가
                ItemSlot emptySlot = GetEmptySlot();
                emptySlot.UpdateSlot(item, amount);
            }
            else
            {
                //아이템을 가진 슬롯에 amount 누적
                slot.AddItemAmount(amount);
            }

            //아이템 획득 퀘스트 처리
            QuestManager.Instance.UpdateQuest(QuestType.Collect, amount);

            return true;
        }

        //매개변수로 들어온 아이템을 가진 슬롯 찾기
        public ItemSlot FindItemInInventoy(Item item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        //매개변수로 들어온 아이템 오브젝트가 인벤토리에 있는지 여부
        public bool IsContainItem(ItemObject itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        //빈 슬롯 찾기
        public ItemSlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id <= -1);
        }

        //아이템 바꾸기
        public void SwapItems(ItemSlot itemA, ItemSlot itemB)
        {
            if (itemA == itemB)
            {
                return;
            }

            if(itemB.CanPlaceInSlot(itemA.ItemObject) && itemA.CanPlaceInSlot(itemB.ItemObject))
            {
                ItemSlot temp = new ItemSlot(itemB.item, itemB.amount);
                itemB.UpdateSlot(itemA.item, itemA.amount);
                itemA.UpdateSlot(temp.item, temp.amount);
            }
        }

        //아이템 사용하기
        public void UseItem(ItemSlot useSlot)
        {
            //아이템 체크
            if (useSlot.ItemObject == null || useSlot.item.id <= -1 
                || useSlot.amount <= 0)
            {
                return;
            }

            ItemObject itemObject = useSlot.ItemObject;
            useSlot.UpdateSlot(useSlot.item, useSlot.amount-1);

            OnUseItem?.Invoke(itemObject);
        }

        //인벤토리 데이터 파일 저장하기, 불러오기
        #region Save/Load Methods
        public string savePath = "/Inventory.json";

        public string ToJson()
        {
            return JsonUtility.ToJson(container);
        }

        public void FromJson(string jsonString)
        {
            container = JsonUtility.FromJson<Inventory>(jsonString);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            string path = Application.persistentDataPath + savePath;
            Debug.Log(path);

            //데이터 2진화 준비
            BinaryFormatter bf = new BinaryFormatter();
            //저장할 파일에 접근
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            //저장할 데이터를 준비
            string saveData = JsonUtility.ToJson(container, true);
            Debug.Log(saveData);
            //데이터를 파일에 저장
            bf.Serialize(fs, saveData);
            //파일 닫기
            fs.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            string path = Application.persistentDataPath + savePath;

            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                string loadData = bf.Deserialize(fs).ToString();
                JsonUtility.FromJsonOverwrite(loadData, container);
                fs.Close();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            container.Clear();
        }
        #endregion

    }
}