using UnityEngine;
using System.Collections;

public class ChameleonMovement : ChameleonBehaviour 
{
    public enum Direction
    {
        Left = -1, 
        Right = 1
    }

    public Vector2 StartingPosition;
    public float JumpCooldown = 0.1f;

    [Header("Speed")]
    public float AirSpeedFraction = 0.8f;
    public float RunningSpeedMultiplier = 1.5f;

    [Header("Swinging")]
    public float SwingForce = 2500;
    public float NormalGravitiyScale = 5;
    public float SwingingGravityScale = 10;

    [Header("Knockback")]
    public float KnockBackForce = 15;
    public float KnockBackAngle = 20;

    public TriggerZone GroundTrigger;

    public Direction CurrentDirection { get; private set; }
    public bool TongueIsAttached { get; private set; }
    public bool IsDangling { get; private set; }

    private Rigidbody2D rigidBody;

    private bool jumpTriggered;
    private bool jumpingEnabled;

    private Vector2 lastPosition;

    public override void Init(Chameleon chameleon)
    {
        base.Init(chameleon);

        rigidBody = GetComponent<Rigidbody2D>();
        Reset();
    }

    public override void Reset()
    {
        StopAllCoroutines();

        transform.position = StartingPosition;
        lastPosition = transform.position;
        CurrentDirection = Direction.Right;

        jumpingEnabled = true;
        jumpTriggered = false;
        IsDangling = false;
        TongueIsAttached = false;

        rigidBody.gravityScale = NormalGravitiyScale;
        rigidBody.freezeRotation = true;
        rigidBody.velocity = Vector2.zero;
        transform.localEulerAngles = Vector3.zero;

        GroundTrigger.Reset();
    }

    public override void ChameleonUpdate()
    {
        IsDangling = TongueIsAttached && !GroundTrigger.IsActive;

        if (InputHelper.JumpInput)
        {
            if ((GroundTrigger.IsActive || IsDangling) && jumpingEnabled)
            {
                jumpTriggered = true;
                StartCoroutine(WaitForJumpCooldown());
            }
        }

        Vector2 positionDifference = ((Vector2)transform.position - lastPosition);
        if (positionDifference.x < 0)
        {
            CurrentDirection = Direction.Left;
        }
        if (positionDifference.x > 0)
        {
            CurrentDirection = Direction.Right;
        }
        lastPosition = transform.position;
    }
	
    public void FixedUpdate()
    {
        float velocityX = InputHelper.HorizontalInput * Time.fixedDeltaTime;
        if (!IsDangling)
        {
            float speed = chameleon.Power.GetGroundSpeed();
            if (InputHelper.RunningInput)
            {
                speed *= RunningSpeedMultiplier;
            }

            if (GroundTrigger.IsActive)
            {
                velocityX *= speed;

                rigidBody.velocity = new Vector2(velocityX, rigidBody.velocity.y);
            }
            else
            {
                velocityX *= speed * AirSpeedFraction;
                if (velocityX != 0f)
                {
                    rigidBody.velocity = new Vector2(velocityX, rigidBody.velocity.y);
                }
            }
        }
        else
        {
            rigidBody.AddRelativeForce(new Vector2(velocityX * SwingForce, 0));
        }

        if (jumpTriggered)
        {
            float jumpStrength = chameleon.Power.GetJumpStrength();
            rigidBody.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            jumpTriggered = false;
        }
    }

    public void KnockBack()
    {
        Vector2 direction = Vector2.up.Rotate(KnockBackAngle * (int)CurrentDirection);
        rigidBody.AddForce(direction * KnockBackForce, ForceMode2D.Impulse);
    }

    public void Throw(Vector2 force)
    {
        rigidBody.AddForce(force);
    }

    public void OnTongueAttached()
    {
        rigidBody.gravityScale = SwingingGravityScale;
        rigidBody.freezeRotation = false;
        TongueIsAttached = true;
    }

    public void OnTongueReleased()
    {
        TongueIsAttached = false;
        rigidBody.gravityScale = NormalGravitiyScale;
        rigidBody.freezeRotation = true;
        transform.localEulerAngles = Vector3.zero;
    }

    private IEnumerator WaitForJumpCooldown()
    {
        jumpingEnabled = false;
        yield return new WaitForSeconds(JumpCooldown);
        jumpingEnabled = true;
    }
}
