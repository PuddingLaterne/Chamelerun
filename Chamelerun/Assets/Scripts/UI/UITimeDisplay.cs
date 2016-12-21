using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITimeDisplay : MonoBehaviour 
{
    public Text ScoreText;

    public void UpdateTimer(float time)
    {
        string minutes = ((int)(time / 60f)).ToString("D2");
        string seconds = ((int)time % 60).ToString("D2");
        string milliseconds = ((int)((time - (int)time) * 60)).ToString("D2");
        ScoreText.text = minutes + ":" + seconds + ":" + milliseconds;
    }

}
