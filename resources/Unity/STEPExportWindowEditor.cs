using UnityEngine;
using UnityEditor;

/// <summary>
/// Provides editor GUI to setup the debug environment and export the scene content to file.
/// </summary>
public class STEPExportWindow : EditorWindow
{   
    private string path = @"C:/Users/me/Documents/UnityScene.step";
    private SceneSetup sceneSetup;
    private StepExport stepExport;
    private GameObject rootCoordinateSystem;

    [MenuItem("Window/STEP Export")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(STEPExportWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("STEP Export", EditorStyles.boldLabel);

        GUILayout.Space(20f);

        // create the simulated right handed coordinate system and spawn the sample model
        GUILayout.Label("Setup right handed coordinate system:", EditorStyles.label);
        if (GUILayout.Button("Setup"))
        {
            if (GameObject.Find("simulated_right_handed_coordinate_system_Z_up") != null)
            {
                Debug.LogWarning("GameObject with name 'simulated_right_handed_coordinate_system_Z_up' already exists. Delete the GameObject first.");
                return;
            }

            sceneSetup = new SceneSetup();
            stepExport = new StepExport();

            rootCoordinateSystem = sceneSetup.Setup();
        }

        GUILayout.Space(20f);

        // specify export path and provide option to export the scene content to file
        GUILayout.Label("Specify export path and write file:", EditorStyles.label);
        EditorGUI.BeginDisabledGroup(sceneSetup == null || rootCoordinateSystem == null);
        path = GUILayout.TextField(path);
        GUILayout.Space(4f);
        if (GUILayout.Button("Export STEP"))
        {
            stepExport.WriteSceneToStepFile(path, rootCoordinateSystem);
        }
        EditorGUI.EndDisabledGroup();

    }
}
