namespace SimpleStepWriter
{
    /// <summary>
    /// Manager that keeps track of global values relevant for the entire STEP file.
    /// </summary>
    internal sealed class StepManager : IStepManager
    {
        public int NextId { get; set; }
        public int ObjectIndex { get; set; }
    }
}
