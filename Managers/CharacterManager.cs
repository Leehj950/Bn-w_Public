using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> characterPrefabs; // 모든 몬스터 프리팹 리스트

    private Dictionary<int, GameObject> assetIdLookup;

    private void Awake()
    {
        assetIdLookup = new Dictionary<int, GameObject>();

        // 모든 프리팹의 AssetID를 읽어와 Dictionary에 저장
        foreach (var prefab in characterPrefabs)
        {
            var character = prefab.GetComponent<Character>();
            if (character == null || character.AssetID <= 0)
            {
                Debug.LogWarning($"Prefab {prefab.name}에 Monster 스크립트가 없거나 AssetID가 올바르지 않습니다.");
                continue;
            }

            if (!assetIdLookup.ContainsKey(character.AssetID))
            {
                assetIdLookup.Add(character.AssetID, prefab);
            }
            else
            {
                Debug.LogWarning($"중복된 AssetID가 발견되었습니다: {character.AssetID}");
            }
        }
    }

    public GameObject GetMonsterPrefabById(int assetId)
    {
        // AssetID로 프리팹 검색
        assetIdLookup.TryGetValue(assetId, out var prefab);
        return prefab;
    }
}
