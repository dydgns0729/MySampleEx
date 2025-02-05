using System;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 아이템 능력치 관리 클래스
    /// </summary>
    [Serializable]
    public class ItemBuff : IModifier
    {
        #region Variables
        public CharacterAttribute stat;
        public int value;

        [SerializeField] private int min;
        [SerializeField] private int max;
        public int Min => min;
        public int Max => max;
        #endregion

        //생성자
        public ItemBuff(int min, int max)
        {
            this.min = min;
            this.max = max;
            GenerateValue();
        }

        //능력치 랜덤값 생성
        public void GenerateValue()
        {
            value = UnityEngine.Random.Range(min, max);
        }

        //매개변수로 입력받은 변수에 value값 누적한다
        public void AddValue(ref int baseValue)
        {
            baseValue += value;    
        }
    }
}
