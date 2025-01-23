using System.Threading;
using UnityEngine;

public class CharacterStateMachine : StateMachine
{
    public Character character { get; }
    public IdleState idleState { get; }
    public ChasingState chasingState { get; }
    public AttackState attackState { get; }
    public MoveState moveState { get; }
    public DieState dieState { get; }
    public PrepareAttackState prepareAttackState { get; }

    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;

    public Transform MainManTransform { get; set; }

    public CharacterStateMachine(Character character)
    {
        this.character = character;

        idleState = new IdleState(this);
        chasingState = new ChasingState(this);
        attackState = new AttackState(this);
        moveState = new MoveState(this);
        dieState = new DieState(this);
        prepareAttackState = new PrepareAttackState(this);
    }

    public bool ReturnCurrentState(IState state)
    {
        return currentState == state;
    }
}
