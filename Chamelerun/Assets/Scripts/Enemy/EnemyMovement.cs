using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour 
{
    [System.Serializable]
    public struct Point
    {
        public Vector2 Target;
        public float RestTime;
        public float ApproachSpeed;
    }

    public Point[] Points;
    public float StoppingDistance = 0.1f;
    public float KnockBackRecoverTime = 0.5f;

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
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i].Target = (Vector2)transform.position + Points[i].Target;
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

        Vector2 direction = (Points[currentTargetPoint].Target - (Vector2)transform.position).normalized;
        rigidBody.velocity = direction * Points[currentTargetPoint].ApproachSpeed * Time.deltaTime;

        CheckTargetReached();
    }

    public void KnockBack(Vector2 force)
    {
        rigidBody.AddForce(force, ForceMode2D.Impulse);
        StartCoroutine(RecoverFromKnockBack());
    }

    private void CheckTargetReached()
    {
        if (Vector2.Distance(transform.position, Points[currentTargetPoint].Target) <= StoppingDistance)
        {
            StartCoroutine(RestForSeconds(Points[currentTargetPoint].RestTime));
            currentTargetPoint = currentTargetPoint < Points.Length - 1 ? currentTargetPoint + 1 : 0;
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
