using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HPBarManager : Singleton<HPBarManager>
{
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private Transform canvasTransform; // HP �ٰ� ǥ�õ� Canvas

    private Dictionary<int, GameObject> hpBarPool = new Dictionary<int, GameObject>(); // UnitId�� HP �� ��Ī
    private Queue<GameObject> hpBarPoolQueue = new Queue<GameObject>();

    void Start()
    {
        int initialPoolSize = 20; // �ʱ� Ǯ ũ�� (���ӿ� ���� ����)
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject hpBar = Instantiate(hpBarPrefab, canvasTransform);
            hpBar.SetActive(false);
            hpBarPoolQueue.Enqueue(hpBar);
        }
    }

    void Update()
    {
        foreach (var kvp in hpBarPool)
        {
            int unitId = kvp.Key;
            GameObject hpBar = kvp.Value;

            Character character = BattleManager.Instance.GetCharacterById(unitId);
            if (character != null && hpBar != null)
            {
                UpdateHPBarPosition(character.transform, hpBar);
            }
        }
    }

    public void AddHPBar(int unitId, Transform targetTransform, float maxHP)
    {
        if (!hpBarPool.ContainsKey(unitId))
        {
            GameObject hpBar = GetHPBarFromPool();
            hpBarPool[unitId] = hpBar;
            HPBar temphpBar = hpBarPool[unitId].GetComponent<HPBar>();

            if (targetTransform.gameObject.layer.Equals(LayerMask.NameToLayer("Ally")))
            {
                temphpBar.CurrentHpSlider.fillRect.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/SliderGreen");
            }
            else
            {
                //red
                temphpBar.CurrentHpSlider.fillRect.GetComponent<Image>().sprite = ResourceManager.Instance.Load<Sprite>("UI/SliderRed");
            }
            var rect = temphpBar.gameObject.GetComponent<RectTransform>();
            rect.localScale = rect.localScale * -1;
        }

        // �ʱ�ȭ �۾�
        UpdateHPBar(unitId, targetTransform, 1.0f); // �ִ� ü������ �ʱ�ȭ
    }

    public void UpdateHPBar(int unitId, Transform unitTransform, float hpPercentage)
    {
        if (hpBarPool.TryGetValue(unitId, out var hpBarGameobject))
        {
            UpdateHPBarPosition(unitTransform, hpBarGameobject);
        }

        // HP ���� ü�� ������Ʈ
        var hpbar = hpBarGameobject.GetComponent<HPBar>();

        // �ǰ�üũ�� ����ڵ�
        if (Mathf.Approximately(hpbar.CurrentHpSlider.value, hpPercentage)) return;

        if (hpbar.CurrentHpSlider != null)
        {
            StartCoroutine(HpPercentageUpdate(hpbar, hpPercentage));
        }
    }

    public void RemoveHPBar(int unitId)
    {
        if (hpBarPool.TryGetValue(unitId, out var hpBar))
        {
            hpBar.SetActive(false);
            ReturnHPBarToPool(hpBar);
            hpBarPool.Remove(unitId);
        }
    }

    private void UpdateHPBarPosition(Transform targetTransform, GameObject hpBar)
    {
        // ���� ���� ������ �߰�
        Vector3 worldPosition = targetTransform.position + new Vector3(0, 1.5f, 0);
        hpBar.transform.position = worldPosition;

    }

    private GameObject GetHPBarFromPool()
    {
        if (hpBarPoolQueue.Count > 0)
        {
            GameObject hpBar = hpBarPoolQueue.Dequeue();
            hpBar.SetActive(true);
            hpBar.transform.SetParent(canvasTransform, false); // World Space Canvas�� ����
            return hpBar;
        }
        else
        {
            GameObject hpBar = Instantiate(hpBarPrefab, canvasTransform);
            hpBar.SetActive(false);
            return hpBar;
        }
    }

    private void ReturnHPBarToPool(GameObject hpBar)
    {
        hpBar.SetActive(false);
        hpBarPoolQueue.Enqueue(hpBar);
    }

    IEnumerator HpPercentageUpdate(HPBar hpBar, float hpPercentage)
    {
        while (Mathf.Abs(hpBar.BackGroundSlider.value - hpPercentage) > 0.01f) // �� ���� ���̰� ���� ������ �� ����
        {
            hpBar.CurrentHpSlider.value = hpPercentage;
            
            hpBar.BackGroundSlider.value = Mathf.Lerp(hpBar.BackGroundSlider.value, hpBar.CurrentHpSlider.value, Time.deltaTime);
            
            yield return null; // �� ������ ���
        }
        // ��ǥ ���� �ٻ�ġ�� �������� �� ��Ȯ�� ����
        hpBar.CurrentHpSlider.value = hpPercentage;
    }
}

