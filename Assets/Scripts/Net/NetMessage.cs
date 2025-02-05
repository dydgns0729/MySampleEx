using System;

namespace MySampleEx
{
    //프로토콜 번호 정의
    public enum NetMessage
    {
        None = -1,
        Version = 0,
        Login = 1101,
        RegisterUser,
        UserInfo,
        LevelUp
    }

    //로그인 요청
    [Serializable]
    public class UserLogin
    {
        public int protocol;
        public string userId;
        public string passWord;
    }

    //로그인 응답
    [Serializable]
    public class UserLoginResult
    {
        public int protocol;
        public int result;
        public string userId;
    }

    //유저등록 요청
    [Serializable]
    public class UserRegister
    {
        public int protocol;
        public string userId;
        public string passWord;
    }

    //유저등록 응답
    [Serializable]
    public class UserRegisterResult
    {
        public int protocol;
        public int result;
        public string userId;
    }

    //유저정보 가져오기 요청
    [Serializable]
    public class UserInfo
    {
        public int protocol;
        public string userId;
    }

    //유저정보 가져오기 응답
    [Serializable]
    public class UserInfoResult
    {
        public int protocol;
        public int result;
        public string userId;
        public int level;
        public int gold;
    }

    //레벨업 요청
    [Serializable]
    public class UserLevelUp
    {
        public int protocol;
        public string userId;
    }

    //레벨업 응답
    [Serializable]
    public class UserLevelUpResult
    {
        public int protocol;
        public int result;
        public string userId;
        public int level;
    }
}