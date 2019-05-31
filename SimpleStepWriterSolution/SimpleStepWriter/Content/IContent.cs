using SimpleStepWriter.Helper;

namespace SimpleStepWriter.Content
{
    internal interface IContent
    {
        IStepManager StepManager { get; }    
        long Guid { get; }
        string Name { get; }
        Vector3 Rotation { get; }
        Vector3 Center { get; }
        string[] GetLines(int childIndex);
    }
}
