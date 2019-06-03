using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.Text;

namespace SimpleStepWriter.Content
{
    /// <summary>
    /// Interface that needs to be implemented by every class that represents a part in the STEP hierarchy.
    /// </summary>
    internal interface IContent
    {
        IStepManager StepManager { get; }
        int Id { get; }
        string Name { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }

        /// <summary>
        /// Get the STEP lines.
        /// </summary>
        /// <param name="childIndex">Child index of this object based on parent.</param>
        /// <param name="sb">StringBuilder instance used for creating STEP content. Has to be cleared when string was created.</param>
        /// <param name="stepEntries">Add your content to this list if it should be appended to the current STEP content.</param>
        void GetLines(int childIndex, in StringBuilder sb, in List<string> stepEntries);
    }
}
