using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
  public override void OnInspectorGUI()
  {
    // Generate default inspector fields
    DrawDefaultInspector();

    MapGenerator mapGenerator = (MapGenerator)target;
    // Button for generating map
    if (GUILayout.Button("Generate map")) {
      Debug.Log("Generating map");
      mapGenerator.GenerateMap();
    }

    // Button for clearing map
    if (GUILayout.Button("Clear map")) {
      Debug.Log("Clearing map");
      mapGenerator.ClearMap();
    }
  }
}
