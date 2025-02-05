using UnityEngine;
using System;
using System.Collections.Generic;

namespace MySampleEx
{
    /// <summary>
    /// 캐릭터 속성 Value(값)을 관리하는 클래스
    /// </summary>
    [Serializable]
    public class ModifiableInt
    {
        #region Variables
        [NonSerialized]
        private int baseValue;          //기본 값
        [SerializeField]
        private int modifedValue;       //수정된 값, 최종 값

        public int BaseValue
        {
            get { return baseValue; }
            set { 
                baseValue = value;
                UpdateMoidifedeValue();
            }
        }
        public int ModifedValue
        {
            get { return modifedValue; }
            set { modifedValue = value; }
        }

        //modifedValue 값 변경시 등록된 함수 실행
        private event Action<ModifiableInt> OnMoidifedValue;

        //modifedValue 값 계산시 추가될 값들을 저장한 리스트
        private List<IModifier> modifiers = new List<IModifier>();
        #endregion

        //생성자 - 값 변경시 호출할 함수를 매개변수로 받아 등록
        public ModifiableInt(Action<ModifiableInt> method = null)
        {
            ModifedValue = baseValue;
            RegisterModEvent(method);
        }

        public void RegisterModEvent(Action<ModifiableInt> method)
        {
            if(method != null)
            {
                OnMoidifedValue += method;
            }
        }

        public void UnRegisterModEvent(Action<ModifiableInt> method)
        {
            if (method != null)
            {
                OnMoidifedValue -= method;
            }
        }

        //modifedValue 값 구하기, 값 변경시 등록된 함수 호출
        private void UpdateMoidifedeValue()
        {
            int valueToAdd = 0;
            foreach (var modifier in modifiers)
            {
                modifier.AddValue(ref valueToAdd);
            }
            ModifedValue = baseValue + valueToAdd;

            //modifedValue 값 변경시 등록된 함수 호출
            OnMoidifedValue?.Invoke(this);
        }

        public void AddModifier(IModifier modifier)
        {
            modifiers.Add(modifier);
            UpdateMoidifedeValue();
        }

        public void RemoveModifier(IModifier modifier)
        {
            modifiers.Remove(modifier);
            UpdateMoidifedeValue();
        }

    }
}
