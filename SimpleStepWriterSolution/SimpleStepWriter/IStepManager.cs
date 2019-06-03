namespace SimpleStepWriter
{
    /// <summary>
    /// Manager that keeps track of global values relevant for the entire STEP file.
    /// </summary>
    internal interface IStepManager
    {
        int NextId { get; set; }        
        int ObjectIndex { get; set; }
    }
}
