using UnityEditor;
using UnityEngine;

/// <summary>
/// Draw debug right handed coordiante system (Z up) gizmo.
/// </summary>
public class CADCoordinateSystem : MonoBehaviour
{
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
}
