using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour 
{
    [System.Serializable]
    public struct Point
    {
        public Vector2 Target;
        public float RestTime;
    }

    public Point[] Points;
    public float KnockBackRecoverTime = 0.5f;
    public int MoveSpeed;

    private Point[] currentPoints;
    private Rigidbody2D rigidBody;
    private TriggerZone groundTrigger;

    private int currentTargetPoint;
    private bool isResting;
    private bool isRecoveringFromKnockBack;

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        groundTrigger = GetComponentInChildren<TriggerZone>();
    }

    public void OnEnable()
    {
        currentPoints = new Point[Points.Length];
        for (int i = 0; i < Points.Length; i++)
        {
            currentPoints[i].Target = (Vector2)transform.position + Points[i].Target;
        }
        currentTargetPoint = 0;
        isResting = false;
        isRecoveringFromKnockBack = false;
    }

    public void FixedUpdate()
    {
        if (isRecoveringFromKnockBack) return;
        if (groundTrigger != null && !groundTrigger.IsActive) return;

        if (isResting)
        {
            rigidBody.velocity = Vector2.zero;
            return;
        }
        Vector2 direction = (currentPoints[currentTargetPoint].Target - (Vector2)transform.position).normalized;
        rigidBody.velocity = direction * MoveSpeed * Time.deltaTime;

        CheckTargetReached();
    }

    public void KnockBack(Vector2 force)
    {
        rigidBody.AddForce(force, ForceMode2D.Impulse);
        StartCoroutine(RecoverFromKnockBack());
    }

    private void CheckTargetReached()
    {
        if (Vector2.Distance(transform.position, currentPoints[currentTargetPoint].Target).Approximately(0, 0.01f))
        {
            StartCoroutine(RestForSeconds(currentPoints[currentTargetPoint].RestTime));
            currentTargetPoint = currentTargetPoint < currentPoints.Length - 1 ? currentTargetPoint + 1 : 0;
        }
    }

    private IEnumerator RestForSeconds(float seconds)
    {
        isResting = true;
        yield return new WaitForSeconds(seconds);
        isResting = false;
    }

    private IEnumerator RecoverFromKnockBack()
    {
        isRecoveringFromKnockBack = true;
        yield return new WaitForSeconds(KnockBackRecoverTime);
        isRecoveringFromKnockBack = false;
    }
}
