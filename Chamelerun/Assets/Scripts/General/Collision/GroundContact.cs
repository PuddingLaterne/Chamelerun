﻿using UnityEngine;
using System.Collections;

public class GroundContact : MonoBehaviour 
{
    public LayerMask GroundLayers;
    public float MaxDistance;
    public float Width;

    public bool HasGroundContact
    {
        get
        {
            if (lastUpdated != Time.frameCount)
            {
                isGrounded = CheckGround(0) || CheckGround(-Width / 2f) || CheckGround(Width / 2f); 
                lastUpdated = Time.frameCount;
                if (isGrounded)
                {
                    lastGroundedTime = Time.time;
                }
            }
            return isGrounded;
        }
    }

    public float TimeWithoutGroundContact
    {
        get
        {
            return Time.time - lastGroundedTime;
        }
    }

    public Vector2 GroundNormal { get; private set; }
    private bool isGrounded;
    private int lastUpdated = -1;
    private float lastGroundedTime;

    private bool CheckGround(float offsetX)
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(offsetX, 0), Vector2.down, MaxDistance, GroundLayers);
        if (hit)
        {
            GroundNormal = hit.normal;
            return true;
        }
        return false;
    }
}
