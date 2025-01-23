using CatDogEnums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PrepareAttackState : CharacterBaseState
{

    public PrepareAttackState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 공격 준비 패킷 전송

        if (stateMachine.character.targetEnemy != null)
        {
            stateMachine.character.CheckAttackUnit();
        }
        else if (stateMachine.character.targetBuilding != null)
        {
            stateMachine.character.CheckAttackBuilding();
        }
    }

    public override void Update()
    {
        base.Update();

    }

    public override void Exit()
    {
        base.Exit();
      
    }

    
}