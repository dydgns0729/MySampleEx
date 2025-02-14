using UnityEngine;
using System.Collections.Generic;
using System;

namespace MySampleEx
{
    public class GacharManager : MonoBehaviour
    {
        #region Variables
        private static GacharData gacharData = null;

        public GacharUI nameGachar;
        public GacharUI itemGachar;

        //수상자 목록
        public Transform prizeParent;
        public GameObject prizePrefab;

        public List<PrizeSlotUI> prizeSlots = new List<PrizeSlotUI>();
        private int winnerIndex = -1;
        #endregion

        private void Awake()
        {
            //가챠 데이터 가져오기
            if (gacharData == null)
            {
                gacharData = ScriptableObject.CreateInstance<GacharData>();
                //gacharData.gacharItemList = gacharData.LoadData("Gachar/GacharItem");
                //gacharData.gacharNameList = gacharData.LoadData("Gachar/GacharName");
            }
        }

        //가챠 데이터 가져오기
        public static GacharData GetGarcharData()
        {
            if (gacharData == null)
            {
                gacharData = ScriptableObject.CreateInstance<GacharData>();
                //gacharData.gacharItemList = gacharData.LoadData("Gachar/GacharItem");
                //gacharData.gacharNameList = gacharData.LoadData("Gachar/GacharName");
            }
            return gacharData;
        }

        private void Start()
        {
            //초기화
            nameGachar.SetGachar(true);
            itemGachar.SetGachar(false);
        }

        public void NameNextGachar()
        {
            nameGachar.nextButton.SetActive(false);
            itemGachar.SetGachar(true);

            CreatePrizeSlot();
            prizeSlots[winnerIndex].SetNameText(nameGachar.gacharList.gacharItems[nameGachar.gacharIndex].name);
        }

        public void ItemNextGachar()
        {
            itemGachar.nextButton.SetActive(false);

            nameGachar.SetGachar(true);
            itemGachar.SetGachar(false);

            prizeSlots[winnerIndex].SetItemText(itemGachar.gacharList.gacharItems[itemGachar.gacharIndex].name);
        }

        private void CreatePrizeSlot()
        {
            GameObject slotGo = Instantiate(prizePrefab, prizeParent);

            PrizeSlotUI prizeSlotUI = slotGo.GetComponent<PrizeSlotUI>();

            prizeSlots.Add(prizeSlotUI);
            winnerIndex++;
        }
    }
}