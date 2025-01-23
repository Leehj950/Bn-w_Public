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
            Debug.Log("건물이 모두 지어졌습니다.");
            return;
        }

        Building building = BattleManager.Instance.GetPrefabByAssetId(assetId).GetComponent<Building>();

     
            CurrencyManager.Instance.currency -= building.cost;

            // 선택된 타입에 맞게 건물을 생성
            myland.BuildBuilding(assetId);

            // 건물 상태 업데이트 (선택된 타입에 따라)
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

        // AssetId에 해당하는 풀 데이터 가져오기
        var poolData = pool.GetPoolData((AssetIdType)assetId);

        if (poolData.pool != null && !string.IsNullOrEmpty(poolData.unitName))
        {
            pool.ActivateUnitFromPool((AssetIdType)assetId, isLeft, unitId);
        }
    }
}
