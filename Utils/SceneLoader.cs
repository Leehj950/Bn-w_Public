using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        // �� �ε� �Ϸ� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            Debug.Log("BattleScene fully loaded!");
            HandleBattleSceneLoaded();
        }
    }

    private void HandleBattleSceneLoaded()
    {
        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.GameStartRequest = new C2SGameStartRequest()
            {
            };
            SocketManager.Instance.Send(packet);
        }
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
