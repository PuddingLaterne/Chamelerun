﻿using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[System.Serializable]
public struct PowerLevel
{
    public int Red;
    public int Yellow;
    public int Blue;

    public PowerLevel(int red, int yellow, int blue)
    {
        Red = red;
        Yellow = yellow;
        Blue = blue;
    }


    public static bool operator ==(PowerLevel levelA, PowerLevel levelB)
    {
        return levelA.Red == levelB.Red && levelA.Yellow == levelB.Yellow && levelA.Blue == levelB.Blue;
    }

    public static bool operator !=(PowerLevel levelA, PowerLevel levelB)
    {
        return !(levelA == levelB);
    }

    public static bool operator >=(PowerLevel levelA, PowerLevel levelB)
    {
        return levelA.Red >= levelB.Red && levelA.Yellow >= levelB.Yellow && levelA.Blue >= levelB.Blue;
    }

    public static bool operator <=(PowerLevel levelA, PowerLevel levelB)
    {
        return levelA.Red <= levelB.Red && levelA.Yellow <= levelB.Yellow && levelA.Blue <= levelB.Blue;
    }

    public override string ToString()
    {
        return "Red: " + Red + ", Yellow: " + Yellow + ", Blue: " + Blue;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class ChameleonPower : MonoBehaviour 
{
    public AnimationCurve TongueLength;
    public AnimationCurve TongueWidth;
    public AnimationCurve JumpStrength;
    public AnimationCurve MoveSpeed;

    public int FullPowerupMultiplier = 2;

    public UnityAction OnPowerChanged = delegate { };
    public UnityAction<PowerupType> OnPowerupAdded = delegate { };
    public UnityAction OnAllPowerLost = delegate { };

    private const int maxNumPowerups = 3;

    public PowerupType[] Powerups
    {
        get
        {
            return (PowerupType[])powerups.Clone();
        }
    }
    private PowerupType[] powerups;

    public int Power
    {
        get
        {
            return red + yellow + blue;
        }
    }

    public PowerLevel PowerLevel
    {
        get
        {
            return new PowerLevel(red, yellow, blue);
        }
    }

    [SerializeField]
    private int red;
    [SerializeField]
    private int yellow;
    [SerializeField]
    private int blue;

    public void Reset()
    {
        powerups = new PowerupType[maxNumPowerups];
        RecalculatePower();
        OnPowerChanged();
    }

    public float GetMaxTongueLength()
    {
        return TongueLength.Evaluate(red);
    }

    public float GetTongueWidth()
    {
        return TongueWidth.Evaluate(red);
    }

    public float GetGroundSpeed()
    {
        float speed = MoveSpeed.Evaluate(yellow);
        return speed;
    }

    public float GetJumpStrength()
    {
        return JumpStrength.Evaluate(blue);
    }

    public void AddPowerup(PowerupType type)
    {
        if (type == PowerupType.Clear) return;
        PowerupType[] tmpPowerups = new PowerupType[maxNumPowerups];
        tmpPowerups[0] = type;
        for(int i = 1; i < maxNumPowerups; i++)
        {
            tmpPowerups[i] = powerups[i - 1];
        }
        powerups = tmpPowerups;
        RecalculatePower();
        OnPowerupAdded(type);
        OnPowerChanged();
    }

    public void RemovePowerup()
    {
        for (int i = maxNumPowerups - 1; i >= 0; i--)
        {
            if(powerups[i] != PowerupType.Clear)
            {
                powerups[i] = PowerupType.Clear;
                RecalculatePower();
                OnPowerChanged();
                return;
            }
        }
        OnAllPowerLost();
    }

    public int GetPowerupCount()
    {
        int count = 0;
        foreach(PowerupType powerup in powerups)
        {
            if(powerup != PowerupType.Clear)
            {
                count++;
            }
        }
        return count;
    }

    public bool FullPowerup()
    {
        return GetPowerupCount() == maxNumPowerups;
    }

    public bool AllPowerupsAreSameType()
    {
        for (int i = 1; i < maxNumPowerups; i++)
        {
            if(powerups[i] != powerups[i - 1])
            {
                return false;
            }
        }
        return true;
    }

    public bool AllPowerupsAreDifferentTypes()
    {
        List<PowerupType> occuringTypes = new List<PowerupType>();
        for (int i = 0; i < maxNumPowerups; i++)
        {
            if(occuringTypes.Contains(powerups[i]))
            {
                return false;
            }
            occuringTypes.Add(powerups[i]);
        }
        return true;
    }

    public int CountPowerupsOfType(PowerupType type)
    {
        int count = 0;
        foreach(PowerupType powerup in powerups)
        {
            if(powerup == type)
            {
                count++;
            }
        }
        return count;
    }

    private void RecalculatePower()
    {
        red = CountPowerupsOfType(PowerupType.Red);
        yellow = CountPowerupsOfType(PowerupType.Yellow);
        blue = CountPowerupsOfType(PowerupType.Blue);

        if(FullPowerup() && AllPowerupsAreDifferentTypes())
        {
            red *= FullPowerupMultiplier;
            yellow *= FullPowerupMultiplier;
            blue *= FullPowerupMultiplier;
        }
    }
}
