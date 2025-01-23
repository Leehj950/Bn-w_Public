using System.Collections;
using System.Collections.Generic;

using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private List<GameObject> characterList;
    private int dogListLimit;
    private int catListLimit;
    public int DogListLimit { get { return dogListLimit; } }
    public int CatListLimit { get { return catListLimit; } }

    public List<GameObject> CharacterList { get { return characterList; } private set { } }
    void Start()
    {
        characterList = new List<GameObject>();

        GameObject[] doglist = Resources.LoadAll<GameObject>("Assets/Prefabs/Character/Dog");

        dogListLimit = doglist.Length;

        for (int i = 0; i < doglist.Length; i++)
        {
            characterList.Add(doglist[i]);
        }

        GameObject[] catlist = Resources.LoadAll<GameObject>("Assets/Prefabs/Character/Cat");

        for (int i = 0; i < catlist.Length; i++)
        {
            characterList.Add(catlist[i]);
        }

        catListLimit = characterList.Count;
    }

    void Update()
    {

    }
    public List<UnitData> LoadUnitData()
    {
        string fileName = "unitJson.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            // JSON ���� �б�
            string jsonData = File.ReadAllText(filePath);
            // JSON �����͸� ��ü�� ��ȯ
            UnitDatas unitDatas = JsonUtility.FromJson<UnitDatas>(jsonData);
            return unitDatas.data;
        }

        return null;

    }
}
