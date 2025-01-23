using CatDogEnums;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;

public class LobbyUIPanel : UIScene
{
    public Sprite startBtnImg;
    public Sprite matchBtnImg;
    public Image image;
    public TextMeshProUGUI text;

    public float rotationDuration = 2.0f; // 한 바퀴 회전 시간
    private Tween rotationTween;          // DOTween의 트윈 객체 저장용 변수


    enum Buttons
    {
        Button_Start,
        CatButton,
        DogButton,
        SettingButton
    }

    enum Texts
    {
        DogChoiceTxt,
        CatChoiceTxt
    }

    private void Awake()
    {
        Init();
    }

    void GlowText(TextMeshProUGUI text)
    {
        text.color = Color.white; 
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetButton((int)Buttons.Button_Start).gameObject.BindEvent(MatchGame);
        GetButton((int)Buttons.DogButton).gameObject.BindEvent(SelectSpciesDog);
        GetButton((int)Buttons.CatButton).gameObject.BindEvent(SelectSpciesCat);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(SettingPopup);
    }

    public void SelectSpciesDog(PointerEventData data)
    {
        if (GameManager.Instance.Player.Specis != "Dog")
        {
            GameManager.Instance.Player.Specis = "Dog";
            DebugOpt.Log(GameManager.Instance.Player.Specis);
            GlowText(GetTextProUGUI((int)Texts.DogChoiceTxt));
            GetTextProUGUI((int)Texts.CatChoiceTxt).color = Color.gray;
        }
    }

    public void SelectSpciesCat(PointerEventData data)
    {
        if (GameManager.Instance.Player.Specis != "Cat")
        {
            GameManager.Instance.Player.Specis = "Cat";
            DebugOpt.Log(GameManager.Instance.Player.Specis);
            GlowText(GetTextProUGUI((int)Texts.CatChoiceTxt));
            GetTextProUGUI((int)Texts.DogChoiceTxt).color = Color.gray;
        }
    }

    [VisibleEnum(typeof(SpeciesType))]
    public void SelectSpecies(int type)
    {
        SpeciesType speciesType;

        speciesType = (SpeciesType)type;

        if (speciesType == SpeciesType.Dog)
        {
            GameManager.Instance.Player.Specis = "Dog";
        }
        else
        {
            GameManager.Instance.Player.Specis = "Cat";
        }
    }

    public void MatchGame(PointerEventData data)
    {
        if (GameManager.Instance.Player.Specis == null)
        {
            Debug.Log("종족을 선택해주세요");
            return;
        }
        else
        {
            Debug.Log("종족" + GameManager.Instance.Player.Specis);
        }

        image.sprite = matchBtnImg;

        rotationTween = image.transform.DORotate(new Vector3(0, 0, -360), rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)   // 일정한 속도로 회전
                .SetLoops(-1, LoopType.Restart); // 무한 반복

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.MatchRequest = new C2SMatchRequest()
            {
                Species = GameManager.Instance.Player.Specis
            };
            SocketManager.Instance.Send(packet);
        }

        // 매칭하면 다시누를때 취소로 수정
        GetButton((int)Buttons.Button_Start).gameObject.BindEvent(CancelMatchGame);

    }

    public void CancelMatchGame(PointerEventData data)
    {
        image.sprite = startBtnImg;

        // 애니메이션 취소
        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill(); // 트윈 애니메이션 중지
        }

        // 매칭 취소
        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.MatchCancelRequest = new C2SMatchCancelRequest()
            {       
            };
            SocketManager.Instance.Send(packet);
        }

        // 매칭취소하면 다시누를때 매칭으로 수정
        GetButton((int)Buttons.Button_Start).gameObject.BindEvent(MatchGame);
        image.transform.rotation = Quaternion.identity;

    }



    public void MatchGame()
    {
        if (GameManager.Instance.Player.Specis == null)
        {
            Debug.Log("종족을 선택해주세요");
            return;
        }
        else
        {
            Debug.Log("종족" + GameManager.Instance.Player.Specis);
        }

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.MatchRequest = new C2SMatchRequest()
            {
                Species = GameManager.Instance.Player.Specis
            };
            SocketManager.Instance.Send(packet);
        }
    }

    public void SettingPopup ( PointerEventData data)
    {
        UIManager.Instance.ShowPopUI<SettingPanel>();
    }
}
