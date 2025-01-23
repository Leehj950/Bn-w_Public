using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Land : MonoBehaviour
{
    public Transform[] buildTile;

    public Building[] buildings;

    public float buildDuration = 2f; // �ǹ� ���� �ð�
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
                Debug.LogWarning("�ش� Ÿ�Կ� �´� �ǹ��� �����ϴ�.");
                return;
            }

            // ���� ����
            GameObject buildingObj = Instantiate(entry, buildTile[i].position, Quaternion.identity);
            buildings[i] = buildingObj.GetComponent<Building>();
            StartCoroutine(BuildAnimation(buildings[i].transform));

            break;
        }
    }


    private IEnumerator BuildAnimation(Transform building)
    {
        Vector3 targetScale = building.localScale; // ���� ũ�� ����
        building.localScale = Vector3.zero;       // ���� ũ�⸦ 0���� ����

        float elapsedTime = 0f;

        while (elapsedTime < buildDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / buildDuration;
            building.localScale = Vector3.Lerp(Vector3.zero, targetScale, t); // ũ�� ���� Ű��
            yield return null;
        }

        building.localScale = targetScale; // ���� ũ��� ����
    }

    // ������ ��� ���������� Ȯ��
    public bool AreAllBuildingsValid()
    {
        return buildings != null && buildings.All(b => b != null);
    }

    //// Ư�� ĳ���� Ÿ�Կ� �´� �ǹ��� ���������� Ȯ��
    //public bool IsBuildingBuiltForDog(DogType dogType)
    //{
    //    return dogBuildingStatus.ContainsKey(dogType) && dogBuildingStatus[dogType];
    //}

    //// �ǹ��� ���� �� ���¸� ������Ʈ
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

    //// ���� ������Ʈ �޼��� (DogType)
    //public void UpdateDogBuildingStatus(DogType dogType)
    //{
    //    if (dogType == null) return;

    //    if (!dogBuildingStatus.ContainsKey(dogType))
    //    {
    //        dogBuildingStatus[dogType] = true;
    //    }
    //}

    //// ���� ������Ʈ �޼��� (CatType)
    //public void UpdateCatBuildingStatus(CatType catType)
    //{
    //    if (catType == null) return;

    //    if (!catBuildingStatus.ContainsKey(catType))
    //    {
    //        catBuildingStatus[catType] = true;
    //    }
    //}
}
