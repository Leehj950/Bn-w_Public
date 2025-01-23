using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private AudioClip bgmClip;

    //�÷��̾� ���� �������� ������ �ִ�.
    private Player player;
    public Player Player { get { return player; } }

    public string jwtToken;


    private void Start()
    {
        Application.targetFrameRate = 60;
        player = new Player();
    }


    public IEnumerator WaitForBattleSceneAndInitialize(GamePacket gamePacket)
    {
        // BattleScene �ε�
        SceneManager.LoadScene("BattleScene");

        // BattleManager�� ������ ������ ���
        while (BattleManager.Instance == null)
        {
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ���� �˸� ó��
        var response = gamePacket.GameStartNotification;

        Debug.Log("���� ����");
        Debug.Log(response.Species);

        // BattleManager �ʱ�ȭ
        if (response.Species == "dog")
        {
            BattleManager.Instance.isDog = true;
        }
        else if (response.Species == "cat")
        {
            BattleManager.Instance.isDog = false;
        }

        Debug.Log(BattleManager.Instance.isDog);

        // CastleManager �ʱ�ȭ
        CastleManager.Instance.CastleSpawn(BattleManager.Instance.isDog);

        foreach (CreateBtn btn in BattleManager.Instance.createBtns)
        {
            btn.Init();
        }
    }
}
