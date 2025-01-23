using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    int order = 10;

    Stack<UIPopUp> popupStack = new Stack<UIPopUp>();
    UIScene uiScene = new UIScene();

    public UIScene UIScene { get { return uiScene; }  set { uiScene = value; } }  

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("Canvas");
            if (root == null)
            {
                root = new GameObject { name = "Canvas" };
            }
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = UIUtil.GetOrrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = order;
            order++;
        }
        else
        {
            canvas.sortingOrder = 10;
        }
    }

    public T ShoWSceneUI<T>(string name = null) where T : UIScene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = ResourceManager.Instance.Instantiate($"UI/Scene/{name}");
       
        T sceneUI = UIUtil.GetOrrAddComponent<T>(go);
        uiScene = sceneUI;

        go.transform.SetParent(Root.transform);
        
        go.transform.localScale = Vector3.one;

        return sceneUI;
    }

    public T ShowPopUI<T>(string name = null) where T : UIPopUp
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = ResourceManager.Instance.Instantiate($"UI/PopUp/{name}");

        T popup = UIUtil.GetOrrAddComponent<T>(go);
        popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        
        return popup;
    }

    public void ClosePopUpUI(UIPopUp popup)
    {
        if (popupStack.Count == 0)
        {
            return;
        }

        if (popupStack.Peek() != popup)
        {
            Debug.Log("가장위가 아니라 삭제 불가");
            return;
        }

        ClosePopUpUI();
    }

    public void ClosePopUpUI(string name)
    {
        if(popupStack.Count == 0)
        {
            return;
        }

        UIPopUp _uIPopUp = popupStack.Peek();
        // 가장 위에 있는 지 확인
        if(_uIPopUp.name == name)
        {
            ClosePopUpUI();
        }
    }


    private void ClosePopUpUI()
    {
        if (popupStack.Count == 0)
        {
            return;
        }
        UIPopUp popup = popupStack.Pop();
        ResourceManager.Instance.Destroy(popup.gameObject);
        popup = null;
        order--;
    }
}
