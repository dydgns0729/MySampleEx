using System.Collections.Generic;
using UnityEngine;

namespace MySampleEx
{
    public class Effect
    {
        public List<EffectClip> effectClips { get; set; }
    }
    /// <summary>
    /// 이펙트 속성 데이터 : 이펙트 프리팹, 경로, 타입 등....
    /// 기능 : 프리팹 사전 로딩, 이팩트 인스턴스화
    /// </summary>
    public class EffectClip
    {
        #region Variables
        public int id { get; set; }                 //이펙트 목록 인덱스
        public string name { get; set; }            //이펙트 이름
        public EffectType effectType { get; set; }  //이펙트 타입
        public string effectPath { get; set; }      //이펙트 프리팹 경로
        public string effectName { get; set; }      //이펙트 프리팹 이름

        public GameObject effectPrefab = null;
        #endregion

        public EffectClip() { }

        //프리팹 사전로딩
        public void PreLoad()
        {
            if (effectPrefab == null || effectName == null)
            {
                return;
            }
            var effectFullpath = effectPath + effectName;
            if (effectFullpath != string.Empty && effectPrefab == null)
            {
                effectPrefab = ResourcesManager.Load(effectFullpath) as GameObject;
            }
        }

        //프리팹 해제
        public void ReleaseEffect()
        {
            if (effectPrefab != null)
            {
                effectPrefab = null;
            }
        }

        public GameObject Instantiate(Vector3 position)
        {
            if (effectPrefab == null)
            {
                PreLoad();
            }
            if (this.effectPrefab != null)
            {
                GameObject effectGo = GameObject.Instantiate(effectPrefab, position, Quaternion.identity);
            }
            return null;
        }
    }
}