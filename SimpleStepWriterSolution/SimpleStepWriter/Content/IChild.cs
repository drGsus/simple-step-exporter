namespace SimpleStepWriter.Content
{
    /// <summary>
    /// Interface that needs to be implemented by every class that represents a part in the STEP hierarchy
    /// and has the ability to be a child of another part.
    /// </summary>
    internal interface IChild : IContent
    {
        IParent Parent { get; set; }
    }
}
