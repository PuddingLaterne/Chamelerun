﻿using UnityEngine;
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

    [Header("Walljumping")]
    public LayerMask WallJumpLayers;
    public float WallAngleTolerance = 30;
    public float WallJumpGravityScale = 0.5f;
    public float WallStickDuration = 0.5f;
    public float WallJumpAngle = 30;
    public float WallJumpMultiplier = 1.2f;

    public Direction CurrentDirection { get; private set; }
    public bool IsDangling { get; private set; }

    private Rigidbody2D rigidBody;

    private bool jumpTriggered, jumpingEnabled, isJumping;
    private bool isFlying;
    private bool isRecoveringFromKnockBack;

    private bool isStickingToWall, isJumpingFromWall;
    private Vector2 wallJumpDirection;
    private GameObject stickedToWall;
    private Coroutine wallReleasingCoroutine;

    private Vector2 lastPosition;
    private float accelerationTime, lastHorizontalInput;

    public override void Init(Chameleon chameleon)
    {
        base.Init(chameleon);
        rigidBody = GetComponent<Rigidbody2D>();

        CollisionEventSource collisionEventSource = GetComponent<CollisionEventSource>();
        collisionEventSource.OnCollisionEnter += CollisionEnter;
        collisionEventSource.OnCollisionExit += CollisionExit;
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
        isStickingToWall = false;
        isJumpingFromWall = false;

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

        isJumpingFromWall = isJumpingFromWall && !(GroundTrigger.IsActive);

        if (InputHelper.JumpPressed && jumpingEnabled)
        {
            if ((GroundTrigger.IsActive || GroundTrigger.InactiveTime < AirJumpTolerance || IsDangling))
            {
                jumpTriggered = true;
                StartCoroutine(WaitForJumpCooldown());
            }
            if (isStickingToWall)
            {
                jumpTriggered = true;
                StartCoroutine(WaitForJumpCooldown());
            }
        }

        Vector2 positionDifference = ((Vector2)transform.position - lastPosition);
        if(positionDifference.x < 0)
        {
            CurrentDirection = Direction.Left;
        }
        else if(positionDifference.x > 0)
        {
            CurrentDirection = Direction.Right;
        }
        else
        {
            CurrentDirection = Direction.None;
        }
        lastPosition = transform.position;
    }
	
    public void FixedUpdate()
    {
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
        if (isRecoveringFromKnockBack || isJumpingFromWall) return;

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
            direction = direction.Rotate(-SwingReleaseJumpAngle * Mathf.Sign(InputHelper.HorizontalInput)) * SwingReleaseJumpMultiplier;
        }
        else if (isStickingToWall)
        {
            direction = wallJumpDirection;
            ReleaseWall();
            isJumpingFromWall = true;
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
        ReleaseWall();
        isJumpingFromWall = false;
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
        yield return new WaitUntil(() => GroundTrigger.IsActive || IsDangling || isStickingToWall);
        jumpingEnabled = true;
    }

    private IEnumerator WaitForKnockBackRecovery()
    {
        isRecoveringFromKnockBack = true;
        yield return new WaitForSeconds(KnockBackRecoveryTime);
        isRecoveringFromKnockBack = false;
    }

    private void CollisionEnter(Collision2D collision)
    {
        if (!chameleon.Tongue.IsAttached && !GroundTrigger.IsActive && 
            collision.gameObject.layer.ToBitmask().IsPartOfBitmask(WallJumpLayers))
        {            
            float collisionAngle = (-collision.contacts[0].normal).GetAngle();
            if (collisionAngle.Approximately(90, WallAngleTolerance))
            {
                StickToWall(collision.gameObject, Direction.Right);
            }
            if (collisionAngle.Approximately(270, WallAngleTolerance))
            {
                StickToWall(collision.gameObject, Direction.Left);
            }
        }
    }

    private void CollisionExit(Collision2D collision)
    {
        if (stickedToWall != null && collision.gameObject == stickedToWall)
        {
            ReleaseWall();
        }
    }

    private void StickToWall(GameObject wallObject, Direction jumpDirection)
    {
        stickedToWall = wallObject;
        wallJumpDirection = Vector2.up.Rotate(-WallJumpAngle * (int)jumpDirection) * WallJumpMultiplier;
        if (wallReleasingCoroutine != null)
        {
            StopCoroutine(wallReleasingCoroutine);
        }
        wallReleasingCoroutine = StartCoroutine(WaitForReleaseWall());
    }

    private void ReleaseWall()
    {
        if (wallReleasingCoroutine != null)
        {
            StopCoroutine(wallReleasingCoroutine);
        }
        isStickingToWall = false;
        rigidBody.gravityScale = IsDangling ? SwingingGravityScale : NormalGravitiyScale;
    }

    private IEnumerator WaitForReleaseWall()
    {
        isStickingToWall = true;
        rigidBody.gravityScale = WallJumpGravityScale;
        yield return new WaitForSeconds(WallStickDuration);
        isStickingToWall = false;
        rigidBody.gravityScale = IsDangling ? SwingingGravityScale : NormalGravitiyScale;
    }
}
