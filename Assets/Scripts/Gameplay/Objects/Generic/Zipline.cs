using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Zipline : MonoBehaviour
{
    // ----------------------
    //
    // This object transports the player from one point to another along a spline-based zipline that preserves speed
    //
    // ----------------------

    [Header("Zipline Spline Settings")]
    public float RideSpeed;
    public float RideAccel; // How quickly the zipline goes from 'RideStartSpeed' to regular 'RideSpeed'

    public enum EntryPoints
    {
        Start,
        End,
        Both
    };
    public EntryPoints AllowedEntryPoints;

    private Vector2 currentPlayerPosition;
    private float currentRideSpeed;
    private int currentDirection = 1;

    private Spline spline;
    private bool isPlayerOnZipline;
    private int targetSection = 0;
    private float t;

    // Object/components references
    private PlayerController player;
    private CircleCollider2D startCol;
    private CircleCollider2D endCol;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        startCol = transform.GetChild(0).GetComponent<CircleCollider2D>();
        endCol = transform.GetChild(1).GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        spline = GetComponent<SpriteShapeController>().spline;
        startCol.gameObject.transform.localPosition = spline.GetPosition(0);
        startCol.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359));
        endCol.gameObject.transform.localPosition = spline.GetPosition(spline.GetPointCount() - 1);
        endCol.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359));

        if (AllowedEntryPoints == EntryPoints.Start)
        {
            endCol.gameObject.SetActive(false);
        }
        else if (AllowedEntryPoints == EntryPoints.End)
        {
            startCol.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Checking for a player object to transport
        if (!isPlayerOnZipline)
        {
            switch (AllowedEntryPoints)
            {
                case EntryPoints.Start:
                    if (startCol.IsTouching(player.col))
                    {
                        StartZipline(false);
                        StartCoroutine(ZiplineMove());
                    }
                    break;

                case EntryPoints.End:
                    if (endCol.IsTouching(player.col))
                    {
                        StartZipline(true);
                        StartCoroutine(ZiplineMove());
                    }
                    break;

                case EntryPoints.Both:
                    if (startCol.IsTouching(player.col))
                    {
                        StartZipline(false);
                        StartCoroutine(ZiplineMove());
                    }
                    if (endCol.IsTouching(player.col))
                    {
                        StartZipline(true);
                        StartCoroutine(ZiplineMove());
                    }
                    break;
            }
        }
    }

    private void StartZipline(bool backwards)
    {
        isPlayerOnZipline = true;
        currentPlayerPosition = backwards ? spline.GetPosition(spline.GetPointCount() - 1) : spline.GetPosition(0);
        currentDirection = backwards ? -1 : 1;
        t = 0;
        targetSection = backwards ? spline.GetPointCount() - 1 : 0;
        player.stateMachine.ChangeState(typeof(PlayerZiplineState));
    }

    private IEnumerator ZiplineMove()
    {
        while (true)
        {
            // If there is no more zipline to move across, end the coroutine
            if ((targetSection >= spline.GetPointCount() - 1 && currentDirection > 0) ||
                (targetSection <= 0 && currentDirection < 0))
            {
                // Gets the direction of movement on the last two frames of spline travel
                Vector2 splinePointsDist = NewBezierPoint(-currentDirection, 0) - NewBezierPoint(-currentDirection, currentRideSpeed);

                // Separate the player from the zipline's entry points
                player.rb.position += splinePointsDist.normalized * (player.col.radius + (startCol.radius * 2));

                switch (player.currentPlayerMode)
                {
                    default:
                    case PlayerController.PlayerModes.Float:
                        player.stateMachine.ChangeState(typeof(PlayerFloatMoveState));
                        break;

                    case PlayerController.PlayerModes.Platform:
                        player.stateMachine.ChangeState(typeof(PlayerPlatformMidairMoveState));
                        break;
                }

                player.rb.AddForce(splinePointsDist.normalized * RideSpeed, ForceMode2D.Impulse);
                break;
            }

            // Otherwise, transport the player object
            // Movement speed is made more consistent by measuring distance between the current and next spline points
            float distance = Vector2.Distance(
                spline.GetPosition(targetSection),
                spline.GetPosition(targetSection + currentDirection)
            );

            if (RideAccel != 0)
            {
                currentRideSpeed += (RideAccel / distance) * Time.fixedDeltaTime;
                currentRideSpeed = Mathf.Clamp(currentRideSpeed, 0, (RideSpeed / distance) * Time.fixedDeltaTime);
            }
            else
            {
                currentRideSpeed = (RideSpeed / distance) * Time.fixedDeltaTime;
            }
            t += currentRideSpeed;

            // Bezier curve spline that the zipline handle follows, along with the player
            currentPlayerPosition = transform.position + NewBezierPoint(currentDirection, t);

            player.rb.velocity = player.rb.position - currentPlayerPosition;
            player.rb.MovePosition(currentPlayerPosition);

            // Continuing to next part of the spline
            if (t >= 1)
            {
                t -= 1;
                targetSection = Mathf.Clamp(targetSection + currentDirection, 0, spline.GetPointCount() - 1);
            }
            if (t <= 0)
            {
                t += 1;
                targetSection = Mathf.Clamp(targetSection + currentDirection, 0, spline.GetPointCount() - 1);
            }

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(ZiplineReset());
    }

    private IEnumerator ZiplineReset()
    {
        isPlayerOnZipline = false;
        t = 0;
        StopCoroutine(ZiplineMove());

        yield return new WaitForFixedUpdate();
        StopCoroutine(ZiplineReset());
    }

    private Vector3 NewBezierPoint(int direction, float t)
    {
        return BezierUtility.BezierPoint(
                spline.GetPosition(targetSection) +
                    (direction > 0 ? spline.GetRightTangent(targetSection) : spline.GetLeftTangent(targetSection)),
                spline.GetPosition(targetSection),
                spline.GetPosition(targetSection + direction),
                spline.GetPosition(targetSection + direction) +
                    (direction > 0 ? spline.GetLeftTangent(targetSection + direction) : spline.GetRightTangent(targetSection + direction)),
                t);
    }
}
