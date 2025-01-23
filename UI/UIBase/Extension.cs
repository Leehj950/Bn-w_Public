using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public static class Extension
{
    public static T GetOrAddcomponent<T>(this GameObject gameObject) where T : UnityEngine.Component
    {
        return UIUtil.GetOrrAddComponent<T>(gameObject);
    }

    public static void BindEvent(this GameObject gameObject, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UIBase.BindEvent(gameObject, action, type);
    }
}

