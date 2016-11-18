using UnityEngine;
using System.Collections;

public class UIColours : MonoBehaviour 
{
    public Color Default;

    public Color Red;
    public Color Yellow;
    public Color Blue;

    public Color GetPowerupColor(PowerupType type)
    {
        switch(type)
        {
            case PowerupType.Red:
                return Red;
            case PowerupType.Yellow:
                return Yellow;
            case PowerupType.Blue:
                return Blue;
            default:
                return Default;
        }
    }
}
