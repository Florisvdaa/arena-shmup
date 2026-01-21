using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StateManager is a MonoBehaviour that owns and runs a set of BaseState instances.
/// - It stores states in a dictionary keyed by the enum EState.
/// - It handles state lifecycle: EnterState, UpdateState, ExitState and transitions.
/// - It forwards Unity trigger events to the active state.
/// </summary>
public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{    
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();   // All available states for this manager. Fill this dictionary in Awake/Start or via a factory.
    protected BaseState<EState> currentState;                                                               // The currently active state instance.

    protected bool isTransitioningState = false;

    private void Start()
    {
        currentState.EnterState();
    }

    /// <summary>
    /// Unity Update callback. Each frame:
    /// 1. Ask the current state what the next state should be (GetNextState).
    /// 2. If the next state equals the current state's key, call UpdateState on the current state.
    /// 3. If the next state differs, initiate a transition to that state.
    /// The isTransitioningState flag prevents overlapping transitions.
    /// </summary>
    private void Update()
    {
        EState nextStateKey = currentState.GetNextState();
        if (!isTransitioningState && nextStateKey.Equals(currentState.StateKey))
            currentState.UpdateState();
        else if (!isTransitioningState)
            TransitionToState(currentState.StateKey);
    }

    /// <summary>
    /// Performs a safe transition from the current state to the state identified by stateKey.
    /// Steps:
    /// 1. Set transition guard.
    /// 2. Call ExitState on the current state.
    /// 3. Replace currentState with the new state instance from the dictionary.
    /// 4. Call EnterState on the new state.
    /// 5. Clear transition guard.
    /// </summary>
    /// <param name="stateKey">Enum key of the state to transition to.</param>
    public void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        currentState.ExitState();
        currentState = States[stateKey];
        currentState.EnterState();
        isTransitioningState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }
    private void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
    }
    private void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }
}
