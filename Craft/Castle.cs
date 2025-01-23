using GooglePlayGames.BasicApi.Events;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public int CastleID; // ��ä ID (0: �� ��ä, 1: ��� ��ä)
    public int CurrentHP;
    public int MaxHP;

    [SerializeField] private GameObject hpBarPrefab;
    private Slider hpSlider;
    private RectTransform hpBarRectTransform;

    private Canvas worldCanvas;

    private void OnEnable()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (!BattleManager.activeBuildingDic.ContainsKey(CastleID))
            {
                BattleManager.activeBuildingDic.Add(CastleID, this); // Ȱ��ȭ �� �߰�
            }

            // characterStateMachine.ChangeState(characterStateMachine.attackState);

        }
    }

    public void Initialize(int castleID, int maxHP, Canvas canvas)
    {
        CastleID = castleID;
        MaxHP = maxHP;
        CurrentHP = maxHP;

        worldCanvas = canvas;

        CreateHPBar();
        UpdateUI();
    }

    private void CreateHPBar()
    {
        // HPBar�� World Canvas�� ����
        GameObject hpBar = Instantiate(hpBarPrefab, worldCanvas.transform);
        hpSlider = hpBar.GetComponentsInChildren<Slider>()[1];
        hpBarRectTransform = hpBar.GetComponent<RectTransform>();

        //Vector3 currentRotation = hpBar.transform.localEulerAngles;
        //currentRotation.x = 180;
        //hpBar.transform.localEulerAngles = currentRotation;

        UpdateHPBarPosition();
    }

    private void UpdateHPBarPosition()
    {
        if (hpBarRectTransform != null)
        {
            hpBarRectTransform.position = transform.position + Vector3.up * 2f; // ��ä ���� �ణ ���
        }
    }

    public void UpdateHP(int newHP)
    {
        CurrentHP = Mathf.Clamp(newHP, 0, MaxHP);

        UpdateUI();

        if (CurrentHP <= 0)
        {
            OnCastleDestroyed();
        }
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = (float)CurrentHP / MaxHP;
        }
    }

    private void OnCastleDestroyed()
    {
        Debug.Log($"Castle {CastleID} destroyed!");
        // �ı� ���� �߰� (�ִϸ��̼�, ȿ�� ��)
    }

    public bool IsEnemy()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            return true;
        }
        return false;
        // return (enemyLayer.value & (1 << gameObject.layer)) != 0;
    }
}

