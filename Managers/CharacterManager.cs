using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> characterPrefabs; // ��� ���� ������ ����Ʈ

    private Dictionary<int, GameObject> assetIdLookup;

    private void Awake()
    {
        assetIdLookup = new Dictionary<int, GameObject>();

        // ��� �������� AssetID�� �о�� Dictionary�� ����
        foreach (var prefab in characterPrefabs)
        {
            var character = prefab.GetComponent<Character>();
            if (character == null || character.AssetID <= 0)
            {
                Debug.LogWarning($"Prefab {prefab.name}�� Monster ��ũ��Ʈ�� ���ų� AssetID�� �ùٸ��� �ʽ��ϴ�.");
                continue;
            }

            if (!assetIdLookup.ContainsKey(character.AssetID))
            {
                assetIdLookup.Add(character.AssetID, prefab);
            }
            else
            {
                Debug.LogWarning($"�ߺ��� AssetID�� �߰ߵǾ����ϴ�: {character.AssetID}");
            }
        }
    }

    public GameObject GetMonsterPrefabById(int assetId)
    {
        // AssetID�� ������ �˻�
        assetIdLookup.TryGetValue(assetId, out var prefab);
        return prefab;
    }
}
