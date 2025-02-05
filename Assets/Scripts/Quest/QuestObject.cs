using System;

namespace MySampleEx
{
    /// <summary>
    /// 퀘스트 진행 데이터 관리 클래스
    /// </summary>
    [Serializable]
    public class QuestObject
    {
        public int number;                          //퀘스트 인덱스
        public QuestState questState;               //퀘스트 상태
        public QuestGoal questGoal;                 //퀘스트 목표

        //생성자
        public QuestObject(Quest quest)
        {
            number = quest.number;
            questState = QuestState.Ready;

            questGoal = new QuestGoal();
            questGoal.questType = quest.questType;
            questGoal.goalIndex = quest.goalIndex;
            questGoal.goalAmount = quest.goalAmount;
            questGoal.currentAmount = 0;
        }

        //퀘스트 미션 달성 - kill
        public void EnemyKill(int enemyId)
        {
            if (questGoal.questType == QuestType.Kill)
            {
                //emeny id 체크
                //if (questGoal.goalIndex == enemyId)
                {
                    questGoal.currentAmount++;
                }
            }
        }

        //퀘스트 미션 달성 - Collect
        public void ItemCollect(int itemId)
        {
            if (questGoal.questType == QuestType.Collect)
            {
                //item id 체크
                //if (questGoal.goalIndex == itemId)
                {
                    questGoal.currentAmount++;
                }
            }
        }
    }
}
