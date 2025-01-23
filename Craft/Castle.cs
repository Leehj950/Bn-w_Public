using GooglePlayGames.BasicApi.Events;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public int CastleID; // 성채 ID (0: 내 성채, 1: 상대 성채)
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
                BattleManager.activeBuildingDic.Add(CastleID, this); // 활성화 시 추가
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
        // HPBar를 World Canvas에 생성
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
            hpBarRectTransform.position = transform.position + Vector3.up * 2f; // 성채 위로 약간 띄움
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
        // 파괴 연출 추가 (애니메이션, 효과 등)
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

