using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }
    private static UIManager instance;

    public UIColours Colours { get; private set; }
    public UIPowerupDisplay PowerupDisplay { get; private set; }

    public void Init()
    {
        Colours = FindObjectOfType<UIColours>();
        PowerupDisplay = FindObjectOfType<UIPowerupDisplay>();

        Chameleon chameleon = GameManager.Instance.Chameleon;
        chameleon.OnPowerChanged += () =>
            {
                PowerupDisplay.UpdatePowerDisplay(chameleon.CurrentPower);
            };
        PowerupDisplay.UpdatePowerDisplay(chameleon.CurrentPower);
    }
}
