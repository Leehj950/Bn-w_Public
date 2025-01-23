using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public AssetIdType assetIdType;
    [SerializeField] private SpawnPointBtn spawnPointBtn;
    public Image characterCardImg;

    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }


    private void OnEnable()
    {
        characterCardImg = GetComponent<Image>();
        spawnPointBtn = FindAnyObjectByType<SpawnPointBtn>();
    }

    public void UpdateAssetIdType(AssetIdType IdType)
    {
        assetIdType = IdType;
    }
    public void UpdateImage(Sprite sprite)
    {
       characterCardImg.sprite = sprite;
    }
    public void UpdateImage()
    {
        Sprite sprite = ResourceManager.Instance.Load<Sprite>("Images/" + assetIdType.ToString());
        characterCardImg.sprite = sprite;
    }


    public void RequestSpawnCharacter()
    {
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
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }

}

