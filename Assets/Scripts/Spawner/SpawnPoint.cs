using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private string type = "";
    [SerializeField] private string classification = "";
    [SerializeField] private string specificPrefab = "";

    public float MaxVolume
    {
        get => maxVolume;
        set => maxVolume = value;
    }

    public float MinVolume
    {
        get => minVolume;
        set => minVolume = value;
    }

    public string Type
    {
        get => type;
        set => type = value;
    }

    public string Classificationn
    {
        get => classification;
        set => classification = value;
    }

    public string SpecificPrefab
    {
        get => specificPrefab;
        set => specificPrefab = value;
    }
}