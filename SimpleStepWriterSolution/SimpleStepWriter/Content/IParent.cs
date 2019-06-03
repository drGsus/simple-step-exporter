using System.Collections.Generic;

namespace SimpleStepWriter.Content
{
    /// <summary>
    /// Interface that needs to be implemented by every class that represents a part in the STEP hierarchy
    /// and has the ability to be a parent of other parts.
    /// </summary>
    internal interface IParent : IContent
    {
        List<IChild> Children { get; set; }

        /// <summary>
        /// STEP id references needed by child objects.
        /// </summary>
        int[] ChildrenStepId_AXIS2_PLACEMENT_3D { get; }
        int StepId_SHAPE_REPRESENTATION { get; }
        int StepId_PRODUCT_DEFINITION { get; }
    }
}
