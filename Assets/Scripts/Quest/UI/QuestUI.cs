using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MySampleEx
{
    /// <summary>
    /// (퀘스트 목록), 퀘스트 진행창, 퀘스트 정보창
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;

        public TextMeshProUGUI goalText;
        public TextMeshProUGUI rewardgoldText;
        public TextMeshProUGUI rewardExpText;
        public TextMeshProUGUI rewardItemText;
        public Image itemImage;

        public GameObject acceptButton;
        public GameObject giveupButton;
        public GameObject okButton;

        //퀘스트 진행 종료시 실행될 이벤트
        public Action OnCloseQuestUI;

        private QuestManager questManager;
        #endregion

        private void OnEnable()
        {
            if (questManager == null)
            {
                questManager = QuestManager.Instance;
            }

            OnCloseQuestUI = null;
        }

        void SetQuestUI(QuestObject questObject)
        {
            Quest quest = DataManager.GetQuestData().Quests.quests[questObject.number];

            nameText.text = quest.name;
            if (questObject.questState == QuestState.Complete)
            {
                descriptionText.text = "Quest Completed";
            }
            else
            {
                descriptionText.text = quest.description;
            }

            goalText.text = questObject.questGoal.currentAmount.ToString() + " / " + questObject.questGoal.goalAmount.ToString();
            rewardgoldText.text = quest.rewardGold.ToString();
            rewardExpText.text = quest.rewardExp.ToString();

            if(quest.rewardItem >= 0)
            {
                rewardItemText.text = UIManager.Instance.database.itemObjects[quest.rewardItem].name;
                itemImage.sprite = UIManager.Instance.database.itemObjects[quest.rewardItem].icon;
                itemImage.enabled = true;
            }
            else
            {
                rewardItemText.text = "";
                itemImage.sprite = null;
                itemImage.enabled = false;
            }

            //버튼 셋팅
            ResetButtons();
            switch (questObject.questState)
            {
                case QuestState.Ready:
                    acceptButton.SetActive(true);
                    break;
                case QuestState.Accept:
                    giveupButton.SetActive(true);
                    break;
                case QuestState.Complete:
                    okButton.SetActive(true);
                    break;
            }
        }

        void ResetButtons()
        {
            acceptButton.SetActive(false);
            giveupButton.SetActive(false);
            okButton.SetActive(false);
        }

        //플레이어 퀘스트 보기
        public void OpenPlayerQuestUI(Action closeMethod)
        {
            if (closeMethod != null)
                OnCloseQuestUI += closeMethod;

            if (questManager.playerQuests.Count <= 0)
            {
                CloseQuestUI();
                return;
            }

            questManager.SetCurrentQuest(questManager.playerQuests[0]);
            SetQuestUI(questManager.currentQuest);
        }

        //NPC 퀘스트 보기
        public void OpenQuestUI(Action closeMethod)
        {
            if(closeMethod != null)
                OnCloseQuestUI += closeMethod;

            if (questManager.currentQuest == null)
            {
                CloseQuestUI();
                return;
            }

            SetQuestUI(questManager.currentQuest);
        }

        public void CloseQuestUI()
        {
            ResetButtons();
            OnCloseQuestUI?.Invoke();
        }

        public void AcceptQuest()
        {
            //플레이어의 퀘스트리스트에 currentQuest 추가
            questManager.AddPlayerQuest();

            CloseQuestUI();
        }

        public void GiveupQuest()
        {
            //플레이어의 퀘스트리스트에서 currentQuest 제거
            questManager.GiveupPlayerQuest();

            CloseQuestUI();
        }
    }
}
