﻿using UnityEngine;
using System.Collections;

public class CameraTargetTracking : MonoBehaviour 
{
    [System.Serializable]
    public struct ToleranceValues
    {
        public float Right;
        public float Left;
        public float Top;
        public float Bottom;
    }
    public ToleranceValues Tolerance;

    public Transform Target;
    public float SmoothTime;
    public float TargetDistanceThreshold;

    private Vector3 velocity;
    private LevelSegmentManager levelSegmentManager;

    public void Init(LevelSegmentManager levelSegmentManager)
    {
        this.levelSegmentManager = levelSegmentManager;
    }

    public void LateUpdate()
    {
        if (Target == null) return;

        Vector3 posDifference = transform.position - Target.transform.position;
        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds();

        Vector3 targetPos = transform.position;        

        float normalizedPositionX = Mathf.Abs(posDifference.x / screenBounds.extents.x);
        if(posDifference.x < 0 && normalizedPositionX > Tolerance.Right)
        {
            targetPos.x = Target.transform.position.x - screenBounds.extents.x * Tolerance.Right;
        }
        if(posDifference.x > 0 && normalizedPositionX > Tolerance.Left)
        {
            targetPos.x = Target.transform.position.x + screenBounds.extents.x * Tolerance.Left;
        }

        float normalizedPositionY = Mathf.Abs(posDifference.y / screenBounds.extents.y);
        if (posDifference.y < 0 && normalizedPositionY > Tolerance.Top)
        {
            targetPos.y = Target.transform.position.y - screenBounds.extents.y * Tolerance.Top;
        }
        if(posDifference.y > 0 && normalizedPositionY > Tolerance.Bottom)
        {
            targetPos.y = Target.transform.position.y + screenBounds.extents.y * Tolerance.Bottom;
        }


        if (targetPos.x - screenBounds.extents.x < levelSegmentManager.CurrentMaxBacktrackingPositionX)
        {
            targetPos.x = levelSegmentManager.CurrentMaxBacktrackingPositionX + screenBounds.extents.x;
        }

        if (targetPos.y - screenBounds.extents.y < 0)
        {
            targetPos.y = screenBounds.extents.y;
        }

        transform.position = targetPos;
    }
}
