using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberCounter : MonoBehaviour
{
    public int CountFPS = 30;
    public float Duration = 1f;
    public string NumberFormat = "N0";
    private int value = 0;

    private Coroutine CountingCoroutine;

    public void UpdateText(int newValue, TextMeshProUGUI Text)
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }

        CountingCoroutine = StartCoroutine(CountText(newValue, Text));
    }

    private IEnumerator CountText(int newValue, TextMeshProUGUI Text)
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = value; // Start with the current value
        int stepAmount;

        // Calculate the step amount (positive or negative)
        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration));
        }

        // Increment or decrement until the target value is reached
        if (previousValue < newValue)
        {
            while (previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }

                Text.SetText(previousValue.ToString(NumberFormat));
                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount;
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }

                Text.SetText(previousValue.ToString(NumberFormat));
                yield return Wait;
            }
        }

        // Ensure previousValue and value match the newValue
        previousValue = newValue;
        value = newValue;

        // Final update to the text (optional, to ensure it reflects the exact value)
        Text.SetText(newValue.ToString(NumberFormat));
    }
}
