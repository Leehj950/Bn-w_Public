using CatDogEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static CharacterObjectPool;

public class CharacterObjectPool : MonoBehaviour
{
    [Serializable]
    public struct PoolData
    {
        public AssetIdType assetIdType;
        public List<GameObject> pool;
        public GameObject prefab;
        public string unitName;
    }

    public Land land;

    public List<PoolData> pools;
    private Dictionary<AssetIdType, PoolData> assetPoolDictionary;
    private Dictionary<string, GameObject> goalCache = new Dictionary<string, GameObject>();

    public Transform[] spawnPoints; // ���� ��ġ �迭

    private List<int> availableSpawnPoints; // ���� ������ ��ġ �ε��� ����Ʈ

    // private Dictionary<CharacterType, (List<GameObject> pool, string unitName)> characterPools;

    public int initialUnitCount = 10;

    private void Awake()
    {
        // Dictionary �ʱ�ȭ
        assetPoolDictionary = pools
            .Where(p => p.assetIdType != AssetIdType.None)
            .ToDictionary(p => p.assetIdType, p => p);

    }

    private void Start()
    {
        foreach (var item in spawnPoints)
        {
            Debug.Log("Spawn Point : " + item.transform.position);
        }

        InitializeGoals();
        foreach (var poolData in pools)
        {
            //Debug.Log($"PoolData: {poolData.unitName}, Prefab: {poolData.prefab}");
        }
        // ��� Ǯ �ʱ�ȭ
        foreach (var poolData in pools)
        {
            if (poolData.prefab == null)
            {
                Debug.LogError($"Prefab is null for unit: {poolData.unitName}");
                continue;
            }
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("SpawnPoints array is null or empty.");
                return;
            }
            poolData.pool.Clear(); // ���� Ǯ �ʱ�ȭ
            foreach (Transform spawnPoint in spawnPoints)
            {
                SpawnAllCharacters(poolData.prefab, poolData.pool, spawnPoint, initialUnitCount);
            }
        }

        // ���� ������ ��ġ �ε��� ����Ʈ �ʱ�ȭ
        availableSpawnPoints = Enumerable.Range(0, spawnPoints.Length).ToList();
    }

    private void InitializeGoals()
    {
        goalCache["EnemyGoal"] = Resources.Load<GameObject>("Goals/EnemyGoal");
        goalCache["Goal"] = Resources.Load<GameObject>("Goals/Goal");
    }

    private void SpawnAllCharacters(GameObject prefab, List<GameObject> pool, Transform spawnPoint,int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject unit = Instantiate(prefab, spawnPoint.position, Quaternion.identity, transform);
            unit.SetActive(false);
            pool.Add(unit);
        }
    }

    public void ActivateUnitFromPool(AssetIdType assetIdType, bool isLeft, int unitId)
    {
        if (assetPoolDictionary.TryGetValue(assetIdType, out var poolData))
        {
            Transform selectedSpawnPoint = isLeft ? GetLeftSpawnPoint() : GetRightSpawnPoint();
            ActivateUnit(poolData, selectedSpawnPoint, unitId, isLeft, false);
        }
        else
        {
            Debug.LogError($"AssetIdType {assetIdType}�� ���� Ǯ�� �����ϴ�.");
        }
    }

    public void ActivateEnemyUnitFromPool(AssetIdType assetIdType, bool isLeft, int unitId)
    {
        if (assetPoolDictionary.TryGetValue(assetIdType, out var poolData))
        {
            Transform enemySpawnPoint = isLeft ? GetEnemyRightSpawnPoint() : GetEnemyLeftSpawnPoint(); // �� ���� ��ġ ��������
            ActivateUnit(poolData, enemySpawnPoint, unitId, isLeft, true); // �� ���� Ȱ��ȭ
        }
        else
        {
            Debug.LogError($"AssetIdType {assetIdType}�� ���� Ǯ�� �����ϴ�.");
        }
    }

    private void ActivateUnit(PoolData poolData, Transform spawnPoint, int unitId, bool isLeft, bool isEnemy = false)
    {
        Transform myTransform = spawnPoint;

        Vector3 worldPosition = myTransform.parent.TransformPoint(myTransform.localPosition);

        DebugOpt.Log("���� : " + spawnPoint.position);
        DebugOpt.Log("���� : " + worldPosition);

        if (spawnPoint != null)
        {
            GameObject unit = poolData.pool.Find(u => !u.activeSelf);

            if (unit != null)
            {

                Character character = unit.GetComponent<Character>();

                character.isLeftDir = isLeft;

                unit.transform.rotation = Quaternion.identity;

                if (isEnemy)
                {
                    int layer = LayerMask.NameToLayer("Enemy");
                    unit.layer = layer;
                    character.enemyLayer = LayerMask.GetMask("Ally");
                    unit.transform.rotation = Quaternion.Euler(unit.transform.rotation.eulerAngles.x, unit.transform.rotation.eulerAngles.y - 90, unit.transform.rotation.eulerAngles.z);
                }
                else
                {
                    int layer = LayerMask.NameToLayer("Ally");
                    unit.layer = layer;
                    character.enemyLayer = LayerMask.GetMask("Enemy");
                    unit.transform.rotation = Quaternion.Euler(unit.transform.rotation.eulerAngles.x, unit.transform.rotation.eulerAngles.y + 90, unit.transform.rotation.eulerAngles.z);
                }

                unit.transform.position = spawnPoint.position;
                unit.GetComponent<Character>().unitId = unitId;
                character.characterData.Hp = character.characterData.MaxHp; // 체력초기화 
                unit.SetActive(true);

                if(!isEnemy) character.characterStateMachine.ChangeState(character.characterStateMachine.moveState);

                // HPBar Ȱ��ȭ �� �ʱ�ȭ
                float hpPercentage = (float)character.characterData.Hp / character.characterData.MaxHp;
                Transform characterTransform = character.transform;
                HPBarManager.Instance.AddHPBar(unitId, characterTransform, hpPercentage);
                HPBarManager.Instance.UpdateHPBar(unitId, characterTransform, hpPercentage);


                string goalKey = isEnemy ? "EnemyGoal" : "Goal";
                if (goalCache.TryGetValue(goalKey, out GameObject goalPrefab) && goalPrefab != null)
                {
                    // Character ������Ʈ�� Goal ����
                    if (character != null)
                    {
                        character.goal = goalPrefab; // ĳ�̵� ��ǥ ���� �Ҵ�
                    }
                }
            }
            else
            {
                Debug.Log($"{poolData.unitName}�� Ȱ��ȭ�� ������ �����ϴ�.");
            }
        }
    }

    private Transform GetLeftSpawnPoint()
    {
        if (spawnPoints.Length > 0)
        {
            return spawnPoints[0]; // ������ ��Ÿ���� �ε����� ��� (�ʿ信 ���� ����)
        }
        return null;
    }

    private Transform GetRightSpawnPoint()
    {
        if (spawnPoints.Length > 1)
        {
            return spawnPoints[1]; // �������� ��Ÿ���� �ε����� ��� (�ʿ信 ���� ����)
        }
        return null;
    }
    private Transform GetEnemyLeftSpawnPoint()
    {
        if (spawnPoints.Length > 2)
        {
            return spawnPoints[2]; // �������� ��Ÿ���� �ε����� ��� (�ʿ信 ���� ����)
        }
        return null;
    }
    
    private Transform GetEnemyRightSpawnPoint()
    {
        if (spawnPoints.Length > 3)
        {
            return spawnPoints[3]; // �������� ��Ÿ���� �ε����� ��� (�ʿ信 ���� ����)
        }
        return null;
    }

    public void DeactivateAllUnits()
    {
        foreach (var poolData in pools)
        {
            DeactivateUnitsInPool(poolData.pool);
        }
    }

    private void DeactivateUnitsInPool(List<GameObject> pool)
    {
        foreach (GameObject character in pool)
        {
            if (character.activeSelf)
            {
                character.SetActive(false);
            }

            Character characterComponent = character.GetComponent<Character>();
            if (characterComponent != null)
            {
                HPBarManager.Instance.RemoveHPBar(characterComponent.unitId);
            }
        }
    }

    // Ư�� Ÿ�Կ� ���� Ǯ ��������
    public PoolData GetPoolData(AssetIdType assetIdType)
    {
        if (assetPoolDictionary.TryGetValue(assetIdType, out var poolData))
        {
            return poolData;
        }

        Debug.LogError($"AssetIdType {assetIdType}�� ���� Ǯ�� �����ϴ�");

        return default;
    }
}