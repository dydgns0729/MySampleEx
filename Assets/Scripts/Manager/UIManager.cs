using UnityEngine;

namespace MySampleEx
{
    public class UIManager : Singleton<UIManager>
    {
        #region Variables
        public ItemDataBase database;
        public InventoryObject inventory;
        public StatsObject playerStats;
        
        public DynamicInventoryUI palyerInventoryUI;
        public StaticInventoryUI palyerEquipmentUI;
        public PlayerStatsUI playerStatsUI;
        public DialogUI dialogUI;
        public QuestUI questUI;
        public ShopUI shopUI;

        public int itemId = 0;
        #endregion

        private void OnEnable()
        {
            palyerInventoryUI.OnUpdateSelectSlot += palyerEquipmentUI.UpdateSelectSlot;

            palyerEquipmentUI.OnUpdateSelectSlot += palyerInventoryUI.UpdateSelectSlot;
        }

        private void Update()
        {
#if !TOUCH_MODE
            if(Input.GetKeyDown(KeyCode.I))
            {
                OpenPlayerInventoryUI();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                OpenPlayerEquipmentUI();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                OpenPlayerStatsUI()                
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OpenPlayerQuestUI();
            }
            if(Input.GetKeyDown(KeyCode.Y))
            {
                OpenShopUI();
            }
#endif
        }

        public void Toggle(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }

        public void OpenPlayerInventoryUI()
        {
            palyerInventoryUI.UpdateSelectSlot(null);
            Toggle(palyerInventoryUI.gameObject);
            if(palyerInventoryUI.gameObject.activeSelf)
            {
                palyerInventoryUI.OpenInventoryUI(ClosePlayerInventoryUI);
            }
        }

        public void ClosePlayerInventoryUI()
        {
            Toggle(palyerInventoryUI.gameObject);
        }

        public void OpenPlayerEquipmentUI()
        {
            palyerEquipmentUI.UpdateSelectSlot(null);
            Toggle(palyerEquipmentUI.gameObject);
            if (palyerEquipmentUI.gameObject.activeSelf)
            {
                palyerEquipmentUI.OpenInventoryUI(ClosePlayerEquipmentUI);
            }
        }

        public void ClosePlayerEquipmentUI()
        {
            Toggle(palyerEquipmentUI.gameObject);
        }

        public void OpenPlayerStatsUI()
        {
            Toggle(playerStatsUI.gameObject);
        }

        public void OpenDialogUI(int dialogIndex, NpcType npcType = NpcType.None)
        {
            Toggle(dialogUI.gameObject);
            dialogUI.OnCloseDialog += CloseDialogUI;
            if (npcType == NpcType.QuestGiver)
            {
                dialogUI.OnCloseDialog += OpenQuestUI;
            }
            else if(npcType == NpcType.Merchant)
            {
                dialogUI.OnCloseDialog += OpenShopUI;
            }

            dialogUI.StartDialog(dialogIndex);
        }

        public void CloseDialogUI()
        {
            Toggle(dialogUI.gameObject);
        }

        //플레이어 퀘스트 보기 (퀘스트 리스트...)
        public void OpenPlayerQuestUI()
        {
            Toggle(questUI.gameObject);
            if(questUI.gameObject.activeSelf)
            {
                questUI.OpenPlayerQuestUI(CloseQuestUI);
            }   
        }

        //NPC 퀘스트 보기
        public void OpenQuestUI()
        {
            Toggle(questUI.gameObject);            
            questUI.OpenQuestUI(CloseQuestUI);
        }

        public void CloseQuestUI()
        {
            Toggle(questUI.gameObject);
        }

        public void OpenShopUI()
        {
            shopUI.UpdateSelectSlot(null);
            Toggle(shopUI.gameObject);
            if(shopUI.gameObject.activeSelf)
            {
                shopUI.OpenInventoryUI(CloseShopUI);
            }
        }

        public void CloseShopUI()
        {
            Toggle(shopUI.gameObject);
        }

        public bool AddItemInventory(Item item, int amount)
        {
            return inventory.AddItem(item, amount);
        }

        public void AddGold(int amount)
        {
            playerStats.AddGold(amount);
        }

        public bool UseGold(int amount)
        {
            return playerStats.UseGold(amount);
        }

        public bool EnoughGold(int amount)
        {
            return playerStats.EnoughGold(amount);
        }

    }
}