using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DebugStep))]
public class DebugStepEditor : Editor
{
    private string path = @"C:/Users/me/Documents/UnityScene.step";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugStep debugStep = (DebugStep)target;
        
        // input field to customize output path
        path = GUILayout.TextField(path);

        // build the simulated right handed coordinate system and sample model
        if (GUILayout.Button("Setup"))
        {
            debugStep.Setup();
        }
        // export all child boxes under root coordiante system GameObject
        if (GUILayout.Button("Export step"))
        {
            debugStep.WriteSceneToStepFile(path);
        }
    }

}
