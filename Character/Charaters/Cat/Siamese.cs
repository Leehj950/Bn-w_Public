using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siamese : Character
{
    public override void CheckAttackUnit()
    {
        SkillManager.Instance.RequestMultishotCharacter(this);
    }

    public override void CheckAttackBuilding()
    {
        SkillManager.Instance.RequestMultishotBuilding(this);
    }

    public override void UseSkill()
    {
        SkillManager.Instance.MultiShotAttack(this);
    }

    public override void Detect()
    {
        base.Detect();
    }

}
