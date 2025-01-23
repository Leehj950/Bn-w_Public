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

        // ���ε�
        Bind<TMP_InputField>(typeof(TMP_InputFields));
        Bind<Button>(typeof(Buttons));

        // Ȱ��ȭ �ʼ� üũ����Ʈ
        GetInputField((int)TMP_InputFields.InputField_UserID).interactable = true;
        GetInputField((int)TMP_InputFields.InputField_UserEmail).interactable = true;
        GetInputField((int)TMP_InputFields.InputField_Password).interactable = true;

        //��ư �̺�Ʈ �Ҵ�
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
                 Debug.Log("ȸ������ �Ϸ�");
                 var popupMessage = UIManager.Instance.ShowPopUI<PopupMessage>();
                 popupMessage.ShowMessage("ȸ�������� �Ϸ�Ǿ����ϴ�.");
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
