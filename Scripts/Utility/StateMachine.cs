using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IState
{
    public void Enter();
    public void Exit();
    public void HandleInput();
    public void Update();
    public void PhysicsUpdate();
}
public class StateMachine : MonoBehaviour
{
    protected IState currentState;
    public void ChangeState(IState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState?.Enter();
    }
    public void HandleInput()
    {
        currentState?.HandleInput();
    }

    public void Update()
    {
        currentState?.Update();
    }
    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
}
