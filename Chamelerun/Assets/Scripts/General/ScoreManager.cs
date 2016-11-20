using UnityEngine;
using System.Collections;

public class ScoreManager 
{
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ScoreManager();
            }
            return instance;
        }
    }
    private static ScoreManager instance;
}
