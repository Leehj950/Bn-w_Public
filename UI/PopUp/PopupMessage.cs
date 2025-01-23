using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupMessage : UIPopUp
{
    [SerializeField] private RectTransform messageTransform; // 메시지 UI의 RectTransform
    [SerializeField] private float durationToCenter = 0.5f;  // 메시지가 중앙으로 올라오는 시간
    [SerializeField] private float stayDuration = 1.0f;      // 중앙에서 머무는 시간
    [SerializeField] private float durationToExit = 0.5f;    // 메시지가 다시 내려가는 시간

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
        // 바인딩
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
        // 초기 위치를 화면 아래로 설정
        messageTransform.anchoredPosition = new Vector2(0, -Screen.height);

        // DOTween Sequence 생성
        Sequence seq = DOTween.Sequence();

        // 1. 화면 중앙으로 올라오는 애니메이션
        seq.Append(messageTransform.DOAnchorPos(Vector2.zero, durationToCenter).SetEase(Ease.OutCubic));

        // 2. 중앙에서 3초 동안 대기
        seq.AppendInterval(stayDuration);

        // 3. 화면 아래로 내려가는 애니메이션
        seq.Append(messageTransform.DOAnchorPos(new Vector2(0, -Screen.height), durationToExit).SetEase(Ease.InCubic));

        // Sequence가 끝난 후 Close 실행
        seq.OnComplete(() => ClosePopupUI());

        // Sequence 실행
        seq.Play();

    }
}
