using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SpawnPatternWindow : EditorWindow {
	
	private string _prefabName = "";
	private string _patternPath = "Resources/Patterns/";
	private string _spawnPointPrefabName = "SpawnPoint";
	private bool _showSpawnPointPrefabs = true;

	[MenuItem("Window/SpawnPattern")]
	public static void ShowWindow ()
	{
		GetWindow<SpawnPatternWindow>("SpawnPattern");
	}

	void OnGUI ()
	{
		if (GUILayout.Button("Start new pattern"))
		{
			StartPattern();
		}
		
		GUILayout.Label("Add a new spawn point", EditorStyles.boldLabel);

		if (GUILayout.Button("Add new spawn point"))
		{
			AddPatternElement();
		}
		
		if (GUILayout.Button("Clone selected spawn point"))
		{
			CopySpawnPoint();
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		GUILayout.Label("Preview this pattern", EditorStyles.boldLabel);
		_showSpawnPointPrefabs = EditorGUILayout.Toggle("Show the default cubes", _showSpawnPointPrefabs);
		if (GUILayout.Button("Preview selected spawnpoints"))
		{
			Preview(Selection.gameObjects);
		}
		if (GUILayout.Button("Preview all spawnpoints"))
		{
			var spawnPoints = GetAllSpawnPoints();
			Preview(spawnPoints.ToArray());
		}
		if (GUILayout.Button("Remove preview"))
		{
			RemovePreview();
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Label("Save pattern as prefab (name is optional)", EditorStyles.boldLabel);
		_prefabName = EditorGUILayout.TextField("New prefab name: ", _prefabName);
		if (GUILayout.Button("Save pattern"))
		{
			CreatePrefab();
		}
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Label("Settings", EditorStyles.boldLabel);
		_patternPath = EditorGUILayout.TextField("Patterns storage folder: ", _patternPath);

		ShowSpawnPointPrefabs(_showSpawnPointPrefabs);
	}

	private void ShowSpawnPointPrefabs(bool isVisible)
	{
		var spawnPoints = GetAllSpawnPoints();
		foreach (var spawnPoint in spawnPoints)
		{
			var renderer = spawnPoint.GetComponent<Renderer>();
			renderer.enabled = isVisible;
		}
	}

	void StartPattern()
	{
		var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (var gameObject in gameObjects)
		{
			if(!IsSpawnPoint(gameObject))
				continue;
			
			DestroyImmediate(gameObject);
		}
	}

	bool IsSpawnPoint(GameObject target)
	{
		return target.GetComponent<SpawnPoint>() != null;
	}

	void Preview(GameObject[] targets)
	{
		PatternSpawner.Instance.ResetCache();
		ResetPreview(targets);

		foreach (GameObject obj in targets)
		{
			var spawnPoint = obj.GetComponent<SpawnPoint>();

			var matchedElement = PatternSpawner.Instance.GetLibraryElement(spawnPoint);

			var newElement = Instantiate(matchedElement.prefab);
			newElement.transform.position = obj.transform.position;
			newElement.transform.parent = obj.transform;
			newElement.transform.localScale = newElement.transform.localScale / obj.transform.localScale.x;
			PatternSpawner.Instance.Customize(newElement, matchedElement);
		}
	}

	void RemovePreview()
	{
		var allSpawnPoints = GetAllSpawnPoints();
		ResetPreview(allSpawnPoints.ToArray());
	}

	void ResetPreview(GameObject[] targetSpawnPoints)
	{
		foreach (GameObject obj in targetSpawnPoints)
		{
			if (!IsSpawnPoint(obj))
				continue;
			
			GameObjectUtils.DestroyChildren(obj);
		}
	}

	void AddPatternElement()
	{
		string[] spawnPointFrefabs = AssetDatabase.FindAssets(_spawnPointPrefabName, new[] {"Assets"});
		if (spawnPointFrefabs.Length == 0)
		{
			return;
		}

		var spawnPointPath = AssetDatabase.GUIDToAssetPath(spawnPointFrefabs[0]);
		Debug.Log(spawnPointPath);
		GameObject spawnPoint = (GameObject)AssetDatabase.LoadAssetAtPath(spawnPointPath, typeof(GameObject));
		Debug.Log(spawnPoint);
		var newSpawnElement = Instantiate(spawnPoint);
		Selection.activeGameObject = newSpawnElement;
		
	}
	
	void CopySpawnPoint()
	{
		var spawnPoint = Selection.activeObject;
		
		var newSpawnElement = Instantiate(spawnPoint) as GameObject;
		newSpawnElement.transform.position = new Vector3(0f, 0f, 0f);
		Selection.activeGameObject = newSpawnElement;
		
	}

	List<GameObject> GetAllSpawnPoints()
	{
		List<GameObject> spawnPoints = new List<GameObject>();
		var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		var gameObjects = SceneUtils.GetAllObjectsInScene(rootObjects);
		
		foreach (GameObject gameObject in gameObjects)
		{
			if (!gameObject.name.Contains(_spawnPointPrefabName))
			{
				continue;
			}

			spawnPoints.Add(gameObject);
		}

		return spawnPoints;
	}

	void CreatePrefab()
	{
		RemovePreview();

		List<GameObject> objectArray = GetAllSpawnPoints();

		if (objectArray.Count == 0)
		{
			Debug.LogWarning("Warning: no spawnpoints available. Creation of prefab is cancelled.");
			return;
		}

		var name = _prefabName != "" ? _prefabName : "New_Prefab_pattern";
		string localPath = "Assets/" + _patternPath + name + ".prefab";
		localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

		var prefab = new GameObject();
		prefab.name = name;

		foreach (GameObject gameObject in objectArray)
		{
			gameObject.transform.parent = prefab.transform;
		}

		Debug.Log(localPath);
		PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, localPath, InteractionMode.UserAction);

	}


}
