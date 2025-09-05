using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTail : MonoBehaviour
{
    // ----------------------
    //
    // Class for moving and rendering the player's tail, in conjunction with the LineRenderer component
    //
    // Based on a tutorial by Blackthornprod: https://youtu.be/9hTnlp9_wX8
    //
    // ----------------------

    [Header("Tail Appearance Options")]
    public int TailLength;
    public float TailSegmentDistance;
    public float TailSpinSpeed;
    public float TailFollowSpeed;

    [Space(10)]
    public float TailWiggleSpeed;
    public float TailWiggleMagnitude;
    public float TailWiggleMagnitudeScaler;

    [Space(10)]
    public Transform Target;
    public Transform WiggleTarget;

    // Tail settings
    private Vector3[] tailSegments;
    private Vector3[] tailSegmentsVel;
    private float currentWiggleMagnitude;

    // Object/component references
    [HideInInspector] public LineRenderer lr;
    private PlayerController player;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        ResetTail();
    }

    public void ResetTail()
    {
        lr.positionCount = TailLength;
        tailSegments = new Vector3[TailLength];
        tailSegmentsVel = new Vector3[TailLength];

        for (int i = 1; i < tailSegments.Length; i++)
        {
            tailSegments[i] = player.transform.position;
        }
        lr.SetPositions(tailSegments);
    }

    private void SetTargetRotation()
    {
        Vector3 direction = player.rb.velocity.normalized * -1;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Target.rotation = Quaternion.Slerp(Target.rotation, rotation, TailSpinSpeed * Time.deltaTime);
    }

    private void Update()
    {
        // Make the tail wiggle faster depending on how fast the player is moving
        currentWiggleMagnitude = TailWiggleMagnitude * (player.rb.velocity.sqrMagnitude * TailWiggleMagnitudeScaler);

        //Move the tail along a sine wave for a wiggle effect
        WiggleTarget.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * TailWiggleSpeed) * currentWiggleMagnitude);

        SetTargetRotation();
    }

    private void FixedUpdate()
    {
        tailSegments[0] = Target.transform.position;

        for (int i = 1; i < tailSegments.Length; i++)
        {
            /*
            tailSegments[i] = Vector3.SmoothDamp(
                tailSegments[i],
                tailSegments[i - 1] + Target.right * TailSegmentDistance,
                ref tailSegmentsVel[i],
                TailFollowSpeed);
            */

            Vector3 targetPos = tailSegments[i - 1] + (tailSegments[i] - tailSegments[i - 1] + Target.right).normalized * TailSegmentDistance;
            tailSegments[i] = Vector3.SmoothDamp(tailSegments[i], targetPos, ref tailSegmentsVel[i], TailFollowSpeed);
        }

        lr.SetPositions(tailSegments);
    }
}
