using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : UIPopUp
{
    enum Buttons
    {
        Button_Lobby,
        Button_Close
    }

    enum Sliders
    {
        SliderBar_Sfx,
        SliderBar_BGM,
    }

    public void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));

        GetSlider((int)Sliders.SliderBar_Sfx).onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        GetSlider((int)Sliders.SliderBar_BGM).onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);

        GetSlider((int)Sliders.SliderBar_Sfx).value = SoundManager.Instance.sfxVolume;
        GetSlider((int)Sliders.SliderBar_BGM).value = SoundManager.Instance.bgmVolume;
        
        if (SceneManager.GetActiveScene().name != "BattleScene")
        {
            GetButton((int)Buttons.Button_Lobby).gameObject.SetActive(false);
        }
        else
        {
            GetButton((int)Buttons.Button_Lobby).gameObject.BindEvent(Lobby);
        }

        GetButton((int)Buttons.Button_Close).gameObject.BindEvent(Close);
    }

    public void Lobby(PointerEventData data)
    {
        SocketManager.Instance.Disconnect();
        SocketManager.Instance.Init(APIModels.ip, APIModels.lobbyPort);
        SocketManager.Instance.Connect(() =>
        {
            SceneManager.LoadScene("LobbyScene");
            if (BattleManager.Instance != null)
            {
                SoundManager.Instance.PlayBGM(BattleManager.Instance.startSceneBgmClip);
            }
            if (SocketManager.Instance.isConnected)
            {
                // 연결 성공 시 요청 전송
                var authRequest = new C2SAuthRequest();
                authRequest.Token = GameManager.Instance.jwtToken;

                GamePacket packet = new GamePacket();
                packet.AuthRequest = authRequest;
                SocketManager.Instance.Send(packet);
                Debug.Log("Auth request sent!");
            }
        });
    }

    public void Close(PointerEventData data)
    {
        ClosePopupUI();
    }
}
