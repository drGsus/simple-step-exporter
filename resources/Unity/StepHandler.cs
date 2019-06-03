using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collects the data arranged under the custom right handed coordinate system and writes that content as a STEP file.
/// Make sure that you have build the SimpleStepWriter.dll library and copied that in a Unity projects 'Plugins' folder.
/// </summary>
public class StepExport
{
    private GameObject coordinateSystem;
    private SimpleStepWriter.StepFile stepFile;

    private Node rootNode;
    private int nextId;
    
    // If you want to export the boxes from Unity make sure that you have the SimpleStepWriter DLL available in your project.
    public void WriteSceneToStepFile(string path, GameObject coordinateSystem)
    {
        this.coordinateSystem = coordinateSystem;

        stepFile = new SimpleStepWriter.StepFile(path, "UnityScene-Assembly");
        
        // init own tree data structure
        rootNode = new Node();
        rootNode.Id = 0;
        rootNode.Parent = null;
        nextId = 1;

        // traverse Unity hierarchy and build own data structure
        TraverseHierarchyToNode(coordinateSystem, rootNode);

        // traverse own data structure and pass values to SimpleStepWriter library
        TraverseNodeToLib(rootNode);

        // write STEP file
        bool success = stepFile.WriteFile();

        if (success)
            Debug.Log("Step writing succeeded:\n" + path);
        else
            Debug.LogError("Step writing failed.");
    }

    // traverse Unity hierarchy and build own data structure
    private void TraverseHierarchyToNode(GameObject go, Node node)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            
            var childNode = new Node();            
            childNode.Id = nextId;
            nextId++;
            childNode.ParentId = node.Id;
            childNode.Parent = node;
            childNode.Go = child.gameObject;

            node.Children.Add(childNode);

            TraverseHierarchyToNode(child.gameObject, childNode);       
        }
    }

    // traverse own data structure and pass values to SimpleStepWriter library
    private void TraverseNodeToLib(Node node)
    {
        for (int i = 0; i < node.Children.Count; i++)
        {
            Node childNode = node.Children[i];
            
            // group
            if (childNode.Children.Count > 0)
            {
                stepFile.AddGroup
                (
                   name: childNode.Go.name,
                   position: new SimpleStepWriter.Helper.Vector3(childNode.Go.transform.localPosition.x * 1000,
                                                               childNode.Go.transform.localPosition.y * 1000,
                                                               childNode.Go.transform.localPosition.z * 1000
                                                              ),
                   rotation: new SimpleStepWriter.Helper.Vector3(childNode.Go.transform.localEulerAngles.x,
                                                                 childNode.Go.transform.localEulerAngles.y,
                                                                 childNode.Go.transform.localEulerAngles.z
                                                                ),
                   parentId: childNode.ParentId
               );
            }
            // box
            else if(childNode.Go.GetComponent<MeshRenderer>() != null)
            {
                stepFile.AddBox
                (
                   name: childNode.Go.name,
                   position: new SimpleStepWriter.Helper.Vector3(childNode.Go.transform.localPosition.x * 1000,
                                                               childNode.Go.transform.localPosition.y * 1000,
                                                               childNode.Go.transform.localPosition.z * 1000
                                                              ),
                   dimension: new SimpleStepWriter.Helper.Vector3(childNode.Go.transform.localScale.x * 1000,
                                                                  childNode.Go.transform.localScale.y * 1000,
                                                                  childNode.Go.transform.localScale.z * 1000
                                                                 ),
                   rotation: new SimpleStepWriter.Helper.Vector3(childNode.Go.transform.localEulerAngles.x,
                                                                 childNode.Go.transform.localEulerAngles.y,
                                                                 childNode.Go.transform.localEulerAngles.z
                                                                ),
                   color: new SimpleStepWriter.Helper.Color(childNode.Go.transform.GetComponent<MeshRenderer>().sharedMaterial.color.r,
                                                            childNode.Go.transform.GetComponent<MeshRenderer>().sharedMaterial.color.g,
                                                            childNode.Go.transform.GetComponent<MeshRenderer>().sharedMaterial.color.b,
                                                            1f
                                                           ),
                   parentId: childNode.ParentId
               );
            }
            else
            {
                Debug.LogWarning("Object couldn't be identified as neither Box or Group: skip object.");
            }

            TraverseNodeToLib(childNode);
        }
    }
}

/// <summary>
/// Node class to build our internal tree data structure.
/// </summary>
public class Node
{
    public GameObject Go;
    public int Id;
    public int ParentId;
    public Node Parent;
    public List<Node> Children;

    public Node()
    {
        Children = new List<Node>();
    }
}
