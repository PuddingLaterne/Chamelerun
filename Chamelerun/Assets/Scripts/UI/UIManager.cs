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
    private UIPowerupDisplay powerupDisplay;
    private UIScoreDisplay scoreDisplay;
    private UITravelledDistanceDisplay distanceDisplay;

    public void Init()
    {
        Colours = FindObjectOfType<UIColours>();

        powerupDisplay = FindObjectOfType<UIPowerupDisplay>();
        scoreDisplay = FindObjectOfType<UIScoreDisplay>();
        distanceDisplay = FindObjectOfType<UITravelledDistanceDisplay>();

        Chameleon chameleon = GameManager.Instance.Chameleon;
        chameleon.OnPowerChanged += () =>
            {
                powerupDisplay.UpdatePowerDisplay(chameleon.CurrentPowerups);
            };
        powerupDisplay.UpdatePowerDisplay(chameleon.CurrentPowerups);

        ScoreManager.Instance.OnScoreChanged += (newScore) =>
            {
                scoreDisplay.UpdateScore(newScore);
            };

        ScoreManager.Instance.OnTravelledDistanceChanged += (newDistance) =>
            {
                distanceDisplay.UpdateDistance(newDistance);
            };
    }
}
