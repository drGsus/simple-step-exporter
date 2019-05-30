using SimpleStepWriter.Content;
using SimpleStepWriter.Content.Internal;
using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.IO;

namespace SimpleStepWriter
{
    /// <summary>
    /// Library API.
    /// </summary>
    public sealed class StepFile
    {
        public string FilePath { get; private set; }
        public string AssemblyName { get; private set; }

        private StepManager stepManager;

        // API parent child relationship
        public long NextNodeGuid { get; private set; }
        public long MaxGuid { get; private set; }

        // flat list of content elements      
        private Dictionary<long, IChild> guidToIChild;
        private Dictionary<long, long> iChildGuidToParentGuid;
        private RootAssembly rootAssembly;

        private List<string> stepLines;
        private List<string> logLines;

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
            guidToIChild = new Dictionary<long, IChild>();
            iChildGuidToParentGuid = new Dictionary<long, long>();

            // init guid
            NextNodeGuid = 0;
            MaxGuid = NextNodeGuid;
            
            // create root assembly (only one root assembly is currently supported)
            long guid = NewGuid();
            rootAssembly = new RootAssembly(this.stepManager, assemblyName, guid);
        }

        public void AddBox(string name, Vector3 center, Vector3 dimension, Vector3 rotation, Color color, long parentGuid)       
        {
            long guid = NewGuid();

            // create a box
            Box box = new Box(stepManager, name, center, dimension, rotation, color, guid);
            guidToIChild.Add(guid, box);
            iChildGuidToParentGuid.Add(guid, parentGuid);

            // we don't need to return the guid cause it's not useful for anything with the box type
            return;
        }

        public long AddGroup(string name, Vector3 center, Vector3 rotation, long parentGuid)
        {
            long guid = NewGuid();

            // create a group
            Group group = new Group(stepManager, name, center, rotation, guid);
            guidToIChild.Add(guid, group);
            iChildGuidToParentGuid.Add(guid, parentGuid);

            // return guid for reference
            return guid;
        }

        /// <summary>
        /// Write current specified content to file.
        /// </summary>
        /// <returns>Succes information.</returns>
        public bool WriteFile()
        {
            // create tree data structure
            BuildHierarchyStructure();

            // new STEP content
            stepLines = new List<string>();
            logLines = new List<string>();

            // traverse hierachy and generate STEP file lines
            Traverse(rootAssembly); // root
            
            // now we have our Tree structure and we can start building the actual STEP content:

            

            

            /*
            // write mandatory intro data
            stepLines.AddRange(Default.FileStart);
            stepLines.AddRange(Default.Header(this.FilePath, "unknown-description"));
            stepLines.AddRange(Default.DataStart);
            
            // write root assembly (required for now)
            RootGroup rootAssembly = new RootGroup(stepManager, AssemblyName);
            long[] childrenCoordinateSystemsIds;
            stepLines.AddRange(rootAssembly.GetLines(fileContent.ToArray(), out childrenCoordinateSystemsIds));

            // write all boxes
            for(int i = 0; i < fileContent.Count; i++)
            {
                stepLines.AddRange(fileContent[i].GetLines(childrenCoordinateSystemsIds[i], i+1 ));
            }
            
            // in order to be a valid STEP file, close the data section after we have written our custom content 
            stepLines.AddRange(Default.DataEnd);
            stepLines.AddRange(Default.FileEnd);
            */
                        
            try
            {
                // STEP
                File.WriteAllLines(FilePath, stepLines);

                // LOGS
                string logPath = Path.GetDirectoryName(FilePath) + "/stepLog.txt";
                File.WriteAllLines(logPath, logLines);

                return true;
                
            } catch
            {
                return false;
            }   
        }


        private void Traverse(IParent node)
        {
            for(int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];

                if(child.GetType() == typeof(Group))
                {
                    logLines.AddRange(new[] { "+ Group: " + ((Group)child).Parent.Name + " / " + ((Group)child).Name });

                    Traverse((IParent)child);
                }
                else if(child.GetType() == typeof(Box))
                {
                    logLines.AddRange(new[] { "+ Box: " + ((Box)child).Parent.Name + " / " + ((Box)child).Name });
                }
                else
                {
                    throw new System.Exception("Woot.");
                }
            }             
        }

        /// <summary>
        /// Build the IContent tree structure based on our parent/child relationship information.
        /// </summary>
        private void BuildHierarchyStructure()
        {
            // for all IContent objects with GUID
            for (int i = 0; i <= MaxGuid; i++)
            {
                // iterate through all IContent objects 
                for (int j = 1; j <= MaxGuid; j++)
                {
                    // find child objects
                    if (iChildGuidToParentGuid[j] == i)
                    {
                        // special case for root assembly
                        if(i == 0)
                        {
                            rootAssembly.Children.Add(guidToIChild[j]);

                            if(guidToIChild[j].Parent == null)
                                guidToIChild[j].Parent = rootAssembly;
                        }
                        else
                        {
                            if (guidToIChild[i].GetType() != typeof(Group))
                                throw new System.Exception("Can't happen.");

                            ((Group)guidToIChild[i]).Children.Add(guidToIChild[j]);

                            if (guidToIChild[j].Parent == null)
                                guidToIChild[j].Parent = (Group)guidToIChild[i];
                        }

                    }

                }
            }

        }

        /// <summary>
        /// Issue a new GUID which uniquely references an IContent object.
        /// </summary>
        /// <returns>The new GUID.</returns>
        private long NewGuid()
        {
            long guid = NextNodeGuid;
            MaxGuid = NextNodeGuid;

            NextNodeGuid++;
           
            return guid;
        }

    }
}
