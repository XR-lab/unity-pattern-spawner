using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpawnPatternLibrary")]
public class SpawnPatternLibrary : SingletonScriptableObject<SpawnPatternLibrary>
{
    [Serializable]
    public struct MinMaxVector
    {
        public float min;
        public float max;
    }

    [Serializable]
    public struct SpawnElement
    {
        public GameObject prefab;
        public float volume;
        public string classification;
        public MinMaxVector heightRange;
        public MinMaxVector horizontalRotationRange;
        public MinMaxVector verticalRotationRange;
    }

    public SpawnElement[] data;
}