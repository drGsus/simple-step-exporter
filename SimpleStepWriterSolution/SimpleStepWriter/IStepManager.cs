namespace SimpleStepWriter
{
    internal interface IStepManager
    {
        int NextId { get; set; }        
        int ObjectIndex { get; set; }
    }
}
