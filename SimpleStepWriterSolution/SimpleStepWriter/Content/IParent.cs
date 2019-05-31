using System.Collections.Generic;

namespace SimpleStepWriter.Content
{
    internal interface IParent : IContent
    {
        List<IChild> Children { get; set; }
        long[] ChildrenStepId_AXIS2_PLACEMENT_3D { get; }
        long StepId_SHAPE_REPRESENTATION { get; }
        long StepId_PRODUCT_DEFINITION { get; }
    }
}
