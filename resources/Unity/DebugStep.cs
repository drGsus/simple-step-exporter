using UnityEditor;
using UnityEngine;

/// <summary>
/// Attach this script to a GameObject in the scene.
/// This will create a debug environment to easily export Unity boxes to STEP file format.
/// Make sure that you have build the SimpleStepWriter.dll library and copied that in a projects 'Plugins' folder.
/// </summary>
public class DebugStep : MonoBehaviour
{
    private GameObject coordinateSystem;

    /// <summary>
    /// Simulate a right handed coordinate system (Z up) and build a sample model.
    /// </summary>
    public void Setup()
    {
        if (GameObject.Find("simulated_right_handed_coordinate_system_Z_up") != null)
        {
            Debug.LogWarning("GameObject with name 'simulated_right_handed_coordinate_system_Z_up' already exists. Delete the GameObject first.");
            return;
        }

        if(coordinateSystem != null)
        {
            DestroyImmediate(coordinateSystem);
            coordinateSystem = null;
            Debug.LogWarning("Rebuild Coordinate system...");
        }

        // simulate the right handed coordiante system
        coordinateSystem = new GameObject("simulated_right_handed_coordinate_system_Z_up");
        coordinateSystem.transform.eulerAngles = new Vector3(-90f, 0, 0);
        coordinateSystem.transform.localScale = new Vector3(-1, 1, 1);
        
        // create debug origin cube
        SpawnBox(
            parent: coordinateSystem.transform,
            name: "origin_box_10mm_10mm_10mm_red",
            center: Vector3.zero,
            dimension: new Vector3(0.01f, 0.01f, 0.01f),
            rotation: Vector3.zero,
            color: Color.red
            );      

        // build the shelf
        SpawnBox(
            parent: coordinateSystem.transform,
            name: "shelf_stand_left",
            center: new Vector3(-0.6f, 0f, 0.7f),
            dimension: new Vector3(0.1f, 0.6f, 1.4f),
            rotation: Vector3.zero,
            color: Color.green
            );
        SpawnBox(
            parent: coordinateSystem.transform,
            name: "shelf_stand_right",
            center: new Vector3(0.6f, 0f, 0.7f),
            dimension: new Vector3(0.1f, 0.6f, 1.4f),
            rotation: Vector3.zero,
            color: Color.green
            );
        SpawnBox(
            parent: coordinateSystem.transform,
            name: "shelf_level_top",
            center: new Vector3(0f, 0f, 1f),
            dimension: new Vector3(1.1f, 0.5f, 0.05f),
            rotation: new Vector3(20f, 0f, 0f),
            color: Color.blue
            );
        SpawnBox(
            parent: coordinateSystem.transform,
            name: "shelf_level_bottom",
            center: new Vector3(0f, 0f, 0.5f),
            dimension: new Vector3(1.1f, 0.5f, 0.05f),
            rotation: new Vector3(20f, 0f, 0f),
            color: Color.yellow
            );

        Debug.Log("'simulated_right_handed_coordinate_system_Z_up' object spawned.");
    }
    
    // If you want to export the boxes from Unity make sure that you have the SimpleStepWriter DLL available in your project.
    public void WriteSceneToStepFile(string path)
    {
        if(coordinateSystem == null)
        {
            Debug.LogWarning("You have to setup the scene first.");
            return;
        }

        SimpleStepWriter.StepFile stepFile = new SimpleStepWriter.StepFile(path, "UnityScene-Assembly");

        for(int i = 0; i < coordinateSystem.transform.childCount; i++)
        {
            Transform go = coordinateSystem.transform.GetChild(i);

            // convert meters to millimeters, convert Unity types to SimpleStepWriter types and afterwards add the box to the file
            stepFile.AddBox
                (
                    name: go.name,
                    center: new SimpleStepWriter.Helper.Vector3( go.localPosition.x*1000,
                                                                 go.localPosition.y*1000,
                                                                 go.localPosition.z*1000
                                                               ),
                    dimension: new SimpleStepWriter.Helper.Vector3( go.localScale.x*1000,
                                                                    go.localScale.y*1000,
                                                                    go.localScale.z*1000
                                                                  ),
                    rotation: new SimpleStepWriter.Helper.Vector3( go.localEulerAngles.x,
                                                                   go.localEulerAngles.y,
                                                                   go.localEulerAngles.z
                                                                 ),
                    color: new SimpleStepWriter.Helper.Color( go.GetComponent<MeshRenderer>().sharedMaterial.color.r,
                                                              go.GetComponent<MeshRenderer>().sharedMaterial.color.g,
                                                              go.GetComponent<MeshRenderer>().sharedMaterial.color.b,
                                                              1f
                                                            )
                );
        }

        bool success = stepFile.WriteFile();

        if (success)
            Debug.Log("Step writing succeeded:\n"+ path);
        else
            Debug.LogError("Step writing failed.");

    }

    // draw debug right handed coordiante system (Z up) gizmo
    public void OnDrawGizmos()
    {
        Handles.Label(new Vector3(2, 0, 2), "Simulated right handed coordinate system (Z up)", GUIStyle.none);
        // x
        var x = new Vector3(-3, 0, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(0, 0, 0), x);
        Handles.Label(x, "X axis", GUIStyle.none);
        // y
        var y = new Vector3(0, 0, -3);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, y);
        Handles.Label(y, "Y axis", GUIStyle.none);
        // z
        var z = new Vector3(0, 3, 0);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(0, 0, 0), z);
        Handles.Label(z, "Z axis", GUIStyle.none);
    }
    
    private GameObject SpawnBox
                    (
                        Transform parent,
                        string name,
                        Vector3 center,
                        Vector3 dimension,
                        Vector3 rotation,
                        Color color
                    )
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(parent);
        go.name = name;
        go.transform.localPosition = center;
        go.transform.localEulerAngles = rotation;
        go.transform.localScale = dimension;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return go;
    }

}
