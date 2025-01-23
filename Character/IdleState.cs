using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : CharacterBaseState
{
    public IdleState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
       
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.character.characterAnimationData.IdleParameterHash);
        //StopMove();
        if (!stateMachine.character.IsEnemy()) stateMachine.character.SendAnimationNotification(AnimationType.Idle);
    }

    public override void Update()
    {
        base.Update();
        if (!stateMachine.character.IsEnemy())
            stateMachine.character.IdleDetect();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.characterAnimationData.IdleParameterHash);
    }

}
