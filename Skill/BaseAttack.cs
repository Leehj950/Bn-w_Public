using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Attack()
    {
        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();

            List<int> opponentUnitIds = new List<int>();

            opponentUnitIds.Add(character.targetEnemy.unitId);

            var attackUnitRequest = new C2SAttackUnitRequest();
            attackUnitRequest.AttackingUnitId = character.unitId;
            attackUnitRequest.TargetUnitIds.Add(character.targetEnemy.unitId);

            packet.AttackUnitRequest = attackUnitRequest;
            SocketManager.Instance.Send(packet);
        }
    }
}
