using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private CharacterTooltip tooltip; // 툴팁 오브젝트
    [SerializeField] private float holdTime = 1f; // 버튼을 눌러야 하는 시간 (1초)
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;

    private void Update()
    {
        // 버튼을 누르고 있는 동안 타이머를 증가
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= holdTime)
            {
                ShowTooltip();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼을 누르기 시작
        isPointerDown = true;
        pointerDownTimer = 0f; // 타이머 초기화
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 버튼에서 손을 뗐을 때 툴팁 숨김
        isPointerDown = false;
        pointerDownTimer = 0f; // 타이머 초기화
        HideTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 버튼 영역을 벗어나면 툴팁 숨김
        isPointerDown = false;
        pointerDownTimer = 0f; // 타이머 초기화
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if(tooltip == null)
        {
            tooltip = UIManager.Instance.ShowPopUI<CharacterTooltip>();
        }
        
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(true);

            CharacterCard card = GetComponent<CharacterCard>();
            if (card != null)
            {
                tooltip.SetupTooltip(card.assetIdType, card.characterCardImg);
            }
        }

    }

    private void HideTooltip()
    {
        
        if (tooltip != null && tooltip.gameObject.activeSelf)
        {
            tooltip.ClosePopupUI();
        }
    }

 
}
