using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int assetId;
    public int unitId;

    [SerializeField] private float maxHp = 100f;
    public float currentHp;
    private float stoppingDistance = 0.5f;
    private bool isCapturing = false;

    public int AssetID => assetId;

    public Vector3 dir;
    public NavMeshAgent agent;
    public GameObject goal;
    public bool isElite;
    private HPBar hpBar;
    private bool hasCaptured = false;

    public Animator animator;
    public Rigidbody rb;
    public CharacterStat characterStat;
    public CharacterAnimationData characterAnimationData;
    public CharacterStateMachine characterStateMachine;
    public CharacterData characterData;
    public SkillData skillData;

    public LayerMask enemyLayer;
    public Character targetEnemy;
    public Character targetAlly;

    public Castle targetBuilding;
    public HashSet<Character> targetAllyList = new HashSet<Character>();
    public HashSet<Character> targetEnemyList = new HashSet<Character>();
    public HashSet<Castle> targetBuildingList = new HashSet<Castle>();

    public SmoothMovement smoothMovement;

    public Transform ProjectilePosition;

    private float cd = float.MaxValue;

    public float CD
    {
        get { return cd; }
        set { cd = value; }
    }
    // 선택한 방향으로만 유닛 상호작용가능
    public bool isLeftDir;

    [SerializeField] public AudioClip attackSfx;

    protected virtual void Awake()
    {
        InitializeComponents();
        if (assetId <= 0)
        {
            Debug.LogWarning($"{gameObject.name}�� AssetID�� �ùٸ��� �������� �ʾҽ��ϴ�! (AssetID: {assetId})");
        }
    }

    void Start()
    {
        InitData();

        currentHp = maxHp;

        SetAnimationSpeed();

        
    }

    private void OnEnable()
    {
        if (!BattleManager.activeCharacterDic.ContainsKey(unitId))
        {
            BattleManager.activeCharacterDic.Add(unitId, this); // Ȱ��ȭ �� �߰�

            if (!IsEnemy())
            {               
                agent.SetDestination(goal.transform.position);               
            }
            else
            {
                agent.ResetPath();
            }

            // characterStateMachine.ChangeState(characterStateMachine.moveState);

        }

        //쿨타임 초기화
        cd = float.MaxValue;
    }

    private void Update()
    {
        characterStateMachine.Update();
        if(isCapturing && !hasCaptured && agent.remainingDistance <= stoppingDistance)
        {
            agent.isStopped = true;
            isCapturing = false;
            hasCaptured = true;
            ChangeStateByAnimationType(AnimationType.Idle);
        }

        if (!isCapturing && agent.remainingDistance > stoppingDistance)
        {
            hasCaptured = false; // 조건이 다시 활성화될 수 있도록 플래그 초기화
        }

        CD += Time.deltaTime;
    }


    public virtual void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        characterStat = GetComponent<CharacterStat>();
        agent = GetComponent<NavMeshAgent>();
        characterStateMachine = new CharacterStateMachine(this);
        characterAnimationData = new CharacterAnimationData();
        characterAnimationData.Init();
        characterData = GetComponent<CharacterData>();
        skillData = GetComponent<SkillData>();
        smoothMovement = GetComponent<SmoothMovement>();
    }

    public void InitializeStats()
    {
        characterStat = Instantiate(characterStat);
    }

    private void OnDisable()
    {
        if (BattleManager.activeCharacterDic.ContainsKey(unitId))
        {
            BattleManager.activeCharacterDic.Remove(unitId); // ��Ȱ��ȭ �� ����
        }
    }

    public void SendAnimationNotification(AnimationType type)
    {
        if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
        {
            var unitAnimationNotification = new C2SUnitAnimationNotification();

            unitAnimationNotification.UnitId = unitId;
            unitAnimationNotification.AnimationId = (int)type;

            GamePacket packet = new GamePacket();
            packet.UnitAnimationNotification = unitAnimationNotification;
            SocketManager.Instance.Send(packet);
        }

    }

    public bool IsEnemy()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            return true;
        }
        return false;
        // return (enemyLayer.value & (1 << gameObject.layer)) != 0;
    }

    public void ChangeStateByAnimationType(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Idle:
                characterStateMachine.ChangeState(characterStateMachine.idleState);
                break;
            case AnimationType.Run:
                characterStateMachine.ChangeState(characterStateMachine.moveState);
                break;
            case AnimationType.Attack:
                characterStateMachine.ChangeState(characterStateMachine.attackState);
                break;
            case AnimationType.Die:
                characterStateMachine.ChangeState(characterStateMachine.dieState);
                break;
            default:
                break;
        }
    }


    private void SetAnimationSpeed()
    {
        Debug.Log("애니메이션 스피드적용 " + characterData.name + characterData.Speed);
        animator.SetFloat("RunSpeed", characterData.Speed);
    }

    public void SetTarget(Vector3 capturePointMiddle)
    {
        agent.SetDestination(capturePointMiddle);
        isCapturing = true;
        agent.isStopped = false;   
    }

    public void MoveToNextTarget()
    {
        agent.SetDestination(goal.transform.position);
        agent.isStopped = false;
    }

    private void InitData()
    {
        List<UnitData> unitDatas = DataManager.Instance.LoadUnitData();
        foreach (var item in unitDatas)
        {
            if(item.id == assetId)
            {
                characterData.MaxHp = item.maxHp;
                characterData.Speed = item.spd;
                characterData.Atk = item.atk;
                characterData.AttackDistance = item.atkRange;
                characterData.Cd = item.cd / 1000f;
                characterData.DetectDistance = item.searchingRange;
                characterData.Cost = item.cost;
                characterData.Tier = item.tier;
                characterData.SkillCd = item.skillCd / 1000f;
            }
        }
    }

    public void ShowHealEffect()
    {
        Debug.Log("힐 이펙트");
        // 힐 이펙트를 생성
        GameObject healEffect = Instantiate(SkillManager.Instance.HealEffectPrefab, this.transform.position, Quaternion.identity);

        // 이펙트를 일정 시간 후 제거
        Destroy(healEffect, 2f); // 2초 후 이펙트 제거
    }

    public void ShowAtkSpeedBuffEffect()
    {
        GameObject atkBuffSpeedEffect = Instantiate(SkillManager.Instance.AtkSpeedBuffEffectPrefab, this.transform.position, Quaternion.identity);

        Destroy(atkBuffSpeedEffect, 2f); // 2초 후 이펙트 제거
    }

    public virtual void UseSkill()
    {
        SkillManager.Instance.BaseAttack(this);
    }

    public virtual void CheckAttackUnit()
    {
        SkillManager.Instance.CheckAttackCharacterRequest(this);
    }

    public virtual void CheckAttackBuilding()
    {
        SkillManager.Instance.CheckAttackBuildingRequest(this);
    }


    public virtual void EnemyUseSkill()
    {

    }

    public virtual void Detect()
    {
        SkillManager.Instance.BaseDetectEnemy(this);
    }

    public virtual void IdleDetect()
    {
        SkillManager.Instance.IdleDetect(this);
    }

    public void ClearTarget()
    {
        targetEnemyList.Remove(targetEnemy);
        targetEnemy = null;
        targetBuildingList.Remove(targetBuilding);
        targetBuilding = null;
    }

    public void ResetCoolDown()
    {
        CD = 0f;
    }

    public void StartMoveGoal()
    {
        agent.SetDestination(goal.transform.position);
        agent.speed = characterData.Speed;
        agent.isStopped = false;
    }

    public void StopMove()
    {
        agent.speed = 0;
        agent.isStopped = true;
    }
}