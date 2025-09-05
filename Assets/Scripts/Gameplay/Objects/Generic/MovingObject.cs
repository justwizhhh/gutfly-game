using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // ----------------------
    //
    // This object follows and moves along a predetermined path in the scene
    //
    // ----------------------

    [Space(10)]
    public float MoveSpeed;
    public float MovePauseTime;
    public float MoveDelayTime;

    [Space(10)]
    public bool IsSmoothMoving;
    public float SmoothMoveDecelTime;

    private bool isMoving;
    private List<Transform> movePoints = new List<Transform>();
    private int currentMovePoint = 0;
    private int nextMovePoint = 1;
    private float moveStartTime;

    private Rigidbody2D rb;

    private void Awake()
    {
        movePoints = GetComponentsInChildren<Transform>().ToList();
        movePoints.RemoveAt(0);
        transform.DetachChildren();

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        LevelController.EnableObjects.AddListener(Activate);
    }

    private void OnDisable()
    {
        LevelController.EnableObjects.RemoveListener(Activate);
    }

    private void Start()
    {
        rb.position = movePoints[0].position;
    }

    private void Activate()
    {
        StartCoroutine(DelayMove());
    }

    private IEnumerator DelayMove()
    { 
        yield return new WaitForSeconds(MoveDelayTime);

        moveStartTime = Time.time;
        isMoving = true;
    }

    private IEnumerator SetNewMovePoints()
    {
        isMoving = false;
        rb.position = (Vector2)movePoints[nextMovePoint].position;

        yield return new WaitForSeconds(MovePauseTime);

        currentMovePoint = nextMovePoint;
        nextMovePoint++;
        if (nextMovePoint >= movePoints.Count)
        {
            nextMovePoint = 0;
        }

        moveStartTime = Time.time;
        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (LevelController.Instance.IsLevelActive && isMoving)
        {
            if (IsSmoothMoving)
            {
                float t = (Time.time - moveStartTime) / SmoothMoveDecelTime;

                rb.MovePosition(
                    Vector2.Lerp(movePoints[currentMovePoint].position, movePoints[nextMovePoint].position,
                        Mathf.SmoothStep(0, 1, t)));

                if (t >= 1)
                {
                    StartCoroutine(SetNewMovePoints());
                }
            }
            else
            {
                Vector2 movement =
                    (movePoints[nextMovePoint].position - movePoints[currentMovePoint].position).normalized
                    * MoveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + movement);

                if (Vector2.Distance(rb.position, (Vector2)movePoints[nextMovePoint].position) <= movement.magnitude)
                {
                    StartCoroutine(SetNewMovePoints());
                }
            }
        }
    }
}
