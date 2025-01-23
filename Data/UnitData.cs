using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Protobuf.Compiler;


[Serializable]
public class UnitDatas
{
    public string name;
    public string version;
    public List<UnitData> data;
}

[Serializable]
public class UnitData
{
    public int id;
    public string species;
    public string DisplayName;
    public string Description;
    public int cost;
    public int maxHp;
    public int atk;
    public float atkRange;
    public float searchingRange;
    public int cd;
    public int skillCd;
    public int spd;
    public string type;
    public int tier;
    public float scale;
    public string @class;
}

[Serializable]
public class GachaTable
{
    public int tier;
    public float Probability; // 확률
}
[Serializable]
public class GachaButtonData
{
    public int Cost;
    public List<GachaTable> GachaRates; // 각 등급 확률
}

public class PlayerCard
{
    public UnitData Unit; // 보관 유닛 데이터
    public int Count; // 보유한 동일 유닛 카드 수
}
