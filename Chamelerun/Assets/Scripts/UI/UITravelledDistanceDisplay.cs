using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITravelledDistanceDisplay : MonoBehaviour 
{
    public Text DistanceText;

    public void UpdateDistance(float newDistance)
    {
        DistanceText.text = ((int)newDistance).ToString("D9");
    }
}
