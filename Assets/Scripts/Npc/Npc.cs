using UnityEngine;
using System;

namespace MySampleEx
{
    [Serializable]
    public class Npc
    {
        public NpcType npcType;     //npc 타입
        public int number;          //npc 고유번호
        public string name;         //npc 이름
    }

    public enum NpcType
    {
        None = -1,
        Merchant,
        BlackSmith,
        SkillMaster,
        QuestGiver,
    }
}
