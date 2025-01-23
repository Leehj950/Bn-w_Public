using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleManager : Singleton<BattleManager>
{
    float playTime = 0;
    float PlayTime { get { return playTime; } }
    public TextMeshProUGUI playTimeText;
    public bool isDog;
    public List<AssetPrefab> assetPrefabs;
    private Dictionary<int, GameObject> assetPrefabDict;
    public CreationSystem creationSystem;

    // ĳ����
    public static Dictionary<int, Character> activeCharacterDic;
    public static Dictionary<int, Castle> activeBuildingDic;
    private Coroutine sendLocationNotificationCor;

    public List<int> AssetidList = new List<int>();

    // �����ʿ�
    public CreateBtn[] createBtns;

    public float locationNotificationDelay = 1f;
    [SerializeField] private float gameOverDelay = 5f;
    public bool isGameOver = false;    

    [System.Serializable]
    public class AssetPrefab
    {
        public int assetId;
        public GameObject prefab;
    }

    [SerializeField] AudioClip battleBgmClip;
    public AudioClip startSceneBgmClip;

    public override void Awake()
    {
        base.Awake();

        // Dictionary �ʱ�ȭ
        activeCharacterDic = new Dictionary<int, Character>();
        activeBuildingDic = new Dictionary<int, Castle>();
        assetPrefabDict = new Dictionary<int, GameObject>();
        foreach (var assetPrefab in assetPrefabs)
        {
            if (!assetPrefabDict.ContainsKey(assetPrefab.assetId))
            {
                assetPrefabDict.Add(assetPrefab.assetId, assetPrefab.prefab);
            }
        }

        createBtns = FindObjectsOfType<CreateBtn>();
        playTimeText = GameObject.Find("PlayTimeTxt").GetComponent<TextMeshProUGUI>();
        creationSystem = FindObjectOfType<CreationSystem>();

    }

    private void Start()
    {
        sendLocationNotificationCor = StartCoroutine(SendLocationNotificationCor());

        SoundManager.Instance.PlayBGM(battleBgmClip);
    }


    private void Update()
    {
        UpdatePlayTimeUI();
    }

    public Character GetCharacterById(int unitId)
    {
        if (activeCharacterDic.TryGetValue(unitId, out var character))
        {
            return character;
        }

        Debug.LogWarning($"Character with unitId {unitId} not found!");
        return null;
    }

    private void UpdatePlayTimeUI()
    {
        playTime += Time.deltaTime;
        // 00.00 ����
        playTimeText.text = playTime.ToString("00.00");
    }

    public GameObject GetPrefabByAssetId(int assetId)
    {
        if (assetPrefabDict.TryGetValue(assetId, out var prefab))
        {
            return prefab; // Dictionary���� prefab ��ȯ
        }

        Debug.LogWarning($"Prefab with assetId {assetId} not found!");
        return null;
    }

    private IEnumerator SendLocationNotificationCor()
    {
        while (true)
        {
            SendLocationNotification(); // �ǽð� �̵� ��ǥ ����
            yield return new WaitForSeconds(locationNotificationDelay); // 1�� ���
        }

    }

    // ĳ���� ��ġ ���� ����
    public void SendLocationNotification()
    {
        var locationNotification = new C2SLocationNotification();

        // Ȱ��ȭ�� ������ �ִ°��
        if(activeCharacterDic.Count != 0)
        {
            foreach (var characterPair in activeCharacterDic)
            {
                Character character = characterPair.Value; // �� ����

                if (character.gameObject.layer == LayerMask.NameToLayer("Enemy")) continue;

                Vector3 worldPosition = character.transform.TransformPoint(character.transform.localPosition);

                // DebugOpt.Log("���� : " + worldPosition);
                // DebugOpt.Log("���� : " + character.transform.position);

                var position = new Position();

                if (isDog)
                {
                    position.X = character.transform.position.x;
                    position.Z = character.transform.position.z;
                }
                else
                {
                    position.X = -character.transform.position.x;
                    position.Z = character.transform.position.z;
                }

                //position.X = character.transform.position.x;
                //position.Z = character.transform.position.z;

                var unitPosition = new UnitPosition();
               
                var rotation = new Rotation();

                rotation.Y = character.transform.rotation.eulerAngles.y;

                unitPosition.UnitId = character.unitId;
                unitPosition.Position = position;
                unitPosition.Rotation = rotation;
               
                locationNotification.UnitPositions.Add(unitPosition);
            }
            

            if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
            {
                GamePacket packet = new GamePacket();

                packet.LocationNotification = locationNotification;

                SocketManager.Instance.Send(packet);
            }

           
        }

        
    }

    public void GameOver(bool isWin)
    {
        StartCoroutine(GameOverCor(isWin));
    }

    private IEnumerator GameOverCor(bool isWin)
    {
        GameOverPanel gameOverPanel = UIManager.Instance.ShowPopUI<GameOverPanel>();

        if (isWin)
        {
            gameOverPanel.ShowWin();
        }
        else
        {
            gameOverPanel.ShowLose();
        }

        yield return new WaitForSeconds(gameOverDelay);

        gameOverPanel.HideGameOver();

        // ���� ���� ��Ŷ

        C2SGameEndNotification c2SGameEndNotification = new C2SGameEndNotification();   

        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();

            packet.GameEndNotification = c2SGameEndNotification;

            SocketManager.Instance.Send(packet);

        }

        SocketManager.Instance.Disconnect();
        SocketManager.Instance.Init(APIModels.ip, APIModels.lobbyPort);
        SocketManager.Instance.Connect(() => {
            SceneManager.LoadScene("LobbyScene");
            SoundManager.Instance.PlayBGM(startSceneBgmClip);

            if (SocketManager.Instance.isConnected)
            {
                // ���� ���� �� ��û ����
                var authRequest = new C2SAuthRequest();
                authRequest.Token = GameManager.Instance.jwtToken;

                GamePacket packet = new GamePacket();
                packet.AuthRequest = authRequest;
                SocketManager.Instance.Send(packet);
                Debug.Log("Auth request sent!");
            }

        });
    }

}
