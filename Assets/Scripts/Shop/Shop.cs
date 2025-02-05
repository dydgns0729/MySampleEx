using System;

namespace MySampleEx
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Shop
    {
        public int npcNumber;               //npc 번호
        public int dialogIndex;             //npc 대화내용(판매전, 판매후)
        public ItemObject[] itemObject;     //npc가 판매하는 아이템 목록
    }
}
