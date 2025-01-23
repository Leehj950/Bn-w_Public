using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public static class DebugOpt
{
#if USE_DEBUG
    // 기본적인 로그
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    // 로그 글씨 추가
    public static void LogForamt(string message, object context)
    {
        Debug.LogFormat(message, context);
    }

    // 경고창
    public static void LogWarning(string message) 
    {
        Debug.LogWarning(message);
    }
    // 경고창 글씨 추가
    public static void LogWarning(string message, UnityEngine.Object context)
    {
        Debug.LogWarning(message, context);
    }

    // 에러 
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    // 에러 글씨 추가
    public static void LogErrorFormat(string message, UnityEngine.Object context) 
    {
        Debug.LogErrorFormat(message, context);
    }
#else
    public static void Log(object message) { }
    public static void LogForamt(string message, object context) { }
    public static void LogWarning(string message) { }
    public static void LogWarning(string message, UnityEngine.Object context) { }
    public static void LogError(string message) { }
    public static void LogErrorFormat(string message, UnityEngine.Object context) { }
#endif

}
