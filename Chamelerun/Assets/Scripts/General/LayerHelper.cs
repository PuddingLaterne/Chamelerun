using UnityEngine;
using System.Collections;

public static class LayerHelper
{
    public static int ChameleonLayer
    {
        get
        {
            if (chameleon == -1)
            {
                chameleon = LayerMask.NameToLayer("Chameleon");
            }
            return chameleon;
        }
    }

    private static int chameleon = -1;
}
