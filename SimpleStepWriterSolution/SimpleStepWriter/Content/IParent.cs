using System.Collections.Generic;

namespace SimpleStepWriter.Content
{
    internal interface IParent : IContent
    {
        List<IChild> Children { get; set; }
    }
}
