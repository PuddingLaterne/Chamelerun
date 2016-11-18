using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPowerupDisplay : MonoBehaviour 
{
    public Image[] Power = new Image[3];

    public void UpdatePowerDisplay(PowerupType[] power)
    {
        for(int i = 0; i < power.Length; i++)
        {
            Power[i].color = UIManager.Instance.Colours.GetPowerupColor(power[i]);
        }
    }
}
