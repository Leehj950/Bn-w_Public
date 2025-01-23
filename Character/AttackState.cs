using CatDogEnums;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AttackState : CharacterBaseState
{
    private bool isAnimationFinished = false;
    public AttackState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isAnimationFinished = false;
        if (!stateMachine.character.IsEnemy())
        {
            stateMachine.character.SendAnimationNotification(AnimationType.Attack);

            if (stateMachine.character.targetEnemy != null)
            {
                if (!stateMachine.character.targetEnemy.gameObject.activeSelf) return;
                stateMachine.character.transform.LookAt(stateMachine.character.targetEnemy.transform);
            }
            if (stateMachine.character.targetBuilding != null) stateMachine.character.transform.LookAt(stateMachine.character.targetBuilding.transform);
        }

        // 공격 애니메이션
        StartTriggerAnimation(stateMachine.character.characterAnimationData.AttackParameterHash);
        // 타격 사운드
        if (stateMachine.character.attackSfx != null)
        {
            SoundManager.Instance.PlaySFX(stateMachine.character.attackSfx, 1f);
        }

        // 공격
        stateMachine.character.UseSkill();

        CoroutineRunner.Instance.StartCoroutine(CheckAnimationComplete());
    }

    public override void Update()
    {
        base.Update();
        if (isAnimationFinished)
        {
            stateMachine.ChangeState(stateMachine.moveState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        isAnimationFinished = false;
        // StopAnimation(stateMachine.character.characterAnimationData.AttackParameterHash);
    }

    private IEnumerator CheckAnimationComplete()
    {
        AnimatorStateInfo currentStateInfo = stateMachine.character.animator.GetCurrentAnimatorStateInfo(0);

        // Attack 애니메이션이 완료될 때까지 대기
        while (currentStateInfo.IsName("Attack") && currentStateInfo.normalizedTime < 1f)
        {
            yield return null;
            currentStateInfo = stateMachine.character.animator.GetCurrentAnimatorStateInfo(0);
        }

        isAnimationFinished = true;
    }


}