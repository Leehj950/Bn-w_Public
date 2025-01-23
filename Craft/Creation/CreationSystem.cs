using CatDogEnums;
using UnityEngine;

public class CreationSystem : MonoBehaviour
{
    public Land myland;
    public Land Enmeyland;
    public CharacterObjectPool pool;

    public void Start()
    {
        pool = GetComponent<CharacterObjectPool>();
    }

    public void BuyBuilding(int assetId)
    {
        if (myland.AreAllBuildingsValid())
        {
            Debug.Log("�ǹ��� ��� ���������ϴ�.");
            return;
        }

        Building building = BattleManager.Instance.GetPrefabByAssetId(assetId).GetComponent<Building>();

     
            CurrencyManager.Instance.currency -= building.cost;

            // ���õ� Ÿ�Կ� �°� �ǹ��� ����
            myland.BuildBuilding(assetId);

            // �ǹ� ���� ������Ʈ (���õ� Ÿ�Կ� ����)
            //if (land.isDogSelected)
            //{
            //    land.UpdateDogBuildingStatus(dogType);
            //}
            //else
            //{
            //    land.UpdateCatBuildingStatus(catType);
            //}

    }

    public void BuyEnemyBuilding(int assetId)
    {

        Building building = BattleManager.Instance.GetPrefabByAssetId(assetId).GetComponent<Building>();

        Enmeyland.BuildBuilding(assetId);

    }

    public void BuyEnemyCharacter(int assetId, int unitId, bool isLeft)
    {
        Character character = BattleManager.Instance.GetPrefabByAssetId(assetId).GetComponent<Character>();

        var poolData = pool.GetPoolData((AssetIdType)assetId);

        if (poolData.pool != null && !string.IsNullOrEmpty(poolData.unitName))
        {
            pool.ActivateEnemyUnitFromPool((AssetIdType)assetId, isLeft, unitId);
        }
    }

    public void BuyCharacter(int assetId, int unitId, bool isLeft)
    {
        Character character = BattleManager.Instance.GetPrefabByAssetId(assetId).GetComponent<Character>();

        CurrencyManager.Instance.currency -= character.characterData.Cost;

        // AssetId�� �ش��ϴ� Ǯ ������ ��������
        var poolData = pool.GetPoolData((AssetIdType)assetId);

        if (poolData.pool != null && !string.IsNullOrEmpty(poolData.unitName))
        {
            pool.ActivateUnitFromPool((AssetIdType)assetId, isLeft, unitId);
        }
    }
}
