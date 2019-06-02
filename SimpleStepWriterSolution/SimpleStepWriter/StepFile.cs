using SimpleStepWriter.Content;
using SimpleStepWriter.Content.Internal;
using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleStepWriter
{
    /// <summary>
    /// Library API.
    /// </summary>
    public sealed class StepFile
    {
        public const int ASSEMBLY_ROOT_ID = 0;

        public string FilePath { get; private set; }
        public string AssemblyName { get; private set; }

        private StepManager stepManager;
        private Dictionary<int, IChild> guidToContent;
        private Dictionary<int, int> guidToParentGuid;
        private RootAssembly rootAssembly;

        /// <summary>
        /// Create an instance of this object, add several content types to it and then write the information to disk as a STEP AP214 file.
        /// </summary>
        /// <param name="path">Path where you want the STEP file to be written. Will be overwritten if it already exists.</param>
        /// <param name="assemblyName">Name of the root assembly. Will be visible in all CAD programs.</param>
        public StepFile(string path, string assemblyName)
        {
            this.FilePath = path;
            this.AssemblyName = assemblyName;
            stepManager = new StepManager();

            // keep track of all assembly children and each parent object
            guidToContent = new Dictionary<int, IChild>();
            guidToParentGuid = new Dictionary<int, int>();
            
            // create root assembly (only one root assembly is currently supported)
            int id = NewId();
            rootAssembly = new RootAssembly(this.stepManager, assemblyName, id);
        }

        /// <summary>
        /// Add a new box with given parameters to STEP file.
        /// A box can't have any children and always has a parent.
        /// </summary>      
        /// <param name="name">Name of the box that is visible in the CAD hierarchy.</param>     
        /// <param name="position">Position of the box relative to parent. Also the center of the box.</param>
        /// <param name="dimension">Box dimension (it's the complete length of an edge, not half of it)</param>
        /// <param name="rotation">Box rotation relative to parent (around the provided position).</param>
        /// <param name="color">Box color. Transparency not supported yet.</param>
        /// <param name="parentId">Id of the parent object (group or root assembly).</param>  
        public void AddBox(string name, Vector3 position, Vector3 dimension, Vector3 rotation, Color color, int parentId)       
        {
            int id = NewId();

            // create a box
            Box box = new Box(stepManager, name, position, dimension, rotation, color, id);
            guidToContent.Add(id, box);
            guidToParentGuid.Add(id, parentId);

            // we don't need to return the guid cause it's not useful for anything with the box type
            return;
        }

        /// <summary>
        /// Add a new group with given parameters to STEP file.
        /// A group can have multiple children (boxes and groupss) and always has a parent.
        /// </summary>       
        /// <param name="name">Name of the group that is visible in the CAD hierarchy.</param>     
        /// <param name="position">Position of the group relative to parent.</param>       
        /// <param name="rotation">Group rotation relative to parent (around the provided position).</param>     
        /// <param name="parentId">Id of the parent object (group or root assembly).</param>    
        public int AddGroup(string name, Vector3 position, Vector3 rotation, int parentId)
        {
            int guid = NewId();

            // create a group
            Group group = new Group(stepManager, name, position, rotation, guid);
            guidToContent.Add(guid, group);
            guidToParentGuid.Add(guid, parentId);

            // return guid for reference
            return guid;
        }

        /// <summary>
        /// Write current specified content to file.
        /// </summary>
        /// <returns>Succes information.</returns>
        public bool WriteFile()
        {
            // list of all objects we want to write to the STEP file as string
            List<string> stepEntries = new List<string>();
            bool success = GetStepData(in stepEntries);

            try
            {
                // write STEP file
                File.WriteAllLines(FilePath, stepEntries);
                
                return true;
                
            } catch
            {
                return false;
            }   
        }

        /// <summary>
        /// Write current specified content to byte array.
        /// </summary>
        /// <returns>STEP content as byte array. Null if something went wrong.</returns>
        public byte[] GetByteData()
        {
            // list of all objects we want to write to the STEP file as string
            List<string> stepEntries = new List<string>();
            return GetStepData(in stepEntries) ? stepEntries.SelectMany(s => Encoding.ASCII.GetBytes(s)).ToArray() : null;            
        }

        /// <summary>
        /// Collect STEP information.
        /// </summary>
        /// <param name="stepEntries">Empty list with all step entries.</param>
        /// <returns>List of STEP content,</returns>
        private bool GetStepData(in List<string> stepEntries)
        {
            try
            {
                // throws if invalid parent data was passed in
                BuildTreeDatasStructure();
            }
            catch
            {
                return false;
            }

            // now we have our tree data structure and we can start building the actual STEP content

            // used for representing string information of a single object that we want to write to the STEP file
            StringBuilder stringBuilder = new StringBuilder();

            // write start content
            Default.GetHeader(this.FilePath, "unknown-description", in stringBuilder, in stepEntries);
            rootAssembly.GetLines(0, in stringBuilder, in stepEntries);

            // write all child content
            GetAllLines(rootAssembly, in stringBuilder, in stepEntries);

            // write end content
            Default.GetFooter(in stringBuilder, in stepEntries);

            return true;
        }

        /// <summary>
        /// Verify provided parent/child relationship and build the IContent tree structure based on that information.
        /// </summary>
        private void BuildTreeDatasStructure()
        {           
            // for all IContent objects with GUID
            for (int i = 1; i <= maxContentId; i++)
            {
                var child = guidToContent[i];                
                int parentId = guidToParentGuid[i];

                if (parentId != 0 && !guidToContent.ContainsKey(parentId))
                {
                    throw new System.Exception("Provided parent ID is unknown.");
                }

                if (parentId == 0)
                    child.Parent = rootAssembly;
                else
                {
                    IParent parent = guidToContent[parentId] as IParent;

                    if (parent != null)
                    {
                        child.Parent = parent;
                    }
                    else
                    {
                        throw new System.Exception("Provided parent ID doesn't belong to an object that implements IParent interface.");
                    }
                }              
              
                child.Parent.Children.Add(child);
            }
        }

        /// <summary>
        /// Iterate through all child objects and collect STEP information.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="stringBuilder">StringBuilder object used for storing information about a single hierarchy object.</param>
        /// <param name="stepEntries">String list containing all STEP information.</param>
        private void GetAllLines(IParent parent, in StringBuilder stringBuilder, in List<string> stepEntries)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                var child = parent.Children[i];
                child.GetLines(i, in stringBuilder, in stepEntries);

                var asParent = child as IParent;
                if (asParent != null)
                    GetAllLines(asParent, in stringBuilder, in stepEntries);
            }
        }

        // ID information
        private int nextContentId;
        private int maxContentId;

        /// <summary>
        /// Issue a new ID which uniquely references an IContent object.
        /// </summary>
        /// <returns>The new ID.</returns>
        private int NewId()
        {
            int id = nextContentId;
            maxContentId = nextContentId;

            nextContentId++;
           
            return id;
        }

    }
}
