using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (NoiseMapGeneration))]
public class NoiseMapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NoiseMapGeneration mapGen = (NoiseMapGeneration)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateNoiseMap();
        }
    }
}
