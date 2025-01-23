using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedBuffState : CharacterSkillState
{
    private int buffAmount = 2;
    private int buffDuration = 3;
    private float currentTime;

    List<Character> alliesInRange;
    List<int> unitIds;
    public AttackSpeedBuffState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.character.characterAnimationData.BuffParameterHash);
        // 아군 2명에게 공속 버프 적용
    }

    public override void Update()
    {
        base.Update();
 
    }

    public override void Exit() 
    { 
        base.Exit();
        StopAnimation(stateMachine.character.characterAnimationData.BuffParameterHash);
    }
}

