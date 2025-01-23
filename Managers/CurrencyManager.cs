using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public TextMeshProUGUI currencyText; // UI에 표시할 Text
    public int currency = 0; // 현재 재화

    private NumberCounter counter;

    private void Start()
    {
        counter = GetComponent<NumberCounter>();
    }

    public void UpdateCurrencyUI()
    {
        counter.UpdateText(currency, currencyText);
        // 소수점 제거
        // currencyText.text = currency.ToString();
    }
}
