using UnityEngine;
using TMPro;

namespace MySampleEx
{
    public class PlayerStatsUI : MonoBehaviour
    {
        #region Variables
        public InventoryObject equipment;
        public StatsObject stats;

        public TextMeshProUGUI[] attributeTexts;
        #endregion

        private void Awake()
        {
            if (equipment != null && stats != null)
            {
                foreach (var slot in equipment.Slots)
                {
                    slot.OnPreUpdate += OnRemoveItem;
                    slot.OnPostUpdate += OnEquipItem;
                }
            }
        }

        private void OnEnable()
        {
            stats.OnChagnedStats += OnChangedStats;
            UpdateAttributeTexts();
        }

        private void OnDisable()
        {
            stats.OnChagnedStats -= OnChangedStats;
        }

        private void UpdateAttributeTexts()
        {
            attributeTexts[0].text = stats.GetModifiredValue(CharacterAttribute.Agility).ToString();
            attributeTexts[1].text = stats.GetModifiredValue(CharacterAttribute.Intellect).ToString();
            attributeTexts[2].text = stats.GetModifiredValue(CharacterAttribute.Stamina).ToString();
            attributeTexts[3].text = stats.GetModifiredValue(CharacterAttribute.Strength).ToString();
        }

        private void OnRemoveItem(ItemSlot slot)
        {
            if (slot.ItemObject == null)
                return;

            Debug.Log("OnRemoveItem");

            if(slot.parent.type == InterfaceType.Equipment)
            {
                foreach(ItemBuff buff in slot.item.buffs)
                {
                    foreach(var attribute in stats.attributes)
                    {
                        if(attribute.type == buff.stat)
                        {
                            attribute.value.RemoveModifier(buff);
                        }
                    }
                }
            }
        }

        private void OnEquipItem(ItemSlot slot)
        {
            if (slot.ItemObject == null)
                return;

            Debug.Log("OnEquipItem");

            if (slot.parent.type == InterfaceType.Equipment)
            {
                foreach (ItemBuff buff in slot.item.buffs)
                {
                    foreach (var attribute in stats.attributes)
                    {
                        if (attribute.type == buff.stat)
                        {
                            attribute.value.AddModifier(buff);
                        }
                    }
                }
            }
        }


        private void OnChangedStats(StatsObject statsObject)
        {
            UpdateAttributeTexts();
        }
    }
}