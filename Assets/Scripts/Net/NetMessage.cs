using System;

namespace MySampleEx
{
    //프로토롤 번호 정의
    public enum NetMessage
    {
        None = -1,
        Version = 0,
        Login = 1101,
        RegisterUser,
        UserInfo,
        Levelup
    }

    //로그인 요청
    [Serializable]
    public class UserLogin
    {
        public int protocol;
        public string userId;
        public string password;
    }

    //로그인 응답
    [Serializable]
    public class UserLoginResult
    {
        public int protocol;
        public int result;
        public string userId;
    }

    //유저 등록 요청
    [Serializable]
    public class UserRegister
    {
        public int protocol;
        public string userId;
        public string password;
    }

    //유저 등록 응답
    [Serializable]
    public class UserRegisterResult
    {
        public int protocol;
        public int result;
        public string userId;
    }

    //유저 정보 가져오기 요청
    public class UserInfo
    {
        public int protocol;
        public string userId;
    }

    //유저 정보 가져오기 응답
    public class UserInfoResult
    {
        public int protocol;
        public int result;
        public string userId;
        public int level;
        public int gold;
    }

    //유저 레벨업 요청
    public class UserLevelup
    {
        public int protocol;
        public string userId;
    }

    //유저 레벨업 응답
    public class UserLevelupResult
    {
        public int protocol;
        public int result;
        public string userId;
        public int level;
    }

}