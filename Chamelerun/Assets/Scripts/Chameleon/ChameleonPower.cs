using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class ChameleonPower : ChameleonBehaviour 
{
    public AnimationCurve TongueLength;
    public AnimationCurve JumpStrength;
    public AnimationCurve MoveSpeed;

    public int FullPowerupMultiplier = 2;

    public UnityAction OnPowerChanged = delegate { };
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

    private int red;
    private int yellow;
    private int blue;

    public override void Init(Chameleon chameleon)
    {
        base.Init(chameleon);
        Reset();
    }

    public override void Reset()
    {
        powerups = new PowerupType[maxNumPowerups];
        RecalculatePower();
        OnPowerChanged();
    }

    public float GetMaxTongueLength()
    {
        return TongueLength.Evaluate(red);
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
        PowerupType[] tmpPowerups = new PowerupType[maxNumPowerups];
        tmpPowerups[0] = type;
        for(int i = 1; i < maxNumPowerups; i++)
        {
            tmpPowerups[i] = powerups[i - 1];
        }
        powerups = tmpPowerups;
        RecalculatePower();
        OnPowerChanged();
    }

    public void RemovePowerup()
    {
        for (int i = maxNumPowerups - 1; i >= 0; i--)
        {
            if(powerups[i] != PowerupType.None)
            {
                powerups[i] = PowerupType.None;
                RecalculatePower();
                OnPowerChanged();
                return;
            }
        }
        OnAllPowerLost();
    }

    private int GetPowerupCount()
    {
        int count = 0;
        foreach(PowerupType powerup in powerups)
        {
            if(powerup != PowerupType.None)
            {
                count++;
            }
        }
        return count;
    }

    private bool FullPowerup()
    {
        return GetPowerupCount() == maxNumPowerups;
    }

    private bool AllPowerupsAreSameType()
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

    private bool AllPowerupsAreDifferentTypes()
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

    private int CountPowerupsOfType(PowerupType type)
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

        if(FullPowerup())
        {
            if(AllPowerupsAreDifferentTypes())
            {
                red *= FullPowerupMultiplier;
                yellow *= FullPowerupMultiplier;
                blue *= FullPowerupMultiplier;
            }
        }
    }
}
