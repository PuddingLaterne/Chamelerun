using UnityEngine;
using System.Collections;

public class ChameleonBody : ChameleonBehaviour 
{
    public enum Direction
    {
        None = 0,
        Left = -1, 
        Right = 1
    }

    [Header("Jumping")]
    public float JumpCooldown = 0.1f;
    public float AirJumpTolerance = 0.5f;
    public TriggerZone GroundTrigger;

    [Header("Speed")]
    public float AirSpeedFraction = 0.9f;
    public AnimationCurve Acceleration;

    [Header("Swinging")]
    public float SwingForce = 2500;
    public float NormalGravitiyScale = 5;
    public float SwingingGravityScale = 10;
    public float SwingReleaseJumpAngle = 45;
    public float SwingReleaseJumpMultiplier = 1;

    [Header("Knockback")]
    public float KnockBackForce = 15;
    public float KnockBackAngle = 20;
    public float KnockBackRecoveryTime = 0.3f;

    public Direction CurrentDirection { get; private set; }
    public bool IsDangling { get; private set; }

    private Rigidbody2D rigidBody;

    private bool jumpTriggered, jumpingEnabled, isJumping;
    private bool isFlying;
    private bool isRecoveringFromKnockBack;

    private Vector2 lastPosition;
    private float accelerationTime, lastHorizontalInput;

    public override void Init(Chameleon chameleon)
    {
        base.Init(chameleon);
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public override void Reset()
    {
        StopAllCoroutines();

        lastPosition = transform.position;
        CurrentDirection = Direction.Right;

        jumpingEnabled = true;
        jumpTriggered = false;
        isJumping = false;
        isRecoveringFromKnockBack = false;
        isFlying = false;

        rigidBody.gravityScale = NormalGravitiyScale;
        rigidBody.freezeRotation = true;
        rigidBody.velocity = Vector2.zero;
        transform.localEulerAngles = Vector3.zero;

        accelerationTime = 0f;
        lastHorizontalInput = 0f;

        GroundTrigger.Reset();
    }

    public override void ChameleonUpdate()
    {
        IsDangling = chameleon.Tongue.IsAttached && !GroundTrigger.IsActive;
        isJumping = isJumping && rigidBody.velocity.y > 0;
        isFlying = isFlying && !GroundTrigger.IsActive;

        if (InputHelper.JumpPressed && jumpingEnabled)
        {
            if ((GroundTrigger.IsActive || GroundTrigger.InactiveTime < AirJumpTolerance || IsDangling))
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
        float horizontalInput = InputHelper.HorizontalInput;
        if (!IsDangling)
        {
            Move(horizontalInput);
        }
        else
        {
            rigidBody.AddRelativeForce(new Vector2(horizontalInput * SwingForce * Time.fixedDeltaTime, 0));
        }
        lastHorizontalInput = horizontalInput;

        if (jumpTriggered)
        {
            Jump();
        }
        else
        {
            if (isJumping && !InputHelper.JumpHeld && !IsDangling)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y / 2f);
            }
        }
    }

    private void Move(float horizontalInput)
    {
        if (isRecoveringFromKnockBack) return;

        accelerationTime += Time.fixedDeltaTime;
        float maxSpeed = chameleon.Power.GetGroundSpeed();
        float acceleration = Acceleration.Evaluate(accelerationTime);
        float speed = maxSpeed * acceleration * horizontalInput;

        if (!GroundTrigger.IsActive)
        {
            if (Mathf.Abs(speed) < Mathf.Abs(rigidBody.velocity.x) && isFlying)
            {
                speed = rigidBody.velocity.x;
            }
            else
            {
                speed *= AirSpeedFraction;
            }
        }
        else
        {
            if ((lastHorizontalInput * horizontalInput) < 0 || horizontalInput == 0)
            {
                accelerationTime = 0;
                speed = 0;
            }
        }

        rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
    }

    private void Jump()
    {
        float jumpStrength = chameleon.Power.GetJumpStrength();
        Vector2 direction = Vector2.up;
        if (IsDangling)
        {
            direction = direction.Rotate(-SwingReleaseJumpAngle * InputHelper.HorizontalInput) * SwingReleaseJumpMultiplier;
        }
        rigidBody.AddForce(direction * jumpStrength, ForceMode2D.Impulse);
        jumpTriggered = false;
        isJumping = true;
    }

    public void KnockBack(Direction direction)
    {
        isFlying = false;
        Vector2 forceDirection;
        if (direction != Direction.None)
        {
            forceDirection = Vector2.up.Rotate(KnockBackAngle * (int)direction);
        }
        else
        {
            forceDirection = Vector2.up.Rotate(KnockBackAngle * (int)CurrentDirection);
        }
        rigidBody.AddForce(forceDirection * KnockBackForce, ForceMode2D.Impulse);
        StartCoroutine(WaitForKnockBackRecovery());
    }

    public void AddImpulse(Vector2 force)
    {
        isFlying = false;
        rigidBody.AddForce(force, ForceMode2D.Impulse);
    }

    public void OnTongueAttached()
    {
        isFlying = false;
        rigidBody.gravityScale = SwingingGravityScale;
        rigidBody.freezeRotation = false;
    }

    public void OnTongueReleased()
    {
        isFlying = !GroundTrigger.IsActive;
        rigidBody.gravityScale = NormalGravitiyScale;
        rigidBody.freezeRotation = true;
        transform.localEulerAngles = Vector3.zero;
    }

    private IEnumerator WaitForJumpCooldown()
    {
        jumpingEnabled = false;
        yield return new WaitForSeconds(JumpCooldown);
        yield return new WaitUntil(() => GroundTrigger.IsActive || IsDangling);
        jumpingEnabled = true;
    }

    private IEnumerator WaitForKnockBackRecovery()
    {
        isRecoveringFromKnockBack = true;
        yield return new WaitForSeconds(KnockBackRecoveryTime);
        isRecoveringFromKnockBack = false;
    }
}
