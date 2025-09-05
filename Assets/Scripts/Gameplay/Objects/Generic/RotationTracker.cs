using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTracker : MonoBehaviour
{
    // ----------------------
    //
    // This object keeps track of the level's rotation state and triggers functions for gates and other similar objects
    //
    // ----------------------

    public bool IsLevelRotationTracked;
    public LevelController.RotationDirections TargetRotation;

    [Space(10)]
    public bool IsLevelAngleTracked;
    public float TargetAngle;
    public float TargetAngleRange;

    [Space(10)]
    public MonoBehaviour TriggerObject;

    private void Update()
    {
        // If both conditions are checked, only enable the trigger object if both conditions are met
        if (IsLevelRotationTracked && IsLevelAngleTracked)
        {
            if (CheckForLevelRotation() && CheckForLevelAngle())
            {
                TriggerEnable();
            }
            else
            {
                TriggerDisable();
            }
        }
        // Otherwise, check conditions individually
        else
        {
            if (IsLevelRotationTracked)
            {
                if (CheckForLevelRotation())
                {
                    TriggerEnable();
                }
                else
                {
                    TriggerDisable();
                }
            }
            else if (IsLevelAngleTracked)
            {
                if (CheckForLevelAngle())
                {
                    TriggerEnable();
                }
                else
                {
                    TriggerDisable();
                }
            }
        }
    }

    private bool CheckForLevelRotation()
    {
        return LevelController.Instance.CurrentRotationDirection == TargetRotation;
    }

    private bool CheckForLevelAngle()
    {
        float angle = LevelController.Instance.UpAngle;
        return Mathf.Abs(Mathf.DeltaAngle(angle, TargetAngle)) <= TargetAngleRange;
    }

    private void TriggerEnable()
    {
        switch (TriggerObject)
        {
            case Gate triggerGate:
                triggerGate.Open();
                break;
        }
    }

    private void TriggerDisable()
    {
        switch (TriggerObject)
        {
            case Gate triggerGate:
                triggerGate.Close();
                break;
        }
    }
}
