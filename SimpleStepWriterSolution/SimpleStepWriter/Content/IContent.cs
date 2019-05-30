namespace SimpleStepWriter.Content
{
    internal interface IContent
    {
        IStepManager StepManager { get; }    
        long Guid { get; }
        string Name { get; }        
    }
}
