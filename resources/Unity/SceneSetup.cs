using UnityEngine;

/// <summary>
/// Spawns the Unity objects in the scene to provide a debug environment for STEP export.
/// </summary>
public class SceneSetup
{    
    private GameObject coordinateSystem;
    
    /// <summary>
    /// Simulate a right handed coordinate system (Z up) and spawn the sample model.
    /// </summary>
    /// <returns>Coordinate systems root GameObject.</returns>
    public GameObject Setup()
    {
        coordinateSystem = new GameObject("simulated_right_handed_coordinate_system_Z_up");
        coordinateSystem.transform.eulerAngles = new Vector3(-90f, 0, 0);
        coordinateSystem.transform.localScale = new Vector3(-1, 1, 1);
        // add scene gizmos
        coordinateSystem.AddComponent<CADCoordinateSystem>();
        
        // create debug origin cube
        SpawnBox(
            parent: coordinateSystem.transform,
            name: "origin_box_10mm_10mm_10mm_red",
            center: Vector3.zero,
            dimension: new Vector3(0.01f, 0.01f, 0.01f),
            rotation: Vector3.zero,
            color: Color.red
            );

        SpawnSampleContent();

        Debug.Log("'simulated_right_handed_coordinate_system_Z_up' object spawned.");

        return coordinateSystem;
    }  
    
    /// <summary>
    /// Spawns the example workstation.
    /// </summary>
    private void SpawnSampleContent()
    {
        GameObject workstation = SpawnGroup(
           name: "workstation",
           center: new Vector3(0, 0, 0),
           rotation: new Vector3(0, 0, 0),          
           parent: coordinateSystem.transform
          );

        GameObject table = SpawnGroup(
            name: "table",
            center: new Vector3(0, 0, 0),
            rotation: new Vector3(0, 0, 0),
            parent: workstation.transform
           );

        GameObject stuff = SpawnGroup(
          name: "stuff",
          center: new Vector3(0, 0, 0.63f),
          rotation: new Vector3(0, 0, 0),
          parent: workstation.transform
         );

        GameObject foundation = SpawnGroup(
            name: "foundation",
            center: new Vector3(0, 0, 0),
            rotation: new Vector3(0, 0, 0),
            parent: table.transform
           );

        SpawnBox(
            name: "leg1",
            center: new Vector3(-0.75f, -0.25f, 0.3f),
            dimension: new Vector3(0.1f, 0.1f, 0.6f),
            rotation: new Vector3(0, 0, 0),
            color: Color.green,
            parent: foundation.transform
           );

        SpawnBox(
            name: "leg2",
            center: new Vector3(0.75f, -0.25f, 0.3f),
            dimension: new Vector3(0.1f, 0.1f, 0.6f),
            rotation: new Vector3(0, 0, 0),
            color: Color.green,
            parent: foundation.transform
           );

        SpawnBox(
            name: "leg3",
            center: new Vector3(-0.75f, 0.25f, 0.3f),
            dimension: new Vector3(0.1f, 0.1f, 0.6f),
            rotation: new Vector3(0, 0, 0),
            color: Color.green,
            parent: foundation.transform
           );

        SpawnBox(
            name: "leg4",
            center: new Vector3(0.75f, 0.25f, 0.3f),
            dimension: new Vector3(0.1f, 0.1f, 0.6f),
            rotation: new Vector3(0, 0, 0),
            color: Color.green,
            parent: foundation.transform
           );

        SpawnBox(
            name: "tabletop",
            center: new Vector3(0, 0, 0.615f),
            dimension: new Vector3(1.6f, 0.6f, 0.030f),
            rotation: new Vector3(0, 0, 0),
            color: Color.blue,
            parent: foundation.transform
           );

        GameObject drawer = SpawnGroup(
           name: "drawer",
           center: new Vector3(-0.55f, 0, 0.45f),
           rotation: new Vector3(0, 0, 0),
           parent: workstation.transform
          );

        SpawnBox(
            name: "walls",
            center: new Vector3(0, 0.05f, 0),
            dimension: new Vector3(0.3f, 0.5f, 0.3f),
            rotation: new Vector3(0, 0, 0),
            color: Color.yellow,
            parent: drawer.transform
           );

        SpawnBox(
            name: "knob1",
            center: new Vector3(0, -0.225f, 0.05f),
            dimension: new Vector3(0.1f, 0.050f, 0.035f),
            rotation: new Vector3(0, 0, 0),
            color: Color.black,
            parent: drawer.transform
           );

        SpawnBox(
            name: "knob2",
            center: new Vector3(0, -0.225f, -0.06f),
            dimension: new Vector3(0.1f, 0.050f, 0.035f),
            rotation: new Vector3(0, 0, 0),
            color: Color.black,
            parent: drawer.transform
           );

        SpawnBox(
            name: "book1",
            center: new Vector3(0.54f, 0.035f, 0.030f),
            dimension: new Vector3(0.25f, 0.36f, 0.06f),
            rotation: new Vector3(0, 0, -35),
            color: Color.white,
            parent: stuff.transform
           );

        SpawnBox(
            name: "book2",
            center: new Vector3(0.54f, 0.035f, 0.09f),
            dimension: new Vector3(0.25f, 0.36f, 0.06f),
            rotation: new Vector3(0, 0, -38),
            color: Color.yellow,
            parent: stuff.transform
           );

        GameObject monitor = SpawnGroup(
           name: "monitor",
           center: new Vector3(0, 0.17f, 0),
           rotation: new Vector3(0, 0, -4),
           parent: stuff.transform
          );

        SpawnBox(
            name: "socket",
            center: new Vector3(0, 0, 0.020f),
            dimension: new Vector3(0.25f, 0.15f, 0.040f),
            rotation: new Vector3(0, 0, 0),
            color: Color.black,
            parent: monitor.transform
           );

        SpawnBox(
            name: "arm",
            center: new Vector3(0, 0, 0.19f),
            dimension: new Vector3(0.1f, 0.03f, 0.3f),
            rotation: new Vector3(0, 0, 0),
            color: Color.black,
            parent: monitor.transform
           );

        SpawnBox(
            name: "display",
            center: new Vector3(0, -0.01f, 0.34f),
            dimension: new Vector3(0.6f, 0.02f, 0.338f),
            rotation: new Vector3(0, 0, 0),
            color: Color.black,
            parent: monitor.transform
           );

        SpawnBox(
            name: "keyboard",
            center: new Vector3(-0.11f, -0.075f, 0),
            dimension: new Vector3(0.560f, 0.185f, 0.04f),
            rotation: new Vector3(0, 0, -6),
            color: Color.red,
            parent: stuff.transform
           );
    }
    
    /// <summary>
    /// Spawns a cube with given parameters in the scene.
    /// </summary>    
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
        go.transform.localScale = dimension;
        go.transform.localPosition = center;
        go.transform.localEulerAngles = rotation;        
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return go;
    }

    /// <summary>
    /// Spawns an empty GameObject with given parameters in the scene.
    /// </summary>   
    private GameObject SpawnGroup
                    (
                        Transform parent,
                        string name,
                        Vector3 center,                      
                        Vector3 rotation                  
                    )
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);      
        go.transform.localPosition = center;
        go.transform.localEulerAngles = rotation;
        go.transform.localScale = Vector3.one;
       
        return go;
    }

}
