using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public sealed class PatternSpawner
{
    // class implements Singleton design pattern

    public Action<List<GameObject>> onPatternSpawned;

    private GameObject container;

    static PatternSpawner()
    {
    }

    private PatternSpawner()
    {
    }

    public static PatternSpawner Instance { get; } = new PatternSpawner();

    public void RemakeObjects()
    {
        _allPatterns = Resources.LoadAll<GameObject>("Patterns");
        container = new GameObject("Spawn Container");
    }
    
    private GameObject[] _allPatterns;

    private Dictionary<string, SpawnPatternLibrary.SpawnElement> _typeCache;

    public void Spawn(float width, float height)
    {
        // todo: implement this method. This method should fill an area with patterns
    }

    public void SpawnPattern(Vector3 position)
    {
        SpawnPattern(position.x, position.y, position.z);
    }

    public void SpawnPattern(float x, float y, float z)
    {
        ResetCache();
        var randomIndex = Random.Range(0, _allPatterns.Length);
        var randomPattern = _allPatterns[randomIndex];
        var spawnedObjects = new List<GameObject>();

        // loop over all spawnpoints
        foreach (Transform child in randomPattern.transform)
        {
            var newObject = SpawnObject(child, x, y, z);
            spawnedObjects.Add(newObject);
        }

        onPatternSpawned?.Invoke(spawnedObjects);
    }
    
    private GameObject SpawnObject(Transform spawnPoint, float x, float y, float z)
    {
        var pointSettings = spawnPoint.GetComponent<SpawnPoint>();
        var matchedLibraryElement = MatchLibraryElement(pointSettings);
        var newPrefab = InstantiatePrefab(matchedLibraryElement.prefab, x + spawnPoint.position.x, y + spawnPoint.position.y,
            z + spawnPoint.position.z);
        Customize(newPrefab, matchedLibraryElement);
        return newPrefab;
    }

    public void Customize(GameObject target, SpawnPatternLibrary.SpawnElement matchedLibraryElement)
    {
        var heightRange = matchedLibraryElement.heightRange;
        var horizontalRotationRange = matchedLibraryElement.horizontalRotationRange;
        var verticalRotationRange = matchedLibraryElement.verticalRotationRange;
        target.transform.localScale = new Vector3(1f, Random.Range(heightRange.min, heightRange.max), 1f);
        target.transform.eulerAngles = new Vector3(Random.Range(verticalRotationRange.min, verticalRotationRange.max),
            Random.Range(horizontalRotationRange.min, horizontalRotationRange.max), 0);
    }

    private GameObject InstantiatePrefab(GameObject matchedPrefab, float x, float y, float z)
    {
        if (matchedPrefab == null) return null;
        var newElement = Object.Instantiate(matchedPrefab);
        newElement.transform.position = new Vector3(x, y, z);
        newElement.transform.parent = container.transform;
        StaticBatchingUtility.Combine(newElement);

        return newElement;
    }

    public void ResetCache()
    {
        _typeCache = new Dictionary<string, SpawnPatternLibrary.SpawnElement>();
    }

    public SpawnPatternLibrary.SpawnElement MatchLibraryElement(SpawnPoint settings)
    {
        SpawnPatternLibrary.SpawnElement targetLibraryElement;
        if (settings.Type != "" && _typeCache.ContainsKey(settings.Type))
        {
            targetLibraryElement = _typeCache[settings.Type];
        }
        else
        {
            var possiblePrefabs = SpawnPatternLibrary.Instance.data
                .Where(item => item.volume >= settings.MinVolume && item.volume <= settings.MaxVolume).ToList();
            if (possiblePrefabs.Count == 0)
                Debug.Log("No object found to spawn with spawnpoint settings: Minvolume: " + settings.MinVolume +
                                 ", MaxVolume: " + settings.MaxVolume);
            
            var randomIndex = Random.Range(0, possiblePrefabs.Count);
            targetLibraryElement = possiblePrefabs[randomIndex];
        }

        _typeCache[settings.Type] = targetLibraryElement;
        return targetLibraryElement;
    }
}