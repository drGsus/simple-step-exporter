namespace SimpleStepWriter.Content
{
    internal interface IChild : IContent
    {
        IParent Parent { get; set; }
    }
}
