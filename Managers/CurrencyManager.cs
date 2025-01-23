using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public TextMeshProUGUI currencyText; // UI�� ǥ���� Text
    public int currency = 0; // ���� ��ȭ

    private NumberCounter counter;

    private void Start()
    {
        counter = GetComponent<NumberCounter>();
    }

    public void UpdateCurrencyUI()
    {
        counter.UpdateText(currency, currencyText);
        // �Ҽ��� ����
        // currencyText.text = currency.ToString();
    }
}
