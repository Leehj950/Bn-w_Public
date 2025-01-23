using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartUIPanel : UIScene
{

    [SerializeField] private AudioClip bgmClip;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(bgmClip, true);
    }

    enum Buttons
    {
        Button_Price_Coin_Login,
        Button_Price_Coin_Register,
        SettingButton
    }

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Button_Price_Coin_Login).gameObject.BindEvent(LoginPopUp);
        GetButton((int)Buttons.Button_Price_Coin_Register).gameObject.BindEvent(RegisterPopUp);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(SettingPopUp);
    }

    private void LoginPopUp(PointerEventData data)
    {
        UIManager.Instance.ShowPopUI<LoginUIPanel>();
    }
    private void RegisterPopUp(PointerEventData data)
    {
        UIManager.Instance.ShowPopUI<RegisterUIPanel>();
    }
    
    private void SettingPopUp(PointerEventData data)
    {
        UIManager.Instance.ShowPopUI<SettingPanel>();
    }
}
