namespace SimpleStepWriter
{
    /// <summary>
    /// Issues values that are relvant in a whole STEP file context.
    /// </summary>
    internal sealed class StepManager : IStepManager
    {
        public int NextId { get; set; }
        public int ObjectIndex { get; set; }
    }
}
