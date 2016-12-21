using UnityEngine;
using System.Collections;

public static class ProbabilityHelper
{
    public static bool RollDice(int probability)
    {
        int value = Random.Range(0, 101);
        return value <= probability;
    }

    public static int PickRandom(int[] elements)
    {
        return elements[Random.Range(0, elements.Length)];
    }
}
