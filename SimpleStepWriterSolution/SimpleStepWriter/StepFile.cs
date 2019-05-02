using SimpleStepWriter.Content;
using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.IO;

namespace SimpleStepWriter
{
    /// <summary>
    /// Library API.
    /// </summary>
    public class StepFile
    {
        public string FilePath { get; private set;  }

        private List<string> fileContent;
        private long nextId;

        /// <summary>
        /// Create an instance fo this object, add several content types to it and then write the information to disk as a STEP AP214 file.
        /// </summary>
        /// <param name="path">Path where you want the STEP file to be written. Will be overwritten if it already exists.</param>
        public StepFile(string path)
        {
            this.FilePath = path;
            fileContent = new List<string>();

            // write mandatory intro data
            fileContent.AddRange(Content.Default.FileStart);
            fileContent.AddRange(Content.Default.Header(this.FilePath, "unknown-description"));
            fileContent.AddRange(Content.Default.DataStart);

            // DataStart already contains 2 hardcoded ID's, so we start with ID 3 for custom data
            nextId = 3;
        }

        /// <summary>
        /// Add a box to the STEP file.
        /// </summary>
        /// <param name="name">Name of the part that is visible in the CAD hierarchy.</param>
        /// <param name="position">Box position in world space (based on center of the box).</param>
        /// <param name="dimension">Box dimension (it's the complete length of an edge, not half of it)</param>
        /// <param name="rotation">Box rotation (around the provided position).</param>
        /// <param name="color">Box color. Transparency not supported yet.</param>
        public void AddBox(string name, Vector3 position, Vector3 dimension, Vector3 rotation, Color color)        
        {
            // create a box
            Box box = new Box(name, position, dimension, rotation, color);

            // append that box to this STEP file instance and update ID
            fileContent.AddRange(box.GenerateDynamicBoxData(nextId));
            nextId += box.LinesAdded;
        }

        /// <summary>
        /// Write current content to file.
        /// </summary>
        /// <returns>Succes information.</returns>
        public bool WriteFile()
        {
            // in order to be a valid STEP file, close the data section after we have written our custom content 
            fileContent.AddRange(Content.Default.DataEnd);
            fileContent.AddRange(Content.Default.FileEnd);
                        
            try
            {
                File.WriteAllLines(FilePath, fileContent);
                return true;
                
            } catch
            {
                return false;
            }   
        }

    }
}
