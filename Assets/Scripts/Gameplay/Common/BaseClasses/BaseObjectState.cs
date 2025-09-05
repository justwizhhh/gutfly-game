using UnityEngine;

public class BaseObjectState : MonoBehaviour
{
    // ------------------------------
    //
    // Base template class for the different states a level object could be in
    //
    // ------------------------------

    [HideInInspector] public bool isActive = false;

    protected ObjectStateMachine stateMachine;

    public virtual void Awake()
    {
        stateMachine = GetComponent<ObjectStateMachine>();
    }

    public virtual void StartState()
    {

    }

    public virtual void UpdateState()
    {

    }

    public virtual void FixedUpdateState()
    {

    }

    public virtual void EndState()
    {

    }
}
