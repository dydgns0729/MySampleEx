using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 게임에서 사용하는 데이터들을 관리하는 클래스
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private static EffectData effectData = null;

        private void Start()
        {
            //이펙트 데이터 가져오기
            if (effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
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

    }
}