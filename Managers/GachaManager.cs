using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public List<UnitData> AllUnits; // 모든 유닛 데이터를 저장
    public List<GachaButtonData> GachaButtons; // 뽑기 버튼 설정

    public UnitData RollGacha(int buttonIndex)
    {
        GachaButtonData button = GachaButtons[buttonIndex];
        float randomValue = Random.value * 100;
        float cumulativeProbability = 0;

        foreach (var rate in button.GachaRates)
        {
            cumulativeProbability += rate.Probability;
            if (randomValue <= cumulativeProbability)
            {
                // 등급에 맞는 유닛 랜덤 선택
                List<UnitData> possibleUnits = AllUnits.FindAll(u => u.tier == rate.tier);
                return possibleUnits[Random.Range(0, possibleUnits.Count)];
            }
        }

        return null; // 뽑기에 실패할 경우
    }
}
