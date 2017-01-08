using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIPowerupMessageDisplay : MonoBehaviour
{
    public Text PowerupMessage;
    public Text SpecialPowerupMessage;

    public void OnPowerupAdded(PowerupType powerup)
    {
        string text = "";

        switch (powerup)
        {
            case PowerupType.Blue:
                text = "Jump Strength Up!";
                break;
            case PowerupType.Red:
                text = "Tongue Size Up!";
                break;
            case PowerupType.Yellow:
                text = "Speed Up!";
                break;
        }

        PowerupMessage.text = text;
        PowerupMessage.gameObject.SetActive(true);
        UIAnimationHelper.ScaleEmphasisLong(PowerupMessage.rectTransform, () => PowerupMessage.gameObject.SetActive(false));
    }

    public void OnPowerChanged(PowerupType[] power, bool fullPowerup, bool allDifferent, bool allSame)
    {
        string text = "";
        if(fullPowerup)
        {
            if(allDifferent)
            {
                text = "Rainbow Mode!";
            }
            else if(allSame)
            {
                switch(power[0])
                {
                    case PowerupType.Blue:
                        text = "Full Jumping Power!";
                        break;
                    case PowerupType.Red:
                        text = "Full Tongue Size!";
                        break;
                    case PowerupType.Yellow:
                        text = "Full Speed!";
                        break;
                }
            }
        }
        if(text != "")
        {
            SpecialPowerupMessage.text = text;
            SpecialPowerupMessage.gameObject.SetActive(true);
            UIAnimationHelper.ScaleEmphasisLong(SpecialPowerupMessage.rectTransform, () => SpecialPowerupMessage.gameObject.SetActive(false));
        }
    }
}
