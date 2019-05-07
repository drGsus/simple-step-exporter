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
        // for now our STEP file only contains boxes
        private List<Box> fileContent;

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
            fileContent = new List<Box>();
        }

        /// <summary>
        /// Add a box to the STEP file.
        /// </summary>
        /// <param name="name">Name of the part that is visible in the CAD hierarchy.</param>
        /// <param name="center">Box position in world space (based on center of the box).</param>
        /// <param name="dimension">Box dimension (it's the complete length of an edge, not half of it)</param>
        /// <param name="rotation">Box rotation (around the provided position).</param>
        /// <param name="color">Box color. Transparency not supported yet.</param>
        public void AddBox(string name, Vector3 center, Vector3 dimension, Vector3 rotation, Color color)        
        {
            // create a box
            Box box = new Box(stepManager, name, center, dimension, rotation, color);
            fileContent.Add(box);
        }

        /// <summary>
        /// Write current specified content to file.
        /// </summary>
        /// <returns>Succes information.</returns>
        public bool WriteFile()
        {
            List<string> stepLines = new List<string>();

            // write mandatory intro data
            stepLines.AddRange(Default.FileStart);
            stepLines.AddRange(Default.Header(this.FilePath, "unknown-description"));
            stepLines.AddRange(Default.DataStart);
            
            // write root assembly (required for now)
            RootAssembly rootAssembly = new RootAssembly(stepManager, AssemblyName);
            long[] childrenCoordinateSystemsIds;
            stepLines.AddRange(rootAssembly.GetLines(fileContent.Count, out childrenCoordinateSystemsIds));

            // write all boxes
            for(int i = 0; i < fileContent.Count; i++)
            {
                stepLines.AddRange(fileContent[i].GetLines(childrenCoordinateSystemsIds[i], i+1 ));
            }
            
            // in order to be a valid STEP file, close the data section after we have written our custom content 
            stepLines.AddRange(Default.DataEnd);
            stepLines.AddRange(Default.FileEnd);
                        
            try
            {
                File.WriteAllLines(FilePath, stepLines);
                return true;
                
            } catch
            {
                return false;
            }   
        }

    }
}
