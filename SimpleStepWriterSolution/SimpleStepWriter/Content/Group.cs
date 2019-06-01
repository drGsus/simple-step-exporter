using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStepWriter.Content
{
    /// <summary>
    /// Group class representing a part in the CAD hierarchy without any representation attached.
    /// It is used for grouping other Boxes and/or Groups.
    /// </summary>
    class Group : IContent, IParent, IChild
    {
        // IContent implementation
        public IStepManager StepManager { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; } = "DefaultGroupName";
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }

        // IChild implementation        
        public IParent Parent { get; set; }

        //  IParent implementation
        public List<IChild> Children { get; set; }
        public int[] ChildrenStepId_AXIS2_PLACEMENT_3D { get; private set; }
        public int StepId_SHAPE_REPRESENTATION { get; private set; }
        public int StepId_PRODUCT_DEFINITION { get; private set; }

        /// <summary>
        /// Create a new group with parameters.
        /// </summary>
        /// <param name="stepManager">Manager that keeps track of global values relevant for the entire STEP file</param>
        /// <param name="name">Name of the group that is visible in the CAD hierarchy.</param>     
        /// <param name="position">Position of the group relative to parent.</param>       
        /// <param name="rotation">Group rotation relative to parent (around the provided position).</param>     
        /// <param name="id">Uniquely identifies this IContent object in the entire assembly.</param>             
        public Group(IStepManager stepManager, string name, Vector3 position, Vector3 rotation, int id)
        {
            this.StepManager = stepManager;
            this.Id = id;            
            this.Name = name;
            this.Position = position; 
            this.Rotation = rotation;
            this.Children = new List<IChild>();
        }

        /// <summary>
        /// Get the STEP lines dependent on the appropriate StepFile object.
        /// </summary>
        /// <param name="childIndex">Child index of this object based on parent. 
        /// </param>     
        /// <returns>The text we append to the STEP file.</returns>
        public string[] GetLines(int childIndex)
        {
            string[] header = new[] {
                @"#" + (StepManager.NextId + 0) + " = SHAPE_DEFINITION_REPRESENTATION(#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 7) + ");",       
                @"#" + (StepManager.NextId + 1) + " = PRODUCT_DEFINITION_SHAPE('','',#" + (StepManager.NextId + 2) + ");",
                @"#" + (StepManager.NextId + 2) + " = PRODUCT_DEFINITION('design','',#" + (StepManager.NextId + 3) + ",#" + (StepManager.NextId + 6) + ");",
                @"#" + (StepManager.NextId + 3) + " = PRODUCT_DEFINITION_FORMATION('','',#" + (StepManager.NextId + 4) + ");",
                @"#" + (StepManager.NextId + 4) + " = PRODUCT('" + Name + "','" + Name + "','',(#" + (StepManager.NextId + 5) + "));",
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#" + (StepManager.NextId + 6) + " = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');"         
            };

            int stepId_PRODUCT = (StepManager.NextId + 4);
            StepId_PRODUCT_DEFINITION = (StepManager.NextId + 2);
            StepId_SHAPE_REPRESENTATION = (StepManager.NextId + 7);
            StepManager.NextId += 8;

            // We generate a separate coordiante system for each child part.                    

            string[] childrenCoordinateSystems = new string[Children.Count * 4];
            string transformRef = "";
            ChildrenStepId_AXIS2_PLACEMENT_3D = new int[Children.Count];
            for (int i = 0; i < Children.Count; i++)
            {
                // see page 51 ff.: https://www.prostep.org/fileadmin/downloads/ProSTEP-iViP_Implementation-Guideline_PDM-Schema_4.3.pdf 
                Matrix3x3 rotationMatrix = Matrix3x3.EulerAnglesToMatrix3x3(Children[i].Rotation);
                var z = new Vector3(rotationMatrix.A13, rotationMatrix.A23, rotationMatrix.A33);
                var a = new Vector3(rotationMatrix.A11, rotationMatrix.A21, rotationMatrix.A31);

                childrenCoordinateSystems[i * 4 + 0]
                    = @"#" + (StepManager.NextId + 0) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ");";    
                childrenCoordinateSystems[i * 4 + 1]
                    = @"#" + (StepManager.NextId + 1) + " = CARTESIAN_POINT('',(" + Children[i].Position.XString + "," + Children[i].Position.YString + "," + Children[i].Position.ZString + "));";
                childrenCoordinateSystems[i * 4 + 2]
                    = @"#" + (StepManager.NextId + 2) + " = DIRECTION('',(" + z.XString + "," + z.YString + "," + z.ZString + "));";
                childrenCoordinateSystems[i * 4 + 3]
                    = @"#" + (StepManager.NextId + 3) + " = DIRECTION('',(" + a.XString + "," + a.YString + "," + a.ZString + "));";
                
                ChildrenStepId_AXIS2_PLACEMENT_3D[i] = StepManager.NextId;

                transformRef += ",#" + StepManager.NextId;
                StepManager.NextId += 4;
            }

            string[] shapeRepresentation = new[] {
                @"#" + StepId_SHAPE_REPRESENTATION + " = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + StepManager.NextId + ");",               
            };

            string[] scaleInformation = new[] {
                @"#" + (StepManager.NextId + 0) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 4) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );", 
                @"#" + (StepManager.NextId + 1) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (StepManager.NextId + 2) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (StepManager.NextId + 3) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (StepManager.NextId + 4) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 1) + ",'distance_accuracy_value','confusion accuracy');"                
            };

            StepManager.NextId = (StepManager.NextId + 5);
            
            // this part to parent
            var hierarchy = new[]
            {
                @"#" + (StepManager.NextId + 0) + " = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 3) + ");",  
                @"#" + (StepManager.NextId + 1) + " = ( REPRESENTATION_RELATIONSHIP('','',#" + StepId_SHAPE_REPRESENTATION + ",#" + Parent.StepId_SHAPE_REPRESENTATION + ") REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#" + (StepManager.NextId + 2) + ") SHAPE_REPRESENTATION_RELATIONSHIP() );",     
                @"#" + (StepManager.NextId + 2) + " = ITEM_DEFINED_TRANSFORMATION('','',#11,#" + Parent.ChildrenStepId_AXIS2_PLACEMENT_3D[childIndex] + ");",
                @"#" + (StepManager.NextId + 3) + " = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#" + (StepManager.NextId + 4) + ");",
                @"#" + (StepManager.NextId + 4) + " = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + (StepManager.ObjectIndex + 0) + "','=>[0:1:1:" + (StepManager.ObjectIndex + 0) + "]','',#" + Parent.StepId_PRODUCT_DEFINITION + ",#" + StepId_PRODUCT_DEFINITION + ",$);",
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#" + stepId_PRODUCT + "));"
            };

            StepManager.ObjectIndex += 1;
            StepManager.NextId = (StepManager.NextId + 6);
            
            return (header.Concat(shapeRepresentation).Concat(childrenCoordinateSystems).Concat(scaleInformation).Concat(hierarchy)).ToArray();
        }

    }

}
