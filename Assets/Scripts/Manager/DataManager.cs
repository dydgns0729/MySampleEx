using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 게임에서 사용하는 데이터들을 관리하는 클래스
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private static EffectData effectData = null;
        private static DialogData dialogData = null;
        private static QuestData questData = null;

        private void Start()
        {
            //이펙트 데이터 가져오기
            if (effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
            }

            //다이알로그 데이터 가져오기
            if (dialogData == null)
            {
                dialogData = ScriptableObject.CreateInstance<DialogData>();
                dialogData.LoadData();
            }

            //퀘스트 데이터 가져오기
            if (questData == null)
            {
                questData = ScriptableObject.CreateInstance<QuestData>();
                questData.LoadData();
            }
        }

        //이펙트 데이터 가져오기
        public static EffectData GetEffectData()
        {
            if (effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
            }
            return effectData;
        }

        //다이알로그 데이터 가져오기
        public static DialogData GetDialogData()
        {
            if (dialogData == null)
            {
                dialogData = ScriptableObject.CreateInstance<DialogData>();
                dialogData.LoadData();
            }
            return dialogData;
        }

        //퀘스트 데이터 가져오기
        public static QuestData GetQuestData()
        {
            if (questData == null)
            {
                questData = ScriptableObject.CreateInstance<QuestData>();
                questData.LoadData();
            }
            return questData;
        }

    }
}