public class MultiShotState : CharacterSkillState
{
    public MultiShotState(CharacterStateMachine stateMachine, int enemyLayer) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // ���� 3���� ��Ƽ�� ����
        // SkillManager.MultiShotAttack(stateMachine.character.transform.position, stateMachine.character.enemyLayer, 15f);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        // ���� �� �ʿ��� �ļ� ó��
    }
}
