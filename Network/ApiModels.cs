using System;
using UnityEngine;

[System.Serializable]
public static class APIModels
{
    public static string ip = "34.47.112.246";
    public static int certificationPort = 5555;
    public static int lobbyPort = 5959;

    // public static string certificationUrl = "http://34.47.112.246:5555/";
    public static string certificationUrl = "https://ddori.site:5555/";
    public static string lobbyUrl = "http://34.47.112.246:5959/";
    public static string loginUrl => certificationUrl + "login";
    public static string registerUrl => certificationUrl + "signup";

    // ---------- Request 구조체 ----------

    #region Request

    public class LoginRequest
    {
        public string id; // 사용자 이름
        public string password; // 사용자 비밀번호
    }

    public class RegisterRequest
    {
        public string id;
        public string password; 
        public string email; 
    }



    #endregion

    // ---------- Response 구조체 ----------

    #region Response

    public class CommonResponse
    {
        public bool success;     // 성공 여부
        public string message;   // 서버 메시지
    }

    public class LoginResponse : CommonResponse
    {
        public string token;     // JWT 토큰
        public string userId;    // 사용자 ID
    }

    public class RegisterResponse : CommonResponse
    {

    }

    #endregion
}
