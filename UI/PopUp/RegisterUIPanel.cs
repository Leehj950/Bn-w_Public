using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static APIModels;

public class RegisterUIPanel : UIPopUp
{

    enum TMP_InputFields
    {
        InputField_UserID,
        InputField_UserEmail,
        InputField_Password
    }

    enum Buttons
    {
        Button_Register,
        Button_Close
    }


    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 바인딩
        Bind<TMP_InputField>(typeof(TMP_InputFields));
        Bind<Button>(typeof(Buttons));

        // 활성화 필수 체크리스트
        GetInputField((int)TMP_InputFields.InputField_UserID).interactable = true;
        GetInputField((int)TMP_InputFields.InputField_UserEmail).interactable = true;
        GetInputField((int)TMP_InputFields.InputField_Password).interactable = true;

        //버튼 이벤트 할당
        GetButton((int)Buttons.Button_Register).gameObject.BindEvent(Register);
        GetButton((int)Buttons.Button_Close).gameObject.BindEvent(Close);

        gameObject.transform.position = Vector3.zero;
    }


    public void Register(PointerEventData data)
    {
        APIModels.RegisterRequest registerRequest = new APIModels.RegisterRequest();

        registerRequest.id = GetInputField((int)TMP_InputFields.InputField_UserID).text;
        registerRequest.password = GetInputField((int)TMP_InputFields.InputField_Password).text;
        registerRequest.email = GetInputField((int)TMP_InputFields.InputField_UserEmail).text;
        var url = APIModels.registerUrl;

        HttpManager.Instance.PostRequest(url, registerRequest,
        (onResponse) =>
        {
            RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(onResponse);

             if (response.success)
             {
                 Debug.Log("회원가입 완료");
                 var popupMessage = UIManager.Instance.ShowPopUI<PopupMessage>();
                 popupMessage.ShowMessage("회원가입이 완료되었습니다.");
                 UIManager.Instance.ClosePopUpUI("RegisterUIPanel");
             }
             else
             {
                 var popupMessage = UIManager.Instance.ShowPopUI<PopupMessage>();
                 popupMessage.ShowMessage(response.message);

             }
        });
    }

    public void Close(PointerEventData data)
    {
        ClosePopupUI();
    }
}
