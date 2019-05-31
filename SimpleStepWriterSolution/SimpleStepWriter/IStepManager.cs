namespace SimpleStepWriter
{
    internal interface IStepManager
    {
        long NextId { get; set; }
        long ObjectIndex { get; set; }
    }
}
