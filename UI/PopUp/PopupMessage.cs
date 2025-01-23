using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupMessage : UIPopUp
{
    [SerializeField] private RectTransform messageTransform; // �޽��� UI�� RectTransform
    [SerializeField] private float durationToCenter = 0.5f;  // �޽����� �߾����� �ö���� �ð�
    [SerializeField] private float stayDuration = 1.0f;      // �߾ӿ��� �ӹ��� �ð�
    [SerializeField] private float durationToExit = 0.5f;    // �޽����� �ٽ� �������� �ð�

    enum Texts
    {
        Text_Info
    }


    public void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        // ���ε�
        Bind<TextMeshProUGUI>(typeof(Texts));

        gameObject.transform.position = Vector3.zero;
    }

   public void ShowMessage(string message)
    {
        TextMeshProUGUI text = GetTextProUGUI((int)Texts.Text_Info);
        text.text = message;
        PlayMessageAnimation();
    }


    public void Close(PointerEventData data)
    {
        ClosePopupUI();
    }

    public void PlayMessageAnimation()
    {
        // �ʱ� ��ġ�� ȭ�� �Ʒ��� ����
        messageTransform.anchoredPosition = new Vector2(0, -Screen.height);

        // DOTween Sequence ����
        Sequence seq = DOTween.Sequence();

        // 1. ȭ�� �߾����� �ö���� �ִϸ��̼�
        seq.Append(messageTransform.DOAnchorPos(Vector2.zero, durationToCenter).SetEase(Ease.OutCubic));

        // 2. �߾ӿ��� 3�� ���� ���
        seq.AppendInterval(stayDuration);

        // 3. ȭ�� �Ʒ��� �������� �ִϸ��̼�
        seq.Append(messageTransform.DOAnchorPos(new Vector2(0, -Screen.height), durationToExit).SetEase(Ease.InCubic));

        // Sequence�� ���� �� Close ����
        seq.OnComplete(() => ClosePopupUI());

        // Sequence ����
        seq.Play();

    }
}
