﻿using UnityEngine;
using System.Collections;

public static class InputHelper 
{
    private static bool usingJoystickInput = true;

    private static Vector2 defaultTargetPosition = Vector2.up.Rotate(-45);
    private static float manualTargetSpeed = 8;
    private static float automaticTargetSpeed = 0.5f;
    private static float timeSinceLastTargetMovement;
    private static float targetRestingTime = 1f;
    
    private static Vector2 currentTargetPosition;

    private static float aimingAngle;
    private static float horizontalInput;
    private static float verticalInput;
    private static bool jumpInput;
    private static bool tongueInput;
    private static bool runningInput;

    private static int lastUpdatedFrameCount;

    public static void Reset()
    {
        lastUpdatedFrameCount = -1;
        timeSinceLastTargetMovement = float.NegativeInfinity;
        currentTargetPosition = defaultTargetPosition;
    }

    public static float AimingAngle
    {
        get
        {
            EnsureInputIsUpdated();
            return aimingAngle;
        }
    }

    public static float HorizontalInput
    {
        get
        {
            EnsureInputIsUpdated();
            return horizontalInput;
        }
    }

    public static float VerticalInput
    {
        get
        {
            EnsureInputIsUpdated();
            return verticalInput;
        }
    }

    public static bool JumpInput
    {
        get
        {
            EnsureInputIsUpdated();
            return jumpInput;
        }
    }

    public static bool TongueInput
    {
        get
        {
            EnsureInputIsUpdated();
            return tongueInput;
        }
    }

    public static bool RunningInput
    {
        get
        {
            EnsureInputIsUpdated();
            return runningInput;
        }
    }

    private static void EnsureInputIsUpdated()
    {
        if (lastUpdatedFrameCount != Time.frameCount)
        {
            UpdateInput();
            lastUpdatedFrameCount = Time.frameCount;
        }
    }

    private static void UpdateInput()
    {
        jumpInput = Input.GetButtonDown("Jump");
        tongueInput = Input.GetButtonDown("Tongue");
        runningInput = Input.GetButton("Run");

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        aimingAngle = CalculateAimingAngle();
    }

    private static float CalculateAimingAngle()
    {
        Vector2 aimingDirection;
        if (usingJoystickInput)
        {
            Vector2 targetDirection = new Vector2(Input.GetAxis("Horizontal_2").SignedSquared(), Input.GetAxis("Vertical_2").SignedSquared());
            if (targetDirection.magnitude.Approximately(0, 0.1f))
            {
                if (timeSinceLastTargetMovement == float.NegativeInfinity)
                {
                    timeSinceLastTargetMovement = -targetRestingTime;
                }
                else
                {
                    timeSinceLastTargetMovement += Time.deltaTime;
                }
                targetDirection = (defaultTargetPosition - currentTargetPosition);
                if (targetDirection.magnitude.Approximately(0, 0.1f))
                {
                    currentTargetPosition = defaultTargetPosition;
                }
                else if(timeSinceLastTargetMovement > 0f)
                {
                    targetDirection.Normalize();
                    currentTargetPosition += targetDirection * (automaticTargetSpeed * timeSinceLastTargetMovement).SignedSquared();
                }
            }
            else
            {
                timeSinceLastTargetMovement = float.NegativeInfinity;
                currentTargetPosition += targetDirection * manualTargetSpeed * Time.deltaTime;
                if (currentTargetPosition.magnitude > 1)
                {
                    currentTargetPosition.Normalize();
                }
            }
            aimingDirection = currentTargetPosition;
        }
        else
        {
            Ray cursorPositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2 cursorPosition = cursorPositionRay.origin + cursorPositionRay.direction * -Camera.main.transform.position.z;
            Vector2 relativeToPosition = (Vector2)GameManager.Instance.Chameleon.Movement.transform.position;
            aimingDirection = (cursorPosition - relativeToPosition).normalized;
        }
        float angle = Vector2.Angle(Vector2.up, aimingDirection);
        Vector3 cross = Vector3.Cross(Vector2.up, aimingDirection);
        if (cross.z < 0)
        {
            angle *= -1;
        }
        return angle;
    }
}
