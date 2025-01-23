using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobermanAttackState : AttackState
{
    public DobermanAttackState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.character.characterAnimationData.AttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.characterAnimationData.AttackParameterHash);
    }
}
