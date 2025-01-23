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
            nameText.text += "�ǹ�";
        }

        // �ʱ� ��ư �̺�Ʈ ����
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

        // ���� �ʱ�ȭ
        switch (createType)
        {
            case CreateType.Character:
                Character character = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Character>();
                costText.text = character.characterData.Cost.ToString();
                // ĳ���� ��ư�̸� ��Ȱ��ȭ
                gameObject.SetActive(false);
                break;
            case CreateType.Building:
                Building building = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Building>();
                costText.text = building.cost.ToString();
                break;
        }

        // �̸� �ʱ�ȭ
        nameText.text = assetIdType.ToString();

        Sprite sprite; 

        // ��ư �̹��� �ʱ�ȭ
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
            Debug.Log("��ᰡ �����մϴ�");
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

            // Todo ������ ���⼭ �ǹ� ��ư�� ��Ȱ��ȭ ������ Response���� ������ ����� ���� ���ִ°����� �����ؾ���
            gameObject.SetActive(false);
            characterBtn.gameObject.SetActive(true);
        }

    }

    void RequestSpawnCharacter()
    {
        Character character = BattleManager.Instance.GetPrefabByAssetId((int)assetIdType).GetComponent<Character>();
        if (character.characterData.Cost > CurrencyManager.Instance.currency)
        {
            Debug.Log("��ᰡ �����մϴ�");
            return;
        }

        // TODO ToTop �� �޵��� �����ʿ�

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

    //    // ���� �̺�Ʈ ����
    //    uiButton.onClick.RemoveAllListeners();

    //    // ���ο� �̺�Ʈ �߰�
    //    uiButton.onClick.AddListener(BuyCharacter);

    //    Debug.Log("��ư�� BuyCharacter ���� ��ȯ�Ǿ����ϴ�.");
    //}
}
