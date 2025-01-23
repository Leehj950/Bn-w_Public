using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private CharacterTooltip tooltip; // ���� ������Ʈ
    [SerializeField] private float holdTime = 1f; // ��ư�� ������ �ϴ� �ð� (1��)
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;

    private void Update()
    {
        // ��ư�� ������ �ִ� ���� Ÿ�̸Ӹ� ����
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
        // ��ư�� ������ ����
        isPointerDown = true;
        pointerDownTimer = 0f; // Ÿ�̸� �ʱ�ȭ
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ��ư���� ���� ���� �� ���� ����
        isPointerDown = false;
        pointerDownTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        HideTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ��ư ������ ����� ���� ����
        isPointerDown = false;
        pointerDownTimer = 0f; // Ÿ�̸� �ʱ�ȭ
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
