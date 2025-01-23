using UnityEngine;

public class CastleManager : Singleton<CastleManager>
{
    [SerializeField] private GameObject DogCastle;
    [SerializeField] private GameObject CatCastle;
    [SerializeField] private Transform CastleSpawnPoint;
    [SerializeField] private Transform EnemyCastlePoint;
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private Canvas worldCanvas;

    public int CurrentHP;
    public int MaxHP;

    public Castle myCastle;
    public Castle enemyCastle;

    public void CastleSpawn(bool isDog)
    {
        if (BattleManager.Instance.isDog)
        {
            myCastle = Instantiate(DogCastle, CastleSpawnPoint).GetComponent<Castle>();
            enemyCastle = Instantiate(CatCastle, EnemyCastlePoint).GetComponent<Castle>();
        }
        else
        {
            myCastle = Instantiate(CatCastle, CastleSpawnPoint).GetComponent<Castle>();
            enemyCastle = Instantiate(DogCastle, EnemyCastlePoint).GetComponent<Castle>();
        }

        // ��ä �ʱ�ȭ
        enemyCastle.gameObject.layer = LayerMask.NameToLayer("Enemy");


        BattleManager.activeBuildingDic.Add(enemyCastle.CastleID, enemyCastle);
        myCastle.transform.eulerAngles = new Vector3(myCastle.transform.eulerAngles.x, 90, myCastle.transform.eulerAngles.z);
        myCastle.Initialize(0, 1000, worldCanvas);  // �� ��ä (ID: 0, �ִ� HP: 100)
        enemyCastle.Initialize(1, 1000, worldCanvas); // ��� ��ä (ID: 1, �ִ� HP: 100)
    }

    public void UpdateCastleHP(int castleID, int newHP)
    {
        if (castleID == 0)
        {
            myCastle.UpdateHP(newHP);
        }
        else if (castleID == 1)
        {
            enemyCastle.UpdateHP(newHP);
        }
        else
        {
            Debug.LogWarning($"Invalid CastleID: {castleID}");
        }
    }

    public Castle GetCastleByID(int castleID)
    {
        return castleID == 0 ? myCastle : (castleID == 1 ? enemyCastle : null);
    }
}
