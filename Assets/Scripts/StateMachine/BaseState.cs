using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// BaseState is an abstract blueprint for a single state in a state machine.
/// - EState is a generic enum type that identifies each state (e.g., Idle, Move, Attack).
/// - Concrete states derive from this class and implement the lifecycle and trigger handlers.
/// </summary>
public abstract class BaseState<EState> where EState : Enum
{
    /// <summary>
    /// Construct a state with its identifying enum key.
    /// </summary>
    /// <param name="key">Enum value that uniquely identifies this state.</param>
    public BaseState(EState key)
    {
        StateKey = key;
    }

    /// <summary>
    /// The enum key that identifies this state. Read-only for external classes.
    /// </summary>
    public EState StateKey { get; private set; }

    /// <summary>
    /// Called once when the state becomes active.
    /// Use this to initialize state-specific variables, start animations, reset timers, etc.
    /// </summary>
    public abstract void EnterState();
    
    /// <summary>
    /// Called once when the state is about to be replaced by another state.
    /// Use this to stop animations, clear timers, or undo changes made in EnterState.
    /// </summary>
    public abstract void ExitState();
    
    /// <summary>
    /// Called every frame while this state is active (from StateManager.Update).
    /// Put per-frame logic here (movement, checks, timers). Avoid heavy blocking operations.
    /// </summary>
    public abstract void UpdateState();
    
    /// <summary>
    /// Return the enum key of the next state to transition to.
    /// - The StateManager will call this each Update to decide whether to stay or transition.
    /// - If the returned key equals this.StateKey, the manager will keep this state active.
    /// - If a different key is returned, the manager will transition to that state.
    /// </summary>
    /// <returns>Enum value representing the next state.</returns>
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
