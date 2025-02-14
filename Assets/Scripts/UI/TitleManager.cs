using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MySampleEx
{
    public class TitleManager : MonoBehaviour
    {
        #region Variables
        public GameObject mainMenu;
        public GameObject option;

        public GameObject login;
        public GameObject loginMenu;
        public GameObject loginUI;
        public GameObject messageUI;
        public GameObject loginButton;
        public GameObject newButton;

        public TMP_InputField loginId;
        public TMP_InputField password;

        public TextMeshProUGUI message;

        private NetManager netManager;

#if AD_MODE
        private AdManager adManager;
#endif

#if FIREBASE_MODE
        private FirebaseAuthManager firebaseAuthManager;
        private FirebaseDatabaseManager firebaseDatabaseManager;
#endif

        #endregion

        private void OnEnable()
        {
            netManager = NetManager.Instance;
#if NET_MODE
            netManager.OnNetUpdate += OnNetUpdate;
#endif
#if FIREBASE_MODE
            firebaseAuthManager = FirebaseAuthManager.Instance;
            firebaseAuthManager.InitializeFirebase();
            firebaseAuthManager.OnChangedAuthState += OnNetUpdate;

            firebaseDatabaseManager = FirebaseDatabaseManager.Instance;
            firebaseDatabaseManager.OnChangeData += OnNetUpdate;
#endif
        }

        private void OnDisable()
        {
#if NET_MODE
            netManager.OnNetUpdate -= OnNetUpdate;
#endif
#if FIREBASE_MODE
            firebaseAuthManager.OnChangedAuthState -= OnNetUpdate;
            firebaseDatabaseManager.OnChangeData -= OnNetUpdate;
#endif
        }

        private void Start()
        {
            //참조
#if NET_MODE || FIREBASE_MODE
            ShowLogin();
#endif

#if AD_MODE
            adManager = AdManager.Instance;
#endif
        }

        public void StartPlay()
        {
#if AD_MODE
            adManager.HideBanner();
#endif
            SceneManager.LoadScene("PlayScene");
        }

        public void ShowOption()
        {
#if AD_MODE
            adManager.HideBanner();
            adManager.ShowInterstitialAd();
            //adManager.ShowRewardAd();
#endif
            mainMenu.SetActive(false);
            option.SetActive(true);
        }

        public void HideOption()
        {
#if AD_MODE
            adManager.ShowBanner();
#endif
            option.SetActive(false);
            mainMenu.SetActive(true);
        }


        public void OnNetUpdate(int netResult)
        {
            switch(netManager.netMessage)
            {
                case NetMessage.Login:
                    if (netResult == 0) //로그인 성공
                    {
                        //로그인 성공 - 유저 정보 가져오기
#if NET_MODE
                        netManager.NetSendUserInfo();
#endif
#if FIREBASE_MODE
                        netManager.netMessage = NetMessage.UserInfo;
                        firebaseDatabaseManager.OnLoad();
#endif

                    }
                    else if (netResult == 1)   //아이디가 없다
                    {
                        //경고창 띄우기
                        ShowMessageUI("유저 아이디가 없습니다");
                    }
                    else    //로그인 실패
                    {
                        //경고창 띄우기
                        ShowMessageUI("네트워크가 불안정 합니다. 다시 실행해주세요");
                    }
                    break;

                case NetMessage.RegisterUser:
                    if (netResult == 0) //유저 등록 성공
                    {
#if FIREBASE_MODE
                        Debug.Log("유저 정보 저장하기"); //UI에서 결과 처리안한다
                        netManager.netMessage = NetMessage.None;
                        firebaseDatabaseManager.OnChangedStats();
#endif
                        ShowMessageUI("유저 등록에 성공 했습니다");
                    }
                    else if (netResult == 1)   //아이디 중복
                    {
                        ShowMessageUI("중복된 아이디 입니다");
                    }
                    else    //로그인 실패
                    {
                        ShowMessageUI("네트워크가 불안정 합니다. 다시 실행해주세요");
                    }
                    break;

                case NetMessage.UserInfo:
                    if (netResult == 0) //가져오기 성공
                    {
                        ShowMainMenu();
                    }
                    else    //가져오기 실패
                    {
                        ShowMessageUI("네트워크가 불안정 합니다. 다시 실행해주세요");
                    }
                    break;
            }
        }


        public void ShowLogin()
        {
            mainMenu.SetActive(false);
            login.SetActive(true);
            ShowLoingMenu();
        }

        public void ShowMainMenu()
        {
            login.SetActive(false);
            mainMenu.SetActive(true);
        }

        void ResetLoginUI()
        {
            loginMenu.SetActive(false);
            loginUI.SetActive(false);
            messageUI.SetActive(false);
            loginButton.SetActive(false);
            newButton.SetActive(false);
            message.text = "";
        }

        public void ShowLoingMenu()
        {
            ResetLoginUI();
            loginMenu.SetActive(true);
        }

        public void ShowLoginUI()
        {
            ResetLoginUI();
            loginUI.SetActive(true);
            loginButton.SetActive(true);
        }

        public void ShowAddUserUI()
        {
            ResetLoginUI();
            loginUI.SetActive(true);
            newButton.SetActive(true);
        }

        public void ShowMessageUI(string msg)
        {
            ResetLoginUI();
            messageUI.SetActive(true);
            message.text = msg;
        }

        public void HideMessageUI()
        {
            if (netManager.netFail)
            {
                Application.Quit();
                return;
            }

            ShowLoingMenu();
        }

        public void Login()
        {
            if(loginId.text.Length < 8 || loginId.text.Length > 20)
            {
                return;
            }
            if (password.text.Length < 8 || password.text.Length > 20)
            {
                return;
            }

#if NET_MODE
            netManager.NetSendLogin(loginId.text, password.text);
#endif
#if FIREBASE_MODE
            netManager.netMessage = NetMessage.Login;
            firebaseAuthManager.SignIn(loginId.text, password.text);
#endif            
            ResetLoginUI();
        }

        public void RegisterUser()
        {
            if (loginId.text.Length < 8 || loginId.text.Length > 20)
            {
                return;
            }
            if (password.text.Length < 8 || password.text.Length > 20)
            {
                return;
            }

#if NET_MODE
            netManager.NetSendUserRegister(loginId.text, password.text);
#endif
#if FIREBASE_MODE
            netManager.netMessage = NetMessage.RegisterUser;
            firebaseAuthManager.CreateUser(loginId.text, password.text);
#endif            
            ResetLoginUI();
        }

    }
}