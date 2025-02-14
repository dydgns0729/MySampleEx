using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;


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
                if(instance == null)
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

        //Firebase 계성 생성(email, password)
        public async void CreateUser(string email, string password)
        {
            int result = 0;

            await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if(task.IsCanceled)
                {
                    Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled");
                    result = 2;
                    return;
                }
                if (task.IsFaulted)
                {
                    int errorCode = GetFirebaseErrorCode(task.Exception);
                    result = (errorCode == (int)Firebase.Auth.AuthError.EmailAlreadyInUse) ? 1 : 2;
                    Debug.LogError($"CreateUserWithEmailAndPasswordAsync error: {task.Exception}");
                    return;
                }

                //계정 생성 성공
                Firebase.Auth.AuthResult authResult = task.Result;
                Debug.Log($"Firebase user create success: {authResult.User.DisplayName}, {authResult.User.UserId}");
            });

            OnChangedAuthState?.Invoke(result);
        }

        //Firebase Auth 로그인(email, password)
        public async void SignIn(string email, string password)
        {
            int result = 0;

            await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SignInWithEmailAndPasswordAsync was canceled");
                    result = 2;
                    return;
                }
                if (task.IsFaulted)
                {
                    int errorCode = GetFirebaseErrorCode(task.Exception);
                    switch(errorCode)
                    {
                        case (int)Firebase.Auth.AuthError.EmailAlreadyInUse:
                            Debug.LogError($"Email Already Use");
                            result = 2;
                            break;

                        case (int)Firebase.Auth.AuthError.WrongPassword:
                            Debug.LogError($"WrongPassword");
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
                Debug.Log($"user signed in success: {authResult.User.DisplayName}, {authResult.User.UserId}");
            });

            OnChangedAuthState?.Invoke(result);
        }

        //Firebase Auth 로그아웃
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
                if(firebaseException != null)
                {
                    break;
                }
            }
            return firebaseException?.ErrorCode ?? 0;
        }

        private void OnAuthStateChanged(object sender, EventArgs eventArgs)
        {
            if(auth.CurrentUser != user)
            {
                bool signedIn = (user != auth.CurrentUser && auth.CurrentUser != null);
                if (!signedIn && user != null)
                {
                    Debug.Log($"Signed Out: {user.UserId}");
                    //OnChangedAuthState?.Invoke(0);
                }

                user = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log($"Signed In: {user.UserId}");
                    //OnChangedAuthState?.Invoke(0);
                }
            }
        }
    }
}
