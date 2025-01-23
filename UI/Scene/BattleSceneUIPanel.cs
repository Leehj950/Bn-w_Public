using CatDogEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleSceneUIPanel : UIScene
{
    enum Buttons
    {
        None,
        FirstStepButton,
        SecondStepButton,
        ThirdStepButton,
        OneButton,
        TwoButton,
        ThreeStepButton,
        FourButton,
        FiveButton,
        SixButton,
        SevenButton,
        EightButton,
        LoadButton,
        SettingButton 
    }

    enum Texts
    {
        CurrencyTxt,
        PlayTimeTxt,
    }
    enum Images
    {
        PetFoodImg
    }

    List<GameObject> buttonList = new List<GameObject>();
    int SelectEnum;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        UIManager.Instance.UIScene = this;
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetButton((int)Buttons.FirstStepButton).gameObject.BindEvent(SelectStepButton);
        GetButton((int)Buttons.SecondStepButton).gameObject.BindEvent(SelectStepButton);
        GetButton((int)Buttons.ThirdStepButton).gameObject.BindEvent(SelectStepButton);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(SettingButton);

        for (int i = (int)Buttons.OneButton; i <= (int)Buttons.EightButton; i++)
        {
            GetButton(i).gameObject.SetActive(false);
            GetButton(i).gameObject.BindEvent(RequestSpawnCharacter);
            buttonList.Add(GetButton(i).gameObject);
        }

        SelectStepButtonSeting();
    }


    void SelectStepButtonSeting()
    {
        if (GameManager.Instance.Player.Specis == "Cat")
        {
            GetButton((int)Buttons.FirstStepButton).gameObject.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/cat20");
            GetButton((int)Buttons.SecondStepButton).gameObject.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/cat40");
            GetButton((int)Buttons.ThirdStepButton).gameObject.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/cat80");
        }
        else
        {
            GetButton((int)Buttons.FirstStepButton).gameObject.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/dog20");
            GetButton((int)Buttons.SecondStepButton).gameObject.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/dog40");
            GetButton((int)Buttons.ThirdStepButton).gameObject.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/dog80");
        }
    }

    public void SelectStepButton(PointerEventData data)
    {
        //방어 코드
        if (BattleManager.Instance.AssetidList.Count == buttonList.Count)
        {
            return;
        }

        string buttonObjectName = EventSystem.current.currentSelectedGameObject.name;

        for (int i = 1; i < 4; i++)
        {
            if (buttonObjectName == Enum.GetName(typeof(Buttons), i))
            {
                SelectEnum = i;
                break;
            }
        }

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.DrawCardRequest = new C2SDrawCardRequest()
            {
                ButtonType = SelectEnum
            };
            SocketManager.Instance.Send(packet);
        }
    }
    public void UpdataButtonList()
    {

        if (BattleManager.Instance.AssetidList == null)
        {
            return;
        }

        int minSize = Mathf.Min(buttonList.Count, BattleManager.Instance.AssetidList.Count);


        for (int i = 0; i < minSize; i++)
        {
            buttonList[i].SetActive(true);

            CharacterCard characterCard = buttonList[i].GetComponent<CharacterCard>();

            if (characterCard == null)
            {
                DebugOpt.LogError("캐릭터 카드 에러");
            }

            characterCard.UpdateAssetIdType((AssetIdType)BattleManager.Instance.AssetidList[i]);

            characterCard.UpdateImage();
        }

        for (int i = minSize; i < buttonList.Count; i++)
        {
            buttonList[i].SetActive(false);
        }
    }

    public void RequestSpawnCharacter(PointerEventData data)
    {
        string objname = EventSystem.current.currentSelectedGameObject.name;

        int AssetCount = new int();

        for (int i = 0; i < buttonList.Count; i++)
        {
            if (buttonList[i].name == objname)
            {
                buttonList[i].GetComponent<CharacterCard>().RequestSpawnCharacter();
                AssetCount = i;
                BattleManager.Instance.AssetidList.Remove(BattleManager.Instance.AssetidList[i]);
            }
        }

        UpdataButtonList();
    }

    public void SettingButton(PointerEventData data)
    {
        UIManager.Instance.ShowPopUI<SettingPanel>();
    }    
}
