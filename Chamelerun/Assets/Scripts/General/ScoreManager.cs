﻿using UnityEngine.Events;
using System.Collections;

public class ScoreManager 
{
    private static class PowerupValue
    {
        private static int[] valueByAmountCollected = { 1, 10, 100 };

        public static int GetPoints(int amountCollected)
        {
            return amountCollected < valueByAmountCollected.Length ?
                valueByAmountCollected[amountCollected] :
                valueByAmountCollected[valueByAmountCollected.Length - 1];
        }
    }

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

    public UnityAction<int> OnScoreChanged = delegate { };
    public UnityAction<float> OnTravelledDistanceChanged = delegate { };

    public int CurrentScore 
    {
        get { return currentScore; }
        private set
        {
            currentScore = value;
            OnScoreChanged(currentScore);
        }
    }
    private int currentScore;


    public float CurrentTravelledDistance
    {
        get { return currentTravelledDistance; }
        private set
        {
            currentTravelledDistance = value;
            OnTravelledDistanceChanged(currentTravelledDistance);
        }
    }
    private float currentTravelledDistance;

    public void Reset()
    {
        CurrentScore = 0;
        CurrentTravelledDistance = 0;
    }

    public void Update(Chameleon chameleon)
    {
        float currentDistanceToStart = chameleon.Position.x;
        if (currentDistanceToStart > CurrentTravelledDistance)
        {
            CurrentTravelledDistance = currentDistanceToStart;
        }
    }

    public void AddPointsForPowerup()
    {
        CurrentScore += PowerupValue.GetPoints(GameManager.Instance.Chameleon.CurrentPower);
    }

    public void AddPoints(int points)
    {
        CurrentScore += points;
    }
}