using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static APIModels;

public class LoginUIPanel : UIPopUp
{
    enum TMP_InputFields
    {
        InputField_UserID,
        InputField_Password
    }

    enum Buttons
    {
        Button_SignIn,
        Button_Close
    }


    public void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        // 바인딩
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(TMP_InputFields));
        // 활성화 필수 체크리스트
        GetInputField((int)TMP_InputFields.InputField_UserID).interactable = true;
        GetInputField((int)TMP_InputFields.InputField_UserID).textComponent.raycastTarget = true;
        // 버튼 이벤트 할당
        GetButton((int)Buttons.Button_SignIn).gameObject.BindEvent(Login);
        GetButton((int)Buttons.Button_Close).gameObject.BindEvent(Close);

        gameObject.transform.position = Vector3.zero;
    }

    public void Login(PointerEventData data)
    {
        APIModels.LoginRequest loginRequest = new APIModels.LoginRequest();

        loginRequest.id = GetInputField((int)TMP_InputFields.InputField_UserID).text;
        loginRequest.password = GetInputField((int)TMP_InputFields.InputField_Password).text;
        var url = APIModels.loginUrl;

        HttpManager.Instance.PostRequest(url, loginRequest,
        (onResponse) => {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(onResponse);

            if (response.success)
            {
                GameManager.Instance.jwtToken = response.token;
                GameManager.Instance.Player.Id = response.userId;

                UIManager.Instance.ClosePopUpUI("LoinUIPanel");

                SceneManager.LoadScene("LobbyScene");

                SocketManager.Instance.Init(APIModels.ip, APIModels.lobbyPort);
                SocketManager.Instance.Connect(() => {
                    if (SocketManager.Instance.isConnected)
                    {
                        // 연결 성공 시 요청 전송
                        var authRequest = new C2SAuthRequest();
                        authRequest.Token = GameManager.Instance.jwtToken;

                        GamePacket packet = new GamePacket();
                        packet.AuthRequest = authRequest;
                        SocketManager.Instance.Send(packet);
                    }
                });

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


