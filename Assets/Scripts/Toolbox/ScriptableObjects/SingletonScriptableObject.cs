using System.Linq;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            var name = typeof(T).Name;
            if (!_instance)
            {
                _instance = Resources.Load<T>(name);
                if (_instance == null) Debug.LogWarning("No scriptable object asset found at Resources/" + name);
            }

            return _instance;
        }
    }
}