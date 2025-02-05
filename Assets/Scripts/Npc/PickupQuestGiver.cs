using System.Collections.Generic;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 퀘스트를 가진 NPC
    /// </summary>
    public class PickupQuestGiver : PickupNpc
    {
        #region Variables
        public List<QuestObject> quests;

        private QuestManager questManager;
        #endregion

        private void OnEnable()
        {
            questManager = QuestManager.Instance;
            questManager.OnAcceptQuest += OnAcceptQuest;
            questManager.OnGiveupQuest += OnGiveupQuest;
            questManager.OnCompletedQuest += OnCompletedQuest;
        }

        private void OnDisable()
        {
            questManager.OnAcceptQuest -= OnAcceptQuest;
            questManager.OnGiveupQuest -= OnGiveupQuest;
            questManager.OnCompletedQuest -= OnCompletedQuest;
        }

        protected override void Start()
        {
            base.Start();
            quests = GetNpcQuest(npc.number);
        }

        //npc 인덱스에 지정된 퀘스트 목록 가져오기
        public List<QuestObject> GetNpcQuest(int npcNumber)
        {
            List<QuestObject> questList = new List<QuestObject>();

            foreach (Quest quest in DataManager.GetQuestData().Quests.quests)
            {
                if (quest.npcNumber == npcNumber)
                {
                    QuestObject newQuest = new QuestObject(quest);
                    questList.Add(newQuest);
                }
            }

            return questList;
        }

        public override void DoAction()
        {
            if(quests.Count == 0)
            {
                //0~2 중의 하나를 랜덤하게 대화한다
                int randNum = Random.Range(0, 3);
                UIManager.Instance.OpenDialogUI(randNum);
                return;
            }

            questManager.SetCurrentQuest(quests[0]);
            int dialogIndex = DataManager.GetQuestData().Quests.quests[quests[0].number].dialogIndex;

            switch (quests[0].questState)
            {
                case QuestState.Ready:
                    UIManager.Instance.OpenDialogUI(dialogIndex, npc.npcType);
                    break;
                case QuestState.Accept:
                    UIManager.Instance.OpenDialogUI(dialogIndex+1);
                    break;
                case QuestState.Complete:
                    UIManager.Instance.OpenDialogUI(dialogIndex+2, npc.npcType);
                    CompletedQuest();
                    break;
            }
        }

        //퀘스트 완료 처리
        public void CompletedQuest()
        {
            //퀘스트 보상 받기
            questManager.RewardQuest();

            //NPC퀘스트 리스트에서 제거
            quests.Remove(quests[0]);
        }

        public void OnAcceptQuest(QuestObject questObject)
        {
            foreach (var quest in quests)
            {
                if(quest.number == questObject.number)
                {
                    quest.questState = QuestState.Accept;
                }
            }
        }

        public void OnGiveupQuest(QuestObject questObject)
        {
            foreach (var quest in quests)
            {
                if (quest.number == questObject.number)
                {
                    quest.questState = QuestState.Ready;
                }
            }
        }

        public void OnCompletedQuest(QuestObject questObject)
        {
            foreach (var quest in quests)
            {
                if (quest.number == questObject.number)
                {
                    quest.questState = QuestState.Complete;
                }
            }
        }
    }
}