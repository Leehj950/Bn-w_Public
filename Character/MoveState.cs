using CatDogEnums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MoveState : CharacterBaseState
{
    public MoveState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.character.ResetCoolDown();

        // 캐릭터 재이동
        if (!stateMachine.character.IsEnemy()) 
        {
            stateMachine.character.agent.speed = stateMachine.character.characterData.Speed;
            stateMachine.character.agent.isStopped = false;
            stateMachine.character.agent.SetDestination(stateMachine.character.goal.transform.position);
            Debug.Log("이동");
        }

        //CoroutineRunner.Instance.StartCoroutine(WaitAttackAnimation());

        // 애니메이션
        StartAnimation(stateMachine.character.characterAnimationData.MoveParameterHash);
        if (!stateMachine.character.IsEnemy()) stateMachine.character.SendAnimationNotification(AnimationType.Run);
    }

    public override void Update()
    {
        base.Update();

        // 쿨타임 적용
        // stateMachine.character.CD += Time.time;

        if (!stateMachine.character.IsEnemy())
            stateMachine.character.Detect();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.character.characterAnimationData.MoveParameterHash);

    }

    private IEnumerator WaitAttackAnimation()
    {
        AnimatorStateInfo currentStateInfo = stateMachine.character.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorTransitionInfo transitionInfo = stateMachine.character.animator.GetAnimatorTransitionInfo(0);

        while ((!currentStateInfo.IsName("Attack") && !transitionInfo.IsUserName("Attack"))
               || currentStateInfo.normalizedTime < 1f)
        {
            yield return null;
            currentStateInfo = stateMachine.character.animator.GetCurrentAnimatorStateInfo(0);
            transitionInfo = stateMachine.character.animator.GetAnimatorTransitionInfo(0);
        }

        StartAnimation(stateMachine.character.characterAnimationData.MoveParameterHash);
        if (!stateMachine.character.IsEnemy()) stateMachine.character.SendAnimationNotification(AnimationType.Run);

    }
}