using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectStateMachine : MonoBehaviour
{
    // ------------------------------
    //
    // Class for sorting through and updating what state a level object (like the player) is currently in
    //
    // ------------------------------

    public BaseObjectState PreviousState;
    public BaseObjectState CurrentState;
    public List<BaseObjectState> States = new List<BaseObjectState>();

    private void Awake()
    {
        States = GetComponents<BaseObjectState>().ToList();
    }

    private void Start()
    {
        PreviousState = States[0];
        CurrentState = States[0];
        CurrentState.isActive = true;
    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdateState();
    }

    private void Update()
    {
        CurrentState.UpdateState();
    }

    public void ChangeState(Type newState)
    {
        if (CurrentState != null)
        {
            CurrentState.EndState();
            CurrentState.isActive = false;
        }
        PreviousState = CurrentState;
        CurrentState = States.Find(x => x.GetType() == newState);
        CurrentState.StartState();
        CurrentState.isActive = true;
    }
}
