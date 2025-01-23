using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] name = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[name.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < name.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = UIUtil.FindChild(gameObject, name[i], true);
            }
            else
            {
                objects[i] = UIUtil.FindChild<T>(gameObject, name[i], true);
            }

            if (objects[i] == null)
            {
                Debug.Log($"bind Failed");
            }
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }

        return objects[idx] as T;
    }

    protected GameObject GetGameObject(int idx) { return Get<GameObject>(idx); }
    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected TextMeshProUGUI GetTextProUGUI(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected TMP_InputField GetInputField(int idx) { return Get<TMP_InputField>(idx); }
    protected RawImage GetRawImage(int idx) { return Get<RawImage>(idx); }
    protected Slider GetSlider(int idx) { return Get<Slider>(idx); }

    public static void BindEvent(GameObject gameObject, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UIEventHandler evt = UIUtil.GetOrrAddComponent<UIEventHandler>(gameObject);

        switch (type)
        {
            case Define.UIEvent.Click:
                //evt.OnClickHandler -= action;
                evt.OnClickHandler = null;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                // evt.OnDragHandler -= action;
                evt.OnDragHandler = null;
                evt.OnDragHandler += action;
                break;
        }
    }
}
