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

    // ---------- Request ����ü ----------

    #region Request

    public class LoginRequest
    {
        public string id; // ����� �̸�
        public string password; // ����� ��й�ȣ
    }

    public class RegisterRequest
    {
        public string id;
        public string password; 
        public string email; 
    }



    #endregion

    // ---------- Response ����ü ----------

    #region Response

    public class CommonResponse
    {
        public bool success;     // ���� ����
        public string message;   // ���� �޽���
    }

    public class LoginResponse : CommonResponse
    {
        public string token;     // JWT ��ū
        public string userId;    // ����� ID
    }

    public class RegisterResponse : CommonResponse
    {

    }

    #endregion
}
