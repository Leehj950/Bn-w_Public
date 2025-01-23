using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DieState : CharacterBaseState
{
    public DieState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
       
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.character.characterAnimationData.DieParameterHash);
        stateMachine.character.gameObject.SetActive(false);
        HPBarManager.Instance.RemoveHPBar(stateMachine.character.unitId);
        //if (!stateMachine.character.IsEnemy()) stateMachine.character.SendAnimationNotification(AnimationType.Die);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.characterAnimationData.DieParameterHash);
    }


}
