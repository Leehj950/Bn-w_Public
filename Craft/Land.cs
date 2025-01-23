using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Land : MonoBehaviour
{
    public Transform[] buildTile;

    public Building[] buildings;

    public float buildDuration = 2f; // 건물 생성 시간
    public bool isDogSelected;

    private void Start()
    {
        buildings = new Building[buildTile.Length];
    }

    public void SetPlayerSelection(bool dogSelected)
    {
        isDogSelected = dogSelected;
    }

    public void BuildBuilding(int assetId)
    {
        for (int i = 0; i < buildTile.Length; i++)
        {
            if (buildings[i] != null)
            {
                continue;
            }

            var entry = BattleManager.Instance.GetPrefabByAssetId(assetId);

            if (entry == null)
            {
                Debug.LogWarning("해당 타입에 맞는 건물이 없습니다.");
                return;
            }

            // 빌딩 생성
            GameObject buildingObj = Instantiate(entry, buildTile[i].position, Quaternion.identity);
            buildings[i] = buildingObj.GetComponent<Building>();
            StartCoroutine(BuildAnimation(buildings[i].transform));

            break;
        }
    }


    private IEnumerator BuildAnimation(Transform building)
    {
        Vector3 targetScale = building.localScale; // 원래 크기 저장
        building.localScale = Vector3.zero;       // 시작 크기를 0으로 설정

        float elapsedTime = 0f;

        while (elapsedTime < buildDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / buildDuration;
            building.localScale = Vector3.Lerp(Vector3.zero, targetScale, t); // 크기 점점 키움
            yield return null;
        }

        building.localScale = targetScale; // 최종 크기로 설정
    }

    // 빌딩이 모두 지어졌는지 확인
    public bool AreAllBuildingsValid()
    {
        return buildings != null && buildings.All(b => b != null);
    }

    //// 특정 캐릭터 타입에 맞는 건물이 지어졌는지 확인
    //public bool IsBuildingBuiltForDog(DogType dogType)
    //{
    //    return dogBuildingStatus.ContainsKey(dogType) && dogBuildingStatus[dogType];
    //}

    //// 건물을 지을 때 상태를 업데이트
    //public void BuildBuildingUpdate(CatType catType)
    //{
    //    if (!catBuildingStatus.ContainsKey(catType))
    //    {
    //        catBuildingStatus[catType] = true;
    //    }
    //}

    //public bool IsBuildingBuiltForCat(CatType catType)
    //{
    //    return catBuildingStatus.ContainsKey(catType) && catBuildingStatus[catType];
    //}

    //// 상태 업데이트 메서드 (DogType)
    //public void UpdateDogBuildingStatus(DogType dogType)
    //{
    //    if (dogType == null) return;

    //    if (!dogBuildingStatus.ContainsKey(dogType))
    //    {
    //        dogBuildingStatus[dogType] = true;
    //    }
    //}

    //// 상태 업데이트 메서드 (CatType)
    //public void UpdateCatBuildingStatus(CatType catType)
    //{
    //    if (catType == null) return;

    //    if (!catBuildingStatus.ContainsKey(catType))
    //    {
    //        catBuildingStatus[catType] = true;
    //    }
    //}
}
