using UnityEngine;

namespace MySampleEx
{
    public class PickupMerchant : PickupNpc
    {
        #region Variables
        public Shop shop;
        #endregion

        public override void DoAction()
        {
            UIManager.Instance.OpenDialogUI(shop.dialogIndex, npc.npcType);
        }
    }
}
