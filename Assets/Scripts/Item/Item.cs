using System;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// Item (id, name)
    /// itemBuff 아이템 능력치
    /// </summary>
    [Serializable]
    public class Item
    {
        public int id = -1;
        public string name;

        public ItemBuff[] buffs;

        //생성자
        public Item()
        {
            id = -1;
            name = "";
        }

        //생성자 - 매개변수로 ItemObject를 받아서 아이템 데이터 셋팅
        public Item(ItemObject itemObject)
        {
            this.name = itemObject.name;
            this.id = itemObject.data.id;

            buffs = new ItemBuff[itemObject.data.buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(itemObject.data.buffs[i].Min, itemObject.data.buffs[i].Max)
                {
                    stat = itemObject.data.buffs[i].stat
                };
            }
        }
    }
}
