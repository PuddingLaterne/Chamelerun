using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ChameleonBody : MonoBehaviour 
{
    public enum Direction
    {
        None = 0,
        Left = -1, 
        Right = 1
    }

    private enum JumpType
    {
        None, Waiting,
        Normal, Wall, Swinging
    }

    [Header("Jumping")]
    public float JumpCooldown = 0.1f;
    public float AirJumpTolerance = 0.5f;
    public float GroundProximityTolerance = 0.5f;

    [Header("Speed")]
    public float AirSpeedFraction = 0.9f;
    public AnimationCurve Acceleration;
    public AnimationCurve SlopeSpeed;

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

    public UnityAction OnJump = delegate{};
    
    public Vector2 Position { get { return transform.position; } }
    public Direction CurrentDirection { get; private set; }
    public bool IsDangling { get; private set; }
    public bool IsGrounded { get { return groundContact.HasGroundContact; } }
    public float VelocityY { get { return rigidBody.velocity.y; } }
    public float VelocityX { get { return rigidBody.velocity.x; } }

    private Rigidbody2D rigidBody;
    private GroundContact groundContact;

    private bool jumpingEnabled, isJumping;
    private JumpType jumpType;
    private bool isFlying;
    private bool isRecoveringFromKnockBack;

    private Vector2 lastPosition;
    private float accelerationTime, lastHorizontalInput;

    private ChameleonTongue tongue;
    private ChameleonPower power;

    public void Init(ChameleonTongue tongue, ChameleonPower power)
    {
        this.tongue = tongue;
        this.power = power;

        rigidBody = GetComponent<Rigidbody2D>();
        groundContact = GetComponentInChildren<GroundContact>();
    }

    public void Reset()
    {
        StopAllCoroutines();

        lastPosition = transform.position;
        CurrentDirection = Direction.Right;

        jumpingEnabled = true;
        jumpType = JumpType.None;
        isJumping = false;
        isRecoveringFromKnockBack = false;
        isFlying = false;

        rigidBody.gravityScale = NormalGravitiyScale;
        rigidBody.freezeRotation = true;
        rigidBody.velocity = Vector2.zero;
        transform.localEulerAngles = Vector3.zero;

        accelerationTime = 0f;
        lastHorizontalInput = 0f;
    }

    public void ChameleonUpdate()
    {
        IsDangling = tongue.IsAttached && !groundContact.HasGroundContact;

        isJumping = isJumping && rigidBody.velocity.y > 0;
        isFlying = isFlying && !groundContact.HasGroundContact;

        if (InputHelper.JumpPressed || jumpType == JumpType.Waiting)
        {
            if (jumpingEnabled)
            {
                if (groundContact.HasGroundContact)
                {
                    jumpType = JumpType.Normal;
                }
                else if (IsDangling)
                {
                    jumpType = JumpType.Swinging;
                }
                else if (groundContact.TimeWithoutGroundContact < AirJumpTolerance)
                {
                    jumpType = JumpType.Normal;
                }
            }
            else if(jumpType == JumpType.None)
            {
                if (rigidBody.velocity.y < 0 && Physics2D.Raycast(Position, Vector2.down, GroundProximityTolerance, groundContact.GroundLayers))
                {
                    jumpType = JumpType.Waiting;
                }
            }
        }

        if (IsDangling)
        {
            Vector2 positionDifference = ((Vector2)transform.position - lastPosition);
            if (positionDifference.x < 0)
            {
                CurrentDirection = Direction.Left;
            }
            else if (positionDifference.x > 0)
            {
                CurrentDirection = Direction.Right;
            }
            else
            {
                CurrentDirection = Direction.None;
            }
        }
        else
        {
            if (InputHelper.HorizontalInput < 0)
            {
                CurrentDirection = Direction.Left;
            }
            else if (InputHelper.HorizontalInput > 0)
            {
                CurrentDirection = Direction.Right;
            }
            else
            {
                CurrentDirection = Direction.None;
            }
        }
        lastPosition = transform.position;
    }
	
    public void FixedUpdate()
    {
        if (jumpType != JumpType.None)
        {
            Jump();
        }
        else
        {
            if (isJumping && !InputHelper.JumpHeld && !IsDangling)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            }
        }

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
    }

    private void Move(float horizontalInput)
    {
        if (isRecoveringFromKnockBack) return;

        accelerationTime += Time.fixedDeltaTime;
        float maxSpeed = power.GetGroundSpeed();
        float acceleration = Acceleration.Evaluate(accelerationTime);

        float speed = maxSpeed * acceleration * horizontalInput;

        if (!groundContact.HasGroundContact)
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
            else
            {
                speed *= SlopeSpeed.Evaluate(Vector2.Dot((Vector2.right * horizontalInput).normalized, groundContact.GroundNormal));
            }
        }

        rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
    }

    private void Jump()
    {
        float jumpStrength = power.GetJumpStrength();
        Vector2 force = Vector2.zero;
        switch(jumpType)
        {
            case JumpType.Normal:
                rigidBody.gravityScale = NormalGravitiyScale;
                force = Vector2.up * jumpStrength;
                break;
            case JumpType.Swinging:
                Vector2 direction = Vector2.up.Rotate(-SwingReleaseJumpAngle * Mathf.Sign(InputHelper.HorizontalInput));
                force = direction * jumpStrength * SwingReleaseJumpMultiplier;
                break;
        }
        rigidBody.AddForce(force, ForceMode2D.Impulse);

        isJumping = true;
        OnJump();
        StartCoroutine(WaitForJumpCooldown());
        jumpType = JumpType.None;
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
        isFlying = !groundContact.HasGroundContact;
        rigidBody.gravityScale = NormalGravitiyScale;
        rigidBody.freezeRotation = true;
        transform.localEulerAngles = Vector3.zero;
    }

    private IEnumerator WaitForJumpCooldown()
    {
        jumpingEnabled = false;
        yield return new WaitForSeconds(JumpCooldown);
        yield return new WaitUntil(() => groundContact.HasGroundContact || IsDangling);
        jumpingEnabled = true;
    }

    private IEnumerator WaitForKnockBackRecovery()
    {
        isRecoveringFromKnockBack = true;
        yield return new WaitForSeconds(KnockBackRecoveryTime);
        isRecoveringFromKnockBack = false;
    }
}
