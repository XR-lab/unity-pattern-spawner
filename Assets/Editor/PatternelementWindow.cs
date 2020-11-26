using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class PatternelementWindow : EditorWindow
{
    private float _maxVolume = 1f;
    private float _minVolume = 0f;
    private string _type = "";
    private string _classification = "";
    private string _specificPrefab = "";

    private GameObject _elementPrefab;

    [MenuItem("Window/PatternElementDetails")]
    public static void ShowWindow()
    {
        GetWindow<PatternelementWindow>("PatternElementDetails");
    }

    void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length == 0);
        GUILayout.Label("Set the properties of one or multiple spawn points", EditorStyles.boldLabel);

        _minVolume = EditorGUILayout.FloatField("Minimum volume:", _minVolume);
        _maxVolume = EditorGUILayout.FloatField("Maximum volume:", _maxVolume);

        GUILayout.Label("Select classifications like nature.trees.* (wildcard)", EditorStyles.boldLabel);
        _classification = EditorGUILayout.TextField("Spawn classification: ", _classification);

//		EditorGUI.BeginDisabledGroup(_specificPrefab != "");
        GUILayout.Label("Elements with the same spawn type will become the same random prefab", EditorStyles.boldLabel);
        _type = EditorGUILayout.TextField("Spawn type: ", _type);
//		EditorGUI.EndDisabledGroup();

//		EditorGUI.BeginDisabledGroup(_type != "");
        GUILayout.Label("Set a specific prefab to disable the random selection", EditorStyles.boldLabel);
        _specificPrefab = EditorGUILayout.TextField("Specific prefab: ", _specificPrefab);
//		EditorGUI.EndDisabledGroup();


        if (GUILayout.Button("Apply"))
        {
            SetProperties();
        }
        

        EditorGUI.EndDisabledGroup();
    }

    void SetProperties()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            var patternElement = obj.GetComponent<SpawnPoint>();
            patternElement.MaxVolume = Mathf.Max(_maxVolume, _minVolume);
            patternElement.MinVolume = Mathf.Min(_maxVolume, _minVolume);
            patternElement.Type = _type;
            patternElement.Classificationn = _classification;
            patternElement.SpecificPrefab = _specificPrefab;

            if (_type != "")
            {
                patternElement.name = _type + " - SpawnPoint";
            }

            var scale = Mathf.Max(0.1f, (_maxVolume + _minVolume) / 2 / 3);

            patternElement.transform.localScale = new Vector3(scale, scale, scale);
            

            var colour = GetUniqueColour(patternElement.name);

            var renderer = patternElement.GetComponent<Renderer>();

            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.color = colour;
            renderer.sharedMaterial = tempMaterial;
        }
    }

    Color GetUniqueColour(string tag = "-")
    {
        var hex = tag.GetHashCode().ToString("X");
        Color color;
        ColorUtility.TryParseHtmlString("#" + hex, out color);

        return color;
    }
}