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

        ChameleonPower.OnPowerChanged += () =>
            {
                PowerupDisplay.UpdatePowerDisplay(GameManager.Instance.Chameleon.Power.GetPowerups());
            };
        PowerupDisplay.UpdatePowerDisplay(GameManager.Instance.Chameleon.Power.GetPowerups());
    }
}
