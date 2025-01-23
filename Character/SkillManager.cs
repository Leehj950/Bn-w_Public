using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SkillManager : Singleton<SkillManager>
{
    public HashSet<int> buffedUnits = new HashSet<int>(); // 버프 받은 유닛
    LayerMask allyLayer;
    public GameObject HealEffectPrefab;
    public GameObject AtkSpeedBuffEffectPrefab;

    public bool allyPresence;

    private void Start()
    {
        allyLayer = LayerMask.GetMask("Ally");
        allyPresence = false;
    }
    #region Heal

    public Character FindLowestHpAlly(Character Healer, Vector3 position, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, allyLayer);
        Character targetAlly = null;
        float lowestHp = float.MaxValue;

        foreach (var collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (character != null 
                && character.gameObject.layer != character.enemyLayer 
                && character != Healer 
                && character.characterData.Hp > 0
                && character.characterData.Hp < character.characterData.MaxHp) // 본인 제외 아군인지 확인
            {
                if (character.characterData.Hp < lowestHp)
                {
                    lowestHp = character.characterData.Hp;
                    targetAlly = character;
                }
            }
        }

        //if (targetAlly == null || !IsValidTarget(Healer, targetAlly, range))
        //{
        //    // 조건에 맞는 아군이 없으면 원래 목표로 이동
        //    Healer.StartMoveGoal();
        //    allyPresence = false;
        //    return null;
        //}
        //else
        //{
        //    Healer.StopMove();
        //    allyPresence = true;
        //}
    
        //MoveTowardsTarget(Healer, targetAlly);

        return targetAlly;
    }

    public void HealTargetUnit(int unitId, int unitHp)
    {
        Debug.Log("힐 타겟 유닛");

        Character unit = BattleManager.Instance.GetCharacterById(unitId);

        unit.characterData.Hp = unitHp;

        unit.ShowHealEffect();
    }
    public void HealTarget(int unitId, Character target)
    {
        if (SocketManager.Instance.isConnected)
        {
            var healUnitRequest = new C2SHealUnitRequest();
            healUnitRequest.UnitId = unitId;
            healUnitRequest.TargetId = target.unitId;
            healUnitRequest.HealAmount = target.characterData.MaxHp / 2;

            GamePacket gamePacket = new GamePacket();
            gamePacket.HealUnitRequest = healUnitRequest;
            SocketManager.Instance.Send(gamePacket);
        }
    }

    #endregion

    #region SpeedBuff

    public void ApplyAttackSpeedBuffFromServer(List<int> unitIds, int buffAmount, float buffDuration)
    {
        foreach (int unitId in unitIds)
        {
            // 버프 중복 확인
            if (buffedUnits.Contains(unitId))
                continue;

            // 해당 유닛 찾기
            Character targetCharacter = BattleManager.Instance.GetCharacterById(unitId);
            if (targetCharacter != null)
            {
                ApplySpeedMultiplier(targetCharacter, buffAmount);
                buffedUnits.Add(unitId);

                // 버프 해제를 위한 타이머 실행
                CoroutineRunner.Instance.StartCoroutine(RemoveAttackSpeedBuff(targetCharacter, buffAmount, buffDuration));
            }
        }
    }

    private void ApplySpeedMultiplier(Character character, int multiplier)
    {
        character.characterData.Cd *= multiplier; // 공속 2배 적용      
    }

    // 버프 지속 시간이 끝난 후 공속을 원래대로 돌려주는 코루틴
    private IEnumerator RemoveAttackSpeedBuff(Character character, int multiplier, float duration)
    {
        yield return new WaitForSeconds(duration);

        character.characterData.Cd /= multiplier; // 공속 복구
        buffedUnits.Remove(character.unitId); // 목록에서 제거
    }

    // 범위 내 아군 유닛을 찾는 함수
    public List<Character> GetAlliesInRange(Character buffer, float range)
    {
        List<Character> allies = new List<Character>();

        Collider[] colliders = Physics.OverlapSphere(buffer.transform.position, range);

        foreach (var collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (character != null && character.gameObject.layer != character.enemyLayer)
            {
                allies.Add(character);
            }
        }

        if(allies.Count == 0)
        {
            // 조건에 맞는 아군이 없으면 원래 목표로 이동
            buffer.StartMoveGoal();
            return null;
        }
        else
        {
            buffer.StopMove();
        }

        MoveTowardsTarget(buffer, allies.FirstOrDefault());

        return allies;
    }

    public void ApplyAttackSpeedBuff(List<Character> alliesInRange, int buffAmount, int buffDuration, Character buffer)
    {
        List<Character> selectedAllies = new List<Character>();

        while (selectedAllies.Count < 2 && alliesInRange.Count > 0)
        {
            Character ally = alliesInRange[Random.Range(0, alliesInRange.Count)];
            alliesInRange.Remove(ally);

            if (!SkillManager.Instance.buffedUnits.Contains(ally.unitId)) // 중복 체크
            {
                selectedAllies.Add(ally);
                SkillManager.Instance.buffedUnits.Add(ally.unitId); // 버프 받은 유닛으로 추가
            }
        }

        // 패킷 생성
        List<int> targetIds = new List<int>();
        foreach (var ally in selectedAllies)
        {
            targetIds.Add(ally.unitId);
        }

        // 패킷 전송
        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            var buffRequest = packet.BuffUnitRequest;
            buffRequest.UnitId = buffer.unitId;
            buffRequest.TargetIds.Add(targetIds);
            buffRequest.BuffAmount = buffAmount;
            buffRequest.BuffDuration = buffDuration;

            SocketManager.Instance.Send(packet);
        }
    }

    #endregion SpeedBuff

    #region MultiShot

    // 멀티샷 - 공격검증
    public void RequestMultishotCharacter(Character character)
    {
        int maxTargetCount = 3;

        // 캐릭터 공격
        if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
        {
            int targetCount = 0;

            var attackUnitRequest = new C2SAttackUnitRequest();
            attackUnitRequest.AttackingUnitId = character.unitId;

            // maxTargetCount만큼 반복 후 종료
            foreach (var target in character.targetEnemyList)
            {
                if (targetCount >= maxTargetCount)
                    break;

                attackUnitRequest.TargetUnitIds.Add(target.unitId);
            }

            GamePacket packet = new GamePacket();
            packet.AttackUnitRequest = attackUnitRequest;
            SocketManager.Instance.Send(packet);
        }
    }

    public void RequestMultishotBuilding(Character character)
    {
        // 건물 공격
        if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
        {
            var attackBuildingRequest = new C2SAttackBaseRequest();
            attackBuildingRequest.UnitId = character.unitId;

            GamePacket packet = new GamePacket();
            packet.AttackBaseRequest = attackBuildingRequest;
            SocketManager.Instance.Send(packet);
        }
    }

    public void MultiShotAttack(Character character)
    {
        if(character.targetEnemyList.Count > 0)
        {
            MultiShotAttackEnemy(character, character.targetEnemyList);
        }else if (character.targetBuilding != null)
        {
            MultiShotAttackBuilding(character, character.targetBuilding);
        }
    }

    // 멀티샷 - 유닛 투사체 발사
    public void MultiShotAttackEnemy(Character character, HashSet<Character> targetEnemys)
    {
        // 캐릭터 공격
        foreach (var target in targetEnemys)
        {
            if(!target.gameObject.activeSelf) continue;
            // 발사체 생성 : 오브젝트풀로 가져옴
            Projectile projectile = ProjectilePool.Instance.GetFromPool();

            // 초기화 
            var x = Random.Range(-0.1f, 0.1f);
            Vector3 newPosition = character.ProjectilePosition.position; // 기존 위치
            newPosition.x += x; // x 값에 랜덤 값 추가
            projectile.transform.position = newPosition; // 위치 설정

            projectile.transform.rotation = Quaternion.identity; // 기본 회전값

            // 발사체 이동 
            projectile.SetTarget(target, character);
        }

        // Move로 변경
        character.ClearTarget();
        character.characterStateMachine.ChangeState(character.characterStateMachine.moveState);
    }

    // 멀티샷 - 성체 투사체 발사
    public void MultiShotAttackBuilding(Character character, Castle targetBuilding)
    {
        // 발사체 생성 : 오브젝트풀로 가져옴
        Projectile projectile = ProjectilePool.Instance.GetFromPool();

        projectile.transform.position = character.ProjectilePosition.position; // 기존 위치
        projectile.transform.rotation = Quaternion.identity; // 기본 회전값

        // 발사체 이동 
        projectile.SetTarget(targetBuilding, character);

        // Move로 변경
        character.ClearTarget();
        character.characterStateMachine.ChangeState(character.characterStateMachine.moveState);
    }


    #endregion

    // 범위 내 적군 유닛을 찾는 함수
    public List<Character> GetEnemiesInRange(Vector3 position, int enemyLayer, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range);
        List<Character> enemies = new List<Character>();

        foreach (var collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (character != null && character.gameObject.layer != enemyLayer)
            {
                enemies.Add(character);
            }
        }

        return enemies;
    }



    #region BaseAttack

    public void BaseAttack(Character character)
    {

        // EnemyLayer null 체크
        if (character.enemyLayer.IsUnityNull())
        {
            DebugOpt.Log("EnemyLayer가 비어있습니다.");
            return;
        }


        // 캐릭터를 공격
        if (character.targetEnemy != null)
        {
            AttackEnemyCharacter(character);
           
        }
        // 건물을 공격
        else if (character.targetBuilding != null)
        {
            AttackEnemyBuilding(character);
       
        }

        character.ClearTarget();
        character.characterStateMachine.ChangeState(character.characterStateMachine.moveState);

     
    }

    private void AttackEnemyCharacter(Character character)
    {
        if (character.IsEnemy() || !character.targetEnemy.gameObject.activeSelf) return;

        Debug.Log("유닛 공격!");

        if (character.targetEnemy.characterData.Hp > 0)
        {
            if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
            {
                var attackUnitRequest = new C2SDamageUnitRequest();
                attackUnitRequest.AttackingUnitId = character.unitId;
                attackUnitRequest.TargetUnitId = character.targetEnemy.unitId;

                GamePacket packet = new GamePacket();
                packet.DamageUnitRequest = attackUnitRequest;
                SocketManager.Instance.Send(packet);
            }
        }
    }


    private void AttackEnemyBuilding(Character character)
    {
        if (character.IsEnemy()) return;

        Debug.Log("건물 공격!");
        if (character.targetBuilding.CurrentHP > 0)
        {
            if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
            {
                var attackBuildingRequest = new C2SDamageBaseRequest();
                attackBuildingRequest.UnitId = character.unitId;

                GamePacket packet = new GamePacket();
                packet.DamageBaseRequest = attackBuildingRequest;
                SocketManager.Instance.Send(packet);
            }
        }
    }

    public void BaseDetectEnemy(Character character)
    {
        character.targetEnemyList.Clear();
        character.targetEnemy = null;
        character.targetBuilding = null;

        // 재이동
        if (character.agent.speed == 0 || character.agent.isStopped == true)
        {
            character.StartMoveGoal();
        }

        if (BattleManager.activeCharacterDic.Count > 0)
        {
            foreach (var item in BattleManager.activeCharacterDic)
            {


                if (item.Value.IsEnemy() && character.isLeftDir == item.Value.isLeftDir)
                {
                    if (Vector3.Distance(item.Value.transform.position, character.transform.position) <= character.characterData.DetectDistance)
                    {
                        character.targetEnemyList.Add(item.Value);
                    }
                }
            }
        }

        if (BattleManager.activeBuildingDic.Count > 0)
        {
            foreach (var item in BattleManager.activeBuildingDic)
            {
                if (item.Value.IsEnemy())
                {
                    if (Vector3.Distance(item.Value.transform.position, character.transform.position) <= character.characterData.DetectDistance)
                    {
                        character.targetBuildingList.Add(item.Value);
                    }
                }
            }
        }
        if (character.targetEnemyList.Count > 0 || character.targetBuildingList.Count > 0)
        {
            var nearestEnemy = character.targetEnemyList
                .OrderBy(enemy => Vector3.Distance(character.transform.position, enemy.transform.position))
            .FirstOrDefault();

            var nearestBuilding = character.targetBuildingList
                .OrderBy(building => Vector3.Distance(character.transform.position, building.transform.position))
                .FirstOrDefault();

            if (nearestEnemy != null && nearestBuilding != null)
            {
                if (Vector3.Distance(character.transform.position, nearestEnemy.transform.position) <
                    Vector3.Distance(character.transform.position, nearestBuilding.transform.position))
                {
                    character.targetEnemy = nearestEnemy.GetComponent<Character>();
                    character.targetBuilding = null;
                }
                else
                {
                    character.targetBuilding = nearestBuilding.GetComponent<Castle>();
                    character.targetEnemy = null;
                }
            }
            else if (nearestEnemy != null)
            {
                character.targetEnemy = nearestEnemy.GetComponent<Character>();
                character.targetBuilding = null;
            }
            else if (nearestBuilding != null)
            {
                character.targetBuilding = nearestBuilding.GetComponent<Castle>();
                character.targetEnemy = null;
            }
        }
        else
        {
            return;
        }

       

        var targetPosition = character.targetEnemy != null
            ? character.targetEnemy.transform.position
            : character.targetBuilding.transform.position;

        //if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.AttackDistance)
        //{
        //    character.StopMove();

        //    // 공격 준비 상태로감 + 쿨타임 체크
        //    if (character.CD >= (character.characterData.Cd))
        //    {
        //        character.StartMoveGoal();
        //        // 공격준비 들어가면 AI 돌림
        //        character.characterStateMachine.ChangeState(character.characterStateMachine.prepareAttackState);
        //    }
        //}
        //else
        //{
        //    if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.DetectDistance
        //   && Vector3.Distance(character.transform.position, targetPosition) >= 1f) // 유닛 어느정도 띄워주기
        //    {

        //        character.transform.position = Vector3.MoveTowards(
        //           character.transform.position,
        //           targetPosition,
        //           character.characterData.Speed * Time.deltaTime * 0.5f
        //       );
        //    }


        //    character.transform.LookAt(targetPosition);
        //}

     

        if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.AttackDistance)
        {
            character.StopMove();

            if (character.CD >= character.characterData.Cd)
            {
                character.characterStateMachine.ChangeState(character.characterStateMachine.prepareAttackState);
            }
        }
        else if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.DetectDistance)
        {
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                targetPosition,
                character.characterData.Speed * Time.deltaTime
            );

            character.transform.LookAt(targetPosition);
        }


    }

    public void CheckAttackCharacterRequest(Character character)
    {
        if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
        {
            if (!character.targetEnemy.gameObject.activeSelf)
            {
                return;
            }
            var attackUnitRequest = new C2SAttackUnitRequest();
            attackUnitRequest.AttackingUnitId = character.unitId;
            attackUnitRequest.TargetUnitIds.Add(character.targetEnemy.unitId);

            GamePacket packet = new GamePacket();
            packet.AttackUnitRequest = attackUnitRequest;
            SocketManager.Instance.Send(packet);

        }
    }

    public void CheckAttackBuildingRequest(Character character)
    {
        if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
        {
            var attackBuildingRequest = new C2SAttackBaseRequest();
            attackBuildingRequest.UnitId = character.unitId;

            GamePacket packet = new GamePacket();
            packet.AttackBaseRequest = attackBuildingRequest;
            SocketManager.Instance.Send(packet);
        }
    }

    #endregion

    private void MoveTowardsTarget(Character character, Character target)
    {
        character.transform.position = Vector3.MoveTowards(
            character.transform.position,
            target.transform.position,
            character.characterData.Speed * Time.deltaTime
        );
        character.transform.LookAt(target.transform.position);
    }

    private bool IsValidTarget(Character unit, Character target, float range)
    {
        if (target == null || target.characterData.Hp <= 0)
            return false;

        float distance = Vector3.Distance(unit.transform.position, target.transform.position);
        return distance <= range; // 범위 내에 있는지 확인
    }

    public void IdleDetect(Character character)
    {
            character.targetEnemyList.Clear();
            character.targetEnemy = null;
            character.targetBuilding = null;

            if (BattleManager.activeCharacterDic.Count > 0)
            {
                foreach (var item in BattleManager.activeCharacterDic)
                {


                    if (item.Value.IsEnemy() && character.isLeftDir == item.Value.isLeftDir)
                    {
                        if (Vector3.Distance(item.Value.transform.position, character.transform.position) <= character.characterData.DetectDistance)
                        {
                            character.targetEnemyList.Add(item.Value);
                        }
                    }
                }
            }

            if (BattleManager.activeBuildingDic.Count > 0)
            {
                foreach (var item in BattleManager.activeBuildingDic)
                {
                    if (item.Value.IsEnemy())
                    {
                        if (Vector3.Distance(item.Value.transform.position, character.transform.position) <= character.characterData.DetectDistance)
                        {
                            character.targetBuildingList.Add(item.Value);
                        }
                    }
                }
            }
            if (character.targetEnemyList.Count > 0 || character.targetBuildingList.Count > 0)
            {
                var nearestEnemy = character.targetEnemyList
                    .OrderBy(enemy => Vector3.Distance(character.transform.position, enemy.transform.position))
                .FirstOrDefault();

                var nearestBuilding = character.targetBuildingList
                    .OrderBy(building => Vector3.Distance(character.transform.position, building.transform.position))
                    .FirstOrDefault();

                if (nearestEnemy != null && nearestBuilding != null)
                {
                    if (Vector3.Distance(character.transform.position, nearestEnemy.transform.position) <
                        Vector3.Distance(character.transform.position, nearestBuilding.transform.position))
                    {
                        character.targetEnemy = nearestEnemy.GetComponent<Character>();
                        character.targetBuilding = null;
                    }
                    else
                    {
                        character.targetBuilding = nearestBuilding.GetComponent<Castle>();
                        character.targetEnemy = null;
                    }
                }
                else if (nearestEnemy != null)
                {
                    character.targetEnemy = nearestEnemy.GetComponent<Character>();
                    character.targetBuilding = null;
                }
                else if (nearestBuilding != null)
                {
                    character.targetBuilding = nearestBuilding.GetComponent<Castle>();
                    character.targetEnemy = null;
                }
            }
            else
            {
                return;
            }



            var targetPosition = character.targetEnemy != null
                ? character.targetEnemy.transform.position
                : character.targetBuilding.transform.position;

            //if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.AttackDistance)
            //{
            //    character.StopMove();

            //    // 공격 준비 상태로감 + 쿨타임 체크
            //    if (character.CD >= (character.characterData.Cd))
            //    {
            //        character.StartMoveGoal();
            //        // 공격준비 들어가면 AI 돌림
            //        character.characterStateMachine.ChangeState(character.characterStateMachine.prepareAttackState);
            //    }
            //}
            //else
            //{
            //    if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.DetectDistance
            //   && Vector3.Distance(character.transform.position, targetPosition) >= 1f) // 유닛 어느정도 띄워주기
            //    {

            //        character.transform.position = Vector3.MoveTowards(
            //           character.transform.position,
            //           targetPosition,
            //           character.characterData.Speed * Time.deltaTime * 0.5f
            //       );
            //    }


            //    character.transform.LookAt(targetPosition);
            //}



            if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.AttackDistance)
            {
                character.StopMove();

                if (character.CD >= character.characterData.Cd)
                {
                    character.characterStateMachine.ChangeState(character.characterStateMachine.prepareAttackState);
                }
            }
            else if (Vector3.Distance(character.transform.position, targetPosition) <= character.characterData.DetectDistance)
            {
                character.transform.position = Vector3.MoveTowards(
                    character.transform.position,
                    targetPosition,
                    character.characterData.Speed * Time.deltaTime
                );

                character.transform.LookAt(targetPosition);
            }


        }
    }

