public class MultiShotState : CharacterSkillState
{
    public MultiShotState(CharacterStateMachine stateMachine, int enemyLayer) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 적군 3명에게 멀티샷 공격
        // SkillManager.MultiShotAttack(stateMachine.character.transform.position, stateMachine.character.enemyLayer, 15f);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        // 공격 후 필요한 후속 처리
    }
}
