using UnityEngine;
using TMPro;

namespace MySampleEx
{
    /// <summary>
    /// NPC를 관리하는 클래스, 인터랙티브 기능 추가
    /// </summary>
    public class PickupNpc : MonoBehaviour
    {
        #region Variables
        public Npc npc;

        //인터랙티브 기능
        protected PlayerController playerController;
        protected float distance;

        public TextMeshProUGUI actionTextUI;
        public string actionText = "Pickup ";
        #endregion

        protected virtual void Start()
        {
            //참조
            playerController = GameObject.FindAnyObjectByType<PlayerController>();
        }

#if !TOUCH_MODE
        protected virtual void OnMouseOver()
        {
            distance = Vector3.Distance(transform.position, playerController.transform.position);

            if(distance < 2f)
            {
                ShowActionUI();
            }
            else
            {
                HiddenActionUI();
            }

            if(Input.GetKeyDown(KeyCode.E) && distance < 2f)
            {
                //transform.GetComponent<BoxCollider>().enabled = false;
                DoAction();
            }
        }

        private void OnMouseExit()
        {
            HiddenActionUI();
        }
#endif

        protected virtual void ShowActionUI()
        {
            actionTextUI.gameObject.SetActive(true);
            actionTextUI.text = actionText + npc.name;
        }

        protected virtual void HiddenActionUI()
        {
            actionTextUI.gameObject.SetActive(false);
            actionTextUI.text = "";
        }

        public virtual void DoAction()
        {
            UIManager.Instance.OpenDialogUI(0, npc.npcType);

#if TOUCH_MODE
            //TODO : something touch
#endif
        }
    }
}