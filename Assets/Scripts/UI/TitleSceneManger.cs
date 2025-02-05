using UnityEngine;

namespace MySampleEx
{
    public class TitleSceneManger : MonoBehaviour
    {
        #region Variables
        public GameObject mainMenu;
        public GameObject option;
        #endregion

        public void ShowOption()
        {
            
            option.SetActive(true);

        }

        public void HideOption()
        {
            option.SetActive(false);
            mainMenu.SetActive(true);
        }

    }
}