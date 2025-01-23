using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Character targetCharacter;
    private Castle targetCastle;
    private Character owner;

    public void SetTarget<T>(T newTarget, Character ownerCharacter)
    {
        if (newTarget is Character character)
        {
            targetCharacter = character;
            targetCastle = null; 
        }
        else if (newTarget is Castle castle)
        {
            targetCastle = castle;
            targetCharacter = null; 
        }

        owner = ownerCharacter;

        // StartCoroutine(ReturnToPoolAfterDelay(3f)); 
    }

    void Update()
    {
        if (targetCharacter != null) 
        {
            // 목표 방향 계산
            Vector3 direction = (targetCharacter.transform.position - transform.position).normalized;

            // 발사체 이동
            transform.position += direction * speed * Time.deltaTime;
        }

        if (targetCastle != null)
        {
            // 목표 방향 계산
            Vector3 direction = (targetCastle.transform.position - transform.position).normalized;

            // 발사체 이동
            transform.position += direction * speed * Time.deltaTime;
        }


    }

    private void OnTriggerEnter(Collider enemy)
    {
        if (enemy.TryGetComponent(out Character character))
        {
            // 타겟이 아니면 넘김
            if (character.unitId == targetCharacter.unitId)
            {
                // 공격패킷 전송
                AttackCharacter(character);
                Debug.Log($"{character.name}에게 도달!");
                // 발사체 오브젝트풀 해제
                StartCoroutine(ReturnToPoolAfterDelay(1f));
            }

        }

        if (enemy.TryGetComponent(out Castle castle))
        {
            // 공격패킷 전송
            AttackBuilding(castle);

            Debug.Log($"{castle.name}에게 도달!");
            // 발사체 오브젝트풀 해제
            //ProjectilePool.Instance.ReturnObject(this);
            StartCoroutine(ReturnToPoolAfterDelay(1f));

        }
    }

    private void AttackCharacter(Character enemyCharacter)
    {
        if (owner.IsEnemy()) return;

        if (enemyCharacter.characterData.Hp > 0)
        {
            if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
            {
                Debug.Log(owner.unitId + "가" + enemyCharacter.unitId + "를 공격");

                var attackUnitRequest = new C2SDamageUnitRequest();
                attackUnitRequest.AttackingUnitId = owner.unitId;
                attackUnitRequest.TargetUnitId = enemyCharacter.unitId;

                GamePacket packet = new GamePacket();
                packet.DamageUnitRequest = attackUnitRequest;
                SocketManager.Instance.Send(packet);
            }
        }
    }

 
    private void AttackBuilding(Castle castle)
    {
        if (owner.IsEnemy()) return;

        DebugOpt.Log("빌딩맞음");

        if (castle.CurrentHP > 0)
        {
            if (SocketManager.Instance.isConnected && !BattleManager.Instance.isGameOver)
            {
                var attackBuildingRequest = new C2SDamageBaseRequest();
                attackBuildingRequest.UnitId = owner.unitId;

                GamePacket packet = new GamePacket();
                packet.DamageBaseRequest = attackBuildingRequest;
                SocketManager.Instance.Send(packet);
            }
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 오브젝트 풀로 반환
        if (gameObject.activeSelf)
        {
            ProjectilePool.Instance.ReturnObject(this);
        }
    }
}
