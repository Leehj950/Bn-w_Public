using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkillState : CharacterSkillState
{
    float currentTime;
    public HealSkillState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.character.characterAnimationData.HealParameterHash);
    }

    public override void Update()
    {
        base.Update();
        //    Character targetAlly = SkillManager.Instance.FindLowestHpAlly(stateMachine.character.transform.position, 10f);

        //    if (targetAlly != null)
        //    {
        //        currentTime += Time.deltaTime;

        //        if (currentTime > stateMachine.character.characterData.Cd)
        //        {
        //            currentTime = 0;
        //            //HealTarget(targetAlly);
        //        }
        //    }
        //    else
        //    {
        //        stateMachine.character.ChangeStateByAnimationType(CatDogEnums.AnimationType.Run);
        //    }
        //}
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.characterAnimationData.HealParameterHash);
    }

    
}
