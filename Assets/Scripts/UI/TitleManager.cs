using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        public TMP_InputField loginPw;

        public TextMeshProUGUI message;

        public NetManager netManager;
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
            firebaseDatabaseManager.OnChangedData += OnNetUpdate;
#endif
        }

        private void OnDisable()
        {
#if NET_MODE
            netManager.OnNetUpdate -= OnNetUpdate;
#endif
#if FIREBASE_MODE
            firebaseAuthManager.OnChangedAuthState -= OnNetUpdate;
            firebaseDatabaseManager.OnChangedData -= OnNetUpdate;
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
#endif
            //adManager.ShowRewardAd();

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
            switch (netManager.netMessage)
            {
                case NetMessage.Login:
                    if (netResult == 0)         //로그인 성공
                    {
#if NET_MODE
                        netManager.NetSendUserInfo();
#endif
#if FIREBASE_MODE
                        netManager.netMessage = NetMessage.UserInfo;
                        firebaseDatabaseManager.OnLoad();
                        Debug.Log("로그인 성공 - 유저 정보 가져오기");
#endif


                    }
                    else if (netResult == 1)    //로그인 실패 : 아이디가 없다
                    {
                        //경고창 띄우기
                        ShowMessageUI("로그인 실패 : 아이디가 없습니다");
                    }
                    else                        //로그인 실패 : 다른 이유
                    {
                        //경고창 띄우기
                        ShowMessageUI("네트워크가 불안정 합니다. 다시 실행해주세요");
                    }
                    break;
                case NetMessage.RegisterUser:
                    if (netResult == 0)         //회원가입 성공
                    {
#if FIREBASE_MODE
                        Debug.Log("유저 정보 저장하기");
                        netManager.netMessage = NetMessage.None;
                        firebaseDatabaseManager.OnChangedStatsInfo();
#endif
                        ShowMessageUI("회원가입 성공");
                    }
                    else if (netResult == 1)    //아이디 중복
                    {
                        ShowMessageUI("회원가입 실패 : 중복 아이디");
                    }
                    else                        //회원가입 실패 : 다른 이유
                    {
                        ShowMessageUI("네트워크가 불안정 합니다. 다시 실행해주세요");
                    }
                    break;
                case NetMessage.UserInfo:
                    if (netResult == 0)         //가져오기 성공
                    {
                        Debug.Log("유저정보 가져오기 성공");
                        ShowMainMenu();
                    }
                    else                        //아이디 중복
                    {
                        ShowMessageUI("네트워크가 불안정 합니다. 다시 실행해주세요");
                    }
                    break;
                case NetMessage.LevelUp:
                    break;
            }
        }

        private void ResetLoginUI()
        {
            loginMenu.SetActive(false);
            loginUI.SetActive(false);
            messageUI.SetActive(false);
            loginButton.SetActive(false);
            newButton.SetActive(false);
            message.text = "";
        }

        public void ShowLogin()
        {
            ResetLoginUI();
            mainMenu.SetActive(false); //false
            login.SetActive(true);
            ShowLoginMenu();
        }

        public void ShowMainMenu()
        {
            login.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void ShowLoginMenu()
        {
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
            ShowLoginMenu();
            messageUI.SetActive(false);
            message.text = "";
        }

        public void Login()
        {
            if (loginId.text.Length < 8 || loginId.text.Length > 20 || loginPw.text.Length < 7 || loginPw.text.Length > 20)
            {
                Debug.Log("아이디 또는 비밀번호 형식이 잘못되었습니다람쥐");
                return;
            }
#if NET_MODE
            netManager.NetSendLogin(loginId.text, loginPw.text);
#endif
#if FIREBASE_MODE
            netManager.netMessage = NetMessage.Login;
            firebaseAuthManager.SignIn(loginId.text, loginPw.text);
#endif
            ResetLoginUI();
        }

        public void RegisterUser()
        {
            if (loginId.text.Length < 8 || loginId.text.Length > 20 || loginPw.text.Length < 8 || loginPw.text.Length > 20)
            {
                Debug.Log("아이디 또는 비밀번호 형식이 잘못되었습니다람쥐");
                return;
            }
#if NET_MODE
            netManager.NetSendUserRegister(loginId.text, loginPw.text);
#endif
#if FIREBASE_MODE
            netManager.netMessage = NetMessage.RegisterUser;
            firebaseAuthManager.CreateUser(loginId.text, loginPw.text);
#endif
            ResetLoginUI();

        }

    }
}
