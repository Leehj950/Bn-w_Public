using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class HttpManager : Singleton<HttpManager>
{
    private Coroutine get; 
    private Coroutine post; 
    private Coroutine postJwt; 

    public void GetRequest(string url, Action<string> onResponse)
    {
        StartCoroutine(GetRequestCor(url,onResponse));
    }

    private IEnumerator GetRequestCor(string url, Action<string> onResponse)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            onResponse?.Invoke(webRequest.downloadHandler.text);

            DebugOpt.Log("Response: " + webRequest.downloadHandler.text);
        }
    }

    public void PostRequestWithJwt(string url, object data, Action<string> onResponse)
    {
        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(PostRequestCor(url, jsonData, onResponse));
    }

    private IEnumerator PostRequestWithJwtCor(string url, string jsonData, Action<string> onResponse)
    {
        // JSON ������ ����
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Content-Type ����
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // JWT ��ū �߰�
            string jwtToken = GameManager.Instance.jwtToken;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                webRequest.SetRequestHeader("Authorization", jwtToken);
            }

            // ��û ����
            yield return webRequest.SendWebRequest();

            onResponse?.Invoke(webRequest.downloadHandler.text);

            DebugOpt.Log("Response: " + webRequest.downloadHandler.text);


        }
    }

    public void PostRequest(string url, object data, Action<string> onResponse)
    {
        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(PostRequestCor(url, jsonData, onResponse));
    }

    private IEnumerator PostRequestCor(string url, string jsonData, Action<string> onResponse)
    {
        // JSON ������ ����
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Content-Type ����
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // ��û ����
            yield return webRequest.SendWebRequest();

            onResponse?.Invoke(webRequest.downloadHandler.text);

            DebugOpt.Log("Response: " + webRequest.downloadHandler.text);
        }
    }




}
