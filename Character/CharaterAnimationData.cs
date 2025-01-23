using UnityEngine;

public class CharacterAnimationData
{
    private string moveParameter = "Run";
    private string attackParameter = "Attack";
    private string dieParameter = "Die";
    private string idleParameter = "Idle";
    private string SkillParameter = "@Skill";
    private string healParameter = "Heal";
    private string buffParameter = "Buff";
    private string multiShotParameter = "MultiShot";

    public int MoveParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }
    public int SkillParameterHash {  get; private set; }
    public int HealParameterHash { get; private set; }
    public int BuffParameterHash { get; private set; }
    public int MultiShotParameterHash { get; private set; }


    public void Init()
    {
        MoveParameterHash = Animator.StringToHash(moveParameter);
        AttackParameterHash = Animator.StringToHash(attackParameter);
        DieParameterHash = Animator.StringToHash(dieParameter);
        IdleParameterHash = Animator.StringToHash(idleParameter);
        SkillParameterHash = Animator.StringToHash(SkillParameter);
        HealParameterHash = Animator.StringToHash(healParameter);
        BuffParameterHash = Animator.StringToHash(buffParameter);
        MultiShotParameterHash = Animator.StringToHash(multiShotParameter);
    }
}