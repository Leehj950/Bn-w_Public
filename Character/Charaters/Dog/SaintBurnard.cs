using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaintBurnard : Character
{
    private int buffAmount = 2;
    private int buffDuration = 3;
    private float currentTime;

    List<Character> alliesInRange;
    List<int> unitIds;
    public override void Detect()
    {
        alliesInRange = SkillManager.Instance.GetAlliesInRange(this, this.characterData.AttackDistance);
        if (alliesInRange.Count > 0)
        {
            foreach (Character c in alliesInRange)
            {
                unitIds.Add(c.unitId);
            }
            this.characterStateMachine.ChangeState(this.characterStateMachine.attackState);
        }
        else
        {
            this.characterStateMachine.ChangeState(this.characterStateMachine.moveState);
        }
    }

    public override void UseSkill()
    {
        if (alliesInRange.Count == 0)
        {
            this.characterStateMachine.ChangeState(this.characterStateMachine.moveState);
        }
        SkillManager.Instance.ApplyAttackSpeedBuff(alliesInRange, buffAmount, buffDuration, this);
    }
}
