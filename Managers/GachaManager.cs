using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public List<UnitData> AllUnits; // ��� ���� �����͸� ����
    public List<GachaButtonData> GachaButtons; // �̱� ��ư ����

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
                // ��޿� �´� ���� ���� ����
                List<UnitData> possibleUnits = AllUnits.FindAll(u => u.tier == rate.tier);
                return possibleUnits[Random.Range(0, possibleUnits.Count)];
            }
        }

        return null; // �̱⿡ ������ ���
    }
}
