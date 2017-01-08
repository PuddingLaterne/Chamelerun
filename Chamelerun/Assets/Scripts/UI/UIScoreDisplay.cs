using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScoreDisplay : MonoBehaviour 
{
    public Text ScoreText;

    public void UpdateScore(int newScore)
    {
        ScoreText.text = newScore.ToString("D9");
        UIAnimationHelper.ScaleEmphasisShort(ScoreText.rectTransform);
    }
	
}
