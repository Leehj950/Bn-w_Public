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

    //플레이어 개인 정보값을 가지고 있다.
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
        // BattleScene 로드
        SceneManager.LoadScene("BattleScene");

        // BattleManager가 생성될 때까지 대기
        while (BattleManager.Instance == null)
        {
            yield return null; // 다음 프레임까지 대기
        }

        // 게임 시작 알림 처리
        var response = gamePacket.GameStartNotification;

        Debug.Log("게임 시작");
        Debug.Log(response.Species);

        // BattleManager 초기화
        if (response.Species == "dog")
        {
            BattleManager.Instance.isDog = true;
        }
        else if (response.Species == "cat")
        {
            BattleManager.Instance.isDog = false;
        }

        Debug.Log(BattleManager.Instance.isDog);

        // CastleManager 초기화
        CastleManager.Instance.CastleSpawn(BattleManager.Instance.isDog);

        foreach (CreateBtn btn in BattleManager.Instance.createBtns)
        {
            btn.Init();
        }
    }
}
