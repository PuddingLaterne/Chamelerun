using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InspectorDictionaryHelper
{
    public static Dictionary<string, ObjectPool> CreateDictionary(ObjectPool[] fromarray)
    {
        var dict = new Dictionary<string, ObjectPool>();
        foreach(var element in fromarray)
        {
            dict.Add(element.gameObject.name, element);
        }
        return dict;
    }
}
