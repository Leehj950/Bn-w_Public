using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RussianBlue : Character
{
    private Character target;

    private double lastSkillTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 5000;
    private double skillTime;
    private double currentTime;

    public override void Detect()
    {
        Debug.Log("lastSkilTime �ʱ�ȭ:" + lastSkillTime);
        currentTime = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        skillTime = currentTime - lastSkillTime;

        target = SkillManager.Instance.FindLowestHpAlly(this, this.transform.position, this.characterData.AttackDistance);

        if (target != null)
        {
            Debug.Log("Ÿ���� " + target);
            this.StopMove();

            if (skillTime >= characterData.SkillCd * 1000)
            {
                this.characterStateMachine.ChangeState(this.characterStateMachine.attackState);
            }

        }
        else
        {
            Debug.Log("Ÿ�ϰ��� ����");
            this.StartMoveGoal();
            this.characterStateMachine.ChangeState(this.characterStateMachine.moveState);
        }
    }

    public override void UseSkill()
    {
        currentTime = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        skillTime = currentTime - lastSkillTime;
        Debug.Log("RussianBlue UseSkill");
        if (skillTime < this.characterData.SkillCd)
            return;


        if (target == null)
        {
            Debug.Log("vecter" + Vector3.Distance(this.transform.position, target.transform.position) + "��Ÿ�" + this.characterData.AttackDistance);
            Debug.Log("RussianBlue UseSkill target is null" + target);
            this.characterStateMachine.ChangeState(this.characterStateMachine.moveState);
            return;
        }

        // ?? ????
        SkillManager.Instance.HealTarget(this.unitId, target);

        // ????? ????
        lastSkillTime = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        target = null;

        // ?? ?? ??? ????
        this.characterStateMachine.ChangeState(this.characterStateMachine.moveState);
    }
}