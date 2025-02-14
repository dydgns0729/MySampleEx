using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System;

namespace MySampleEx
{
    public class NetManager : PersistentSingleton<NetManager>
    {
        #region Variables
        //서버 URL (dev,live)
#if LIVE_MODE
        private string serverURL = "http://192.168.101.154:6001";       //Live 서버
#else
        private string serverURL = "http://192.168.101.154:6001";       //Dev 서버
#endif
        //Http Post 호출
        Dictionary<HttpWebRequest, object> mRequestDatas = new Dictionary<HttpWebRequest, object>();
        public delegate void WWWRequestFinished(string pSuccess, string pDatat);

        //Net State 관리
        public NetMessage netMessage = NetMessage.None;

        public bool netFail = false;        //응답 실패 처리
        //private string netError = "";       //에러 메세지
        private int netResult = -1;

        private string userId = "";

        //응답 성공시 호출되는 이벤트 함수
        public Action<int> OnNetUpdate;

        //게임 데이터
        public StatsObject playerStats;
        #endregion

        //서버 요청
        public void POST(string url, string post, WWWRequestFinished pDelegate)
        {
            var bytes = Encoding.UTF8.GetBytes(post);

            HttpWebRequest aWww = (HttpWebRequest)WebRequest.Create(url);
            aWww.Method = "POST";
            aWww.ContentType = "application/json";
            aWww.MediaType = "application/json";
            aWww.Accept = "application/json";
            aWww.ContentLength = bytes.Length;

            Stream requestStream = aWww.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            mRequestDatas[aWww] = pDelegate;
            StartCoroutine(WaitForRequest(aWww));
        }

        IEnumerator WaitForRequest(HttpWebRequest pWww)
        {
            yield return pWww;

            string aSuccess = "success";

            //응답처리
            HttpWebResponse httpWebResponse;
            using (httpWebResponse = (HttpWebResponse)pWww.GetResponse())
            {
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                string result = streamReader.ReadToEnd();

                WWWRequestFinished aDelegate = (WWWRequestFinished)mRequestDatas[pWww];
                aDelegate(aSuccess, result);
            }
        }

        //응답 결과 처리
        private void ReciveResult(string pSuccess, string pData)
        {
            if (pSuccess.Equals("success"))
            {
                switch(netMessage)
                {
                    case NetMessage.Login:
                        UserLoginResult loginResult = JsonUtility.FromJson<UserLoginResult>(pData);
                        NetSendLoginResult(loginResult);
                        break;

                    case NetMessage.RegisterUser:
                        UserRegisterResult registerResult = JsonUtility.FromJson<UserRegisterResult>(pData);
                        NetSendUserRegisterResult(registerResult);
                        break;

                    case NetMessage.UserInfo:
                        UserInfoResult infoResult = JsonUtility.FromJson<UserInfoResult>(pData);
                        NetSendUserInfoResult(infoResult);
                        break;

                    case NetMessage.Levelup:
                        UserLevelupResult levelupResult = JsonUtility.FromJson<UserLevelupResult>(pData);
                        NetSendUserLevelupResult(levelupResult);
                        break;
                }

                OnNetUpdate?.Invoke(netResult);
            }
        }

        //넷 메세지 설정
        private void SetNetMessage(NetMessage message)
        {
            netMessage = message;
            netFail = false;
            //netError = "";
        }

        //로그인 요청
        public void NetSendLogin(string id, string password)
        {
            //네트워크 상태 설정
            SetNetMessage(NetMessage.Login);

            //보내는 메세지 가공
            UserLogin userLogin = new UserLogin();
            userLogin.protocol = (int)netMessage;
            userLogin.userId = id;
            userLogin.password = password;
            string json = JsonUtility.ToJson(userLogin);

            //요청
            string requestUrl = serverURL + "/api/UserLoginServices";
            //Debug.Log($"userLogin: {json}");
            POST(requestUrl, json, ReciveResult);
        }

        private void NetSendLoginResult(UserLoginResult loginResult)
        {
            netResult = loginResult.result;
            if (netResult == 0) //로그인 성공
            {
                Debug.Log("로그인 성공");
                userId = loginResult.userId;
            }
            else if (netResult == 1)   //아이디가 없다
            {
                Debug.Log("로그인 아이디가 없습니다");
            }
            else    //로그인 실패
            {
                netFail = true;
                Debug.Log("로그인 실패");
            }
        }

        //유저 등록 요청
        public void NetSendUserRegister(string id, string password)
        {
            //네트워크 상태 설정
            SetNetMessage(NetMessage.RegisterUser);

            //보내는 메세지 가공
            UserRegister userRegister = new UserRegister();
            userRegister.protocol = (int)netMessage;
            userRegister.userId = id;
            userRegister.password = password;
            string json = JsonUtility.ToJson(userRegister);

            //요청
            string requestUrl = serverURL + "/api/UserAddServices";
            //Debug.Log($"userLogin: {json}");
            POST(requestUrl, json, ReciveResult);
        }

        //유저 등록 응답
        public void NetSendUserRegisterResult(UserRegisterResult registerResult)
        {
            netResult = registerResult.result;
            if (netResult == 0) //유저 등록 성공
            {
                Debug.Log("유저 등록 성공");
            }
            else if (netResult == 1)   //아이디 중복
            {
                Debug.Log("중복된 아이디 입니다");
            }
            else    //로그인 실패
            {
                netFail = true;
                Debug.Log("등록 실패");
            }
        }

        //유저 정보 가져오기 요청
        public void NetSendUserInfo()
        {
            //네트워크 상태 설정
            SetNetMessage(NetMessage.UserInfo);

            //보내는 메세지 가공
            UserInfo userInfo = new UserInfo();
            userInfo.protocol = (int)netMessage;
            userInfo.userId = userId;
            string json = JsonUtility.ToJson(userInfo);

            //요청
            string requestUrl = serverURL + "/api/UserInfoServices";
            //Debug.Log($"userLogin: {json}");
            POST(requestUrl, json, ReciveResult);
        }

        //유저 정보 가져오기 응답
        public void NetSendUserInfoResult(UserInfoResult infoResult)
        {
            netResult = infoResult.result;
            if (netResult == 0) //가져오기 성공
            {
                Debug.Log("가져오기 성공");
                playerStats.Level = infoResult.level;
                playerStats.Gold = infoResult.gold;
            }
            else    //가져오기 실패
            {
                netFail = true;
                Debug.Log("가져오기 실패");
            }
        }

        //유저 레벨업 요청
        public void NetSendUserLevelup()
        {
            //네트워크 상태 설정
            SetNetMessage(NetMessage.Levelup);

            //보내는 메세지 가공
            UserLevelup levelup = new UserLevelup();
            levelup.protocol = (int)netMessage;
            levelup.userId = userId;
            string json = JsonUtility.ToJson(levelup);

            //요청
            string requestUrl = serverURL + "/api/UserLevelupServices";
            //Debug.Log($"userLogin: {json}");
            POST(requestUrl, json, ReciveResult);
        }

        //유저 레벨업 응답
        public void NetSendUserLevelupResult(UserLevelupResult levelupResult)
        {
            netResult = levelupResult.result;
            if (netResult == 0) //레벨업 성공
            {
                Debug.Log("레벨업 성공");
                playerStats.Level = levelupResult.level;
                playerStats.OnChagnedStats?.Invoke(playerStats);
            }
            else    //레벨업 실패
            {
                netFail = true;
                Debug.Log("레벨업 실패");
            }
        }

    }
}
