using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITimeDisplay : MonoBehaviour 
{
    public Text ScoreText;

    public void UpdateTimer(float time)
    {
        ScoreText.text = (int)time + ":" + (int)((time - (int)time) * 100);
    }
	
}
