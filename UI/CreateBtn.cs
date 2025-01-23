using CatDogEnums;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Land;

public class CreateBtn : MonoBehaviour
{
    //public Button uiButton;

    private AssetIdType assetIdType;
    public AssetIdType dogAssetIdType;
    public AssetIdType catAssetIdType;
    public CreateType createType;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public CreateBtn characterBtn;
    public SpawnPointBtn spawnPointBtn;
    private Image btnImg;

    List<CreateBtn> createBtns = new List<CreateBtn>();
    private void Awake()
    {
        createBtns.AddRange(FindObjectsOfType<CreateBtn>());
        btnImg = GetComponent<Image>(); 
    }

    private void Start()
    {
        spawnPointBtn = FindAnyObjectByType<SpawnPointBtn>();

        if (createType == CreateType.Building)
        {
            nameText.text += "건물";
        }

        // 초기 버튼 이벤트 설정
        //if (uiButton != null)
        //{
        //    uiButton.onClick.AddListener(Buy);
        //}
    }

    public void Init()
    {
        if (BattleManager.Instance.isDog)
        {
            assetIdType = dogAssetIdType;
        }
        else
        {
            assetIdType = catAssetIdType;
        }

        // 가격 초기화
        switch (createType)
        {
            case CreateType.Character:
                Character character = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Character>();
                costText.text = character.characterData.Cost.ToString();
                // 캐릭터 버튼이면 비활성화
                gameObject.SetActive(false);
                break;
            case CreateType.Building:
                Building building = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Building>();
                costText.text = building.cost.ToString();
                break;
        }

        // 이름 초기화
        nameText.text = assetIdType.ToString();

        Sprite sprite; 

        // 버튼 이미지 초기화
        if (BattleManager.Instance.isDog)
        {
            sprite = ResourceManager.Instance.Load<Sprite>("Images/" + dogAssetIdType.ToString());       
        }
        else
        {
            sprite = ResourceManager.Instance.Load<Sprite>("Images/" + catAssetIdType.ToString());
        }
        btnImg.sprite = sprite;
    }

    public void Buy()
    {
        switch (createType)
        {
            case CreateType.Building:
                RequestBuilding();
                break;
            case CreateType.Character:
                RequestSpawnCharacter();
                break;
        }
    }

    void RequestBuilding()
    {
        Building building = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Building>();
        if(building.cost > CurrencyManager.Instance.currency)
        {
            Debug.Log("사료가 부족합니다");
            return;
        }

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.PurchaseBuildingRequest = new C2SPurchaseBuildingRequest()
            {
                AssetId = (int)assetIdType
            };
            SocketManager.Instance.Send(packet);

            // Todo 지금은 여기서 건물 버튼을 비활성화 하지만 Response에서 응답이 제대로 오면 해주는것으로 수정해야함
            gameObject.SetActive(false);
            characterBtn.gameObject.SetActive(true);
        }

    }

    void RequestSpawnCharacter()
    {
        Character character = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Character>();
        if (character.characterData.Cost > CurrencyManager.Instance.currency)
        {
            Debug.Log("사료가 부족합니다");
            return;
        }

        // TODO ToTop 값 받도록 수정필요

        Debug.Log((int)assetIdType);

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            var spawnUnitRequest = new C2SSpawnUnitRequest();
                spawnUnitRequest.AssetId = (int)assetIdType;
                spawnUnitRequest.ToTop = spawnPointBtn.isLeft;
                packet.SpawnUnitRequest = spawnUnitRequest;

            SocketManager.Instance.Send(packet);
        }
    }



    //public void SwitchToCharacterMode()
    //{
    //    if (uiButton == null)
    //    {
    //        Debug.LogWarning("UI Button is not assigned!");
    //        return;
    //    }

    //    // 기존 이벤트 제거
    //    uiButton.onClick.RemoveAllListeners();

    //    // 새로운 이벤트 추가
    //    uiButton.onClick.AddListener(BuyCharacter);

    //    Debug.Log("버튼이 BuyCharacter 모드로 전환되었습니다.");
    //}
}
