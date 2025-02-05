using System;

namespace MySampleEx
{
    /// <summary>
    /// 캐릭터 속성 타입, 값
    /// </summary>
    [Serializable]
    public class Attribute
    {
        public CharacterAttribute type;
        public ModifiableInt value;
    }
}
