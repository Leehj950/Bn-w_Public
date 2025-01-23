using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CharacterBaseState : IState
{
    protected CharacterStateMachine stateMachine;

    public CharacterBaseState(CharacterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }


    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public void HandleInput()
    {
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {
    }

    protected void StartAnimation(int animatorHash)
    {
        stateMachine.character.animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.character.animator.SetBool(animatorHash, false);
    }

    protected void StartTriggerAnimation(int animatorHash)
    {
        stateMachine.character.animator.SetTrigger(animatorHash);
    }

    protected void StopTriggerAnimation(int animatorHash)
    {
        stateMachine.character.animator.ResetTrigger(animatorHash);
    }

   

    private void ClearTarget()
    {
        stateMachine.character.targetEnemyList.Clear();
        stateMachine.character.targetEnemy = null;
        stateMachine.character.targetBuilding = null;
    }

    protected void StartMoveGoal(GameObject goal)
    {
        stateMachine.character.agent.SetDestination(goal.transform.position);
        stateMachine.character.agent.speed = stateMachine.character.characterData.Speed;
        stateMachine.character.agent.isStopped = false;
    }

    protected void StopMove()
    {
        stateMachine.character.agent.speed = 0;
        stateMachine.character.agent.isStopped = true;
    }

}
