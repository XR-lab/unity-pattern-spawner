using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneUtils
{
    public static GameObject[] GetAllObjectsInScene(GameObject[] rootObjects)
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (var rootObject in rootObjects)
        {
            AddObject(rootObject, objectsInScene);
        }

        return objectsInScene.ToArray();
    }

    private static void AddObject(GameObject target, List<GameObject> targetList)
    {
        targetList.Add(target);
        foreach (Transform child in target.transform) {
            AddObject(child.gameObject, targetList);
        }
    }
}
