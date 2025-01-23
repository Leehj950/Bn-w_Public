using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [Serializable]
    public class CharacterPrefabEntry
    {
        public GameObject prefab;         // ������
    }

    public List<CharacterPrefabEntry> characterPrefabList = new List<CharacterPrefabEntry>();
}
