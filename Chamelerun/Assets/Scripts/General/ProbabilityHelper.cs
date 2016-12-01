using UnityEngine;
using System.Collections;

public static class ProbabilityHelper
{
    public static bool RollDice(int probability)
    {
        int value = Random.Range(0, 101);
        return value <= probability;
    }
}
