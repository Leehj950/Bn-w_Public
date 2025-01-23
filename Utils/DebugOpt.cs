using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public static class DebugOpt
{
#if USE_DEBUG
    // �⺻���� �α�
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    // �α� �۾� �߰�
    public static void LogForamt(string message, object context)
    {
        Debug.LogFormat(message, context);
    }

    // ���â
    public static void LogWarning(string message) 
    {
        Debug.LogWarning(message);
    }
    // ���â �۾� �߰�
    public static void LogWarning(string message, UnityEngine.Object context)
    {
        Debug.LogWarning(message, context);
    }

    // ���� 
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    // ���� �۾� �߰�
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
