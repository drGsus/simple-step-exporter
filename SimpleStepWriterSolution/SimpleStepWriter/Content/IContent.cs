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
        void GetLines(int childIndex, in StringBuilder sb, in List<string> stepEntries);
    }
}
