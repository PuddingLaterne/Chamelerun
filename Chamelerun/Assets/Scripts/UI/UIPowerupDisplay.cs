using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPowerupDisplay : MonoBehaviour 
{
    public RectTransform Container;
    public Image[] Power = new Image[3];

    private UIColours colours;

    public void Init(UIColours colours)
    {
        this.colours = colours;
    }

    public void UpdatePowerDisplay(PowerupType[] power)
    {
        if (Container != null)
        {
            UIAnimationHelper.ScaleEmphasisShort(Container);
        }
        for(int i = 0; i < power.Length; i++)
        {
            Power[i].color = colours.GetPowerupColor(power[i]);
        }
    }
}
