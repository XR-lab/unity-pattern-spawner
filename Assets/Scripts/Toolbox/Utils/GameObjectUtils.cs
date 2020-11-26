using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectUtils
{
    public static void DestroyChildren(GameObject target)
    {
        foreach (Transform child in target.transform) Object.DestroyImmediate(child.gameObject);
    }
}