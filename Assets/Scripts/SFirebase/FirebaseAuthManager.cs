using Firebase.Auth;
using System;
using Firebase;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// Firebase 인증 (로그인, 계정생성)
    /// </summary>
    public class FirebaseAuthManager
    {
        #region Singleton
        private static FirebaseAuthManager instance = null;
        public static FirebaseAuthManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FirebaseAuthManager();
                }
                return instance;
            }
        }
        #endregion

        #region Variables
        private FirebaseAuth auth;
        private FirebaseUser user;

        public string UserId => user?.UserId ?? string.Empty;

        //Auth 상태 변경시 등록된 함수 호출
        public Action<int> OnChangedAuthState;
        #endregion

        //FirebaseAuth 초기화
        public void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += OnAuthStateChanged;
            OnAuthStateChanged(this, null);
        }

        //Firebase 계정 생성
        public async void CreateUser(string id, string pw)
        {
            int result = 0;

            await auth.CreateUserWithEmailAndPasswordAsync(id, pw).ContinueWith(task =>
            {
                //취소했는지 확인
                if (task.IsCanceled)
                {
                    Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled");
                    result = 2;
                    return;
                }
                //실패했는지 확인
                if (task.IsFaulted)
                {
                    int errorCode = GetFirebaseErrorCode(task.Exception);
                    result = (errorCode == (int)Firebase.Auth.AuthError.EmailAlreadyInUse) ? 1 : 2;
                    Debug.LogError($"CreateUserWithEmailAndPasswordAsync was Faulted / Error : {task.Exception}");
                    return;
                }
                //계정 생성 성공
                Firebase.Auth.AuthResult authResult = task.Result;
                Debug.Log($"Firebase user create success : {authResult.User.DisplayName}, {authResult.User.UserId}");
            });

            OnChangedAuthState?.Invoke(result);
        }

        //Firebase Auth 계정 로그인
        public async void SignIn(string id, string pw)
        {
            int result = 0;

            await auth.SignInWithEmailAndPasswordAsync(id, pw).ContinueWith(task =>
            {
                //취소했는지 확인
                if (task.IsCanceled)
                {
                    Debug.Log("SignInWithEmailAndPasswordAsync was canceled");
                    result = 2;
                    return;
                }
                //실패했는지 확인
                if (task.IsFaulted)
                {
                    int errorCode = GetFirebaseErrorCode(task.Exception);
                    switch (errorCode)
                    {
                        case (int)Firebase.Auth.AuthError.EmailAlreadyInUse:
                            Debug.LogError("EmailAlreadyInUse");
                            result = 2;
                            break;
                        case (int)Firebase.Auth.AuthError.WrongPassword:
                            Debug.LogError("WrongPassword");
                            result = 1;
                            break;
                        default:
                            result = 2;
                            break;
                    }
                    return;
                }

                //계정 생성 성공
                Firebase.Auth.AuthResult authResult = task.Result;
                Debug.Log($"User Signed in success : {authResult.User.DisplayName}, {authResult.User.UserId}");
            });

            OnChangedAuthState?.Invoke(result);
        }

        //Firebase Auth 계정 로그아웃
        public void SignOut()
        {
            auth.SignOut();
        }

        //Firebase Auth 에러코드 가져오기
        private int GetFirebaseErrorCode(AggregateException exception)
        {
            FirebaseException firebaseException = null;
            foreach (Exception ex in exception.Flatten().InnerExceptions)
            {
                firebaseException = ex as FirebaseException;
                if (firebaseException != null)
                {
                    break;
                }
            }
            return firebaseException?.ErrorCode ?? 0;
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            if (auth.CurrentUser != null)
            {
                bool signedIn = (user != auth.CurrentUser && auth.CurrentUser != null);
                if (!signedIn && user != null)
                {
                    Debug.Log($"Signed Out : {user.UserId}");
                    //EX : OnChangedAuthState?.Invoke(-1);
                }

                user = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log($"Signed In : {user.UserId}");
                    //EX : OnChangedAuthState?.Invoke(0);
                }
            }
        }
    }
}