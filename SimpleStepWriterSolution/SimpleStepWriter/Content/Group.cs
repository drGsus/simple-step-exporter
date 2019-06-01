using SimpleStepWriter.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleStepWriter.Content
{
    /// <summary>
    /// Group class representing a part in the CAD hierarchy without any representation attached.
    /// It is used for grouping other Boxes or Groups.
    /// </summary>
    class Group : IContent, IParent, IChild
    {
        // IContent implementation
        public IStepManager StepManager { get; private set; }
        public long Guid { get; private set; }

        // IChild implementation        
        public IParent Parent { get; set; }

        //  IParent implementation
        public List<IChild> Children { get; set; }
        public long[] ChildrenStepId_AXIS2_PLACEMENT_3D { get; private set; }
        public long StepId_SHAPE_REPRESENTATION { get; private set; }
        public long StepId_PRODUCT_DEFINITION { get; private set; }

        // user provided values
        public string Name { get; private set; } = "DefaultGroupName";
        public Vector3 Center { get; private set; }       
        public Vector3 Rotation { get; private set; }

       

        /// <summary>
        /// Initialize the object with user defined values.
        /// </summary>
        /// <param name="stepManager">Manager that keeps track of global values relevant for the entire STEP file</param>
        /// <param name="name">Group name that is displayed in the CAD hierarchy.</param>
        /// <param name="center">Base point in world space of this group and all its children. - CURRENTLY NOT SUPPORTED</param>
        /// <param name="rotation">Base rotation of this group and all its children. - CURRENTLY NOT SUPPORTED</param>
        /// <param name="guid">Uniquely identifies this IContent object in the entire assembly.</param>       
        public Group(IStepManager stepManager, string name, Vector3 center, Vector3 rotation, long guid)
        {
            this.StepManager = stepManager;
            this.Name = name;
            this.Guid = guid;            
            this.Children = new List<IChild>();

            // custom transforms for groups are currently not supported, everything is based on the default transform
            this.Center = center; //Vector3.Zero;
            this.Rotation = rotation; // Vector3.Zero;
        }

        public string[] GetLines(int childIndex)
        {
            // lines defining the beginning of our root assembly
            string[] header = new[] {
                @"#" + (StepManager.NextId + 0) + " = SHAPE_DEFINITION_REPRESENTATION(#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 7) + ");",        // #3
                @"#" + (StepManager.NextId + 1) + " = PRODUCT_DEFINITION_SHAPE('','',#" + (StepManager.NextId + 2) + ");",
                @"#" + (StepManager.NextId + 2) + " = PRODUCT_DEFINITION('design','',#" + (StepManager.NextId + 3) + ",#" + (StepManager.NextId + 6) + ");",
                @"#" + (StepManager.NextId + 3) + " = PRODUCT_DEFINITION_FORMATION('','',#" + (StepManager.NextId + 4) + ");",
                @"#" + (StepManager.NextId + 4) + " = PRODUCT('" + Name + "','" + Name + "','',(#" + (StepManager.NextId + 5) + "));",
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#" + (StepManager.NextId + 6) + " = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');"           // #9
            };

            long stepId_PRODUCT = (StepManager.NextId + 4);

            StepId_PRODUCT_DEFINITION = (StepManager.NextId + 2);

            // We generate a separate coordiante system for each child box.
            // In addition we have one global (root assembly dependent and mandatory) coordinate system (#10-#14).           

            StepId_SHAPE_REPRESENTATION = (StepManager.NextId + 7);

            StepManager.NextId += 8;

            string[] childrenCoordinateSystems = new string[Children.Count * 4];
            string transformRef = "";
            ChildrenStepId_AXIS2_PLACEMENT_3D = new long[Children.Count];
            for (int i = 0; i < Children.Count; i++)
            {
                // see page 51 ff.: https://www.prostep.org/fileadmin/downloads/ProSTEP-iViP_Implementation-Guideline_PDM-Schema_4.3.pdf 
                Matrix3x3 rotationMatrix = Matrix3x3.EulerAnglesToMatrix3x3(Children[i].Rotation);
                var z = new Vector3(rotationMatrix.A13, rotationMatrix.A23, rotationMatrix.A33);
                var a = new Vector3(rotationMatrix.A11, rotationMatrix.A21, rotationMatrix.A31);

                childrenCoordinateSystems[i * 4 + 0]
                    = @"#" + (StepManager.NextId + 0) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ");";     // #15
                childrenCoordinateSystems[i * 4 + 1]
                    = @"#" + (StepManager.NextId + 1) + " = CARTESIAN_POINT('',(" + Children[i].Center.XString + "," + Children[i].Center.YString + "," + Children[i].Center.ZString + "));";
                childrenCoordinateSystems[i * 4 + 2]
                    = @"#" + (StepManager.NextId + 2) + " = DIRECTION('',(" + z.XString + "," + z.YString + "," + z.ZString + "));";
                childrenCoordinateSystems[i * 4 + 3]
                    = @"#" + (StepManager.NextId + 3) + " = DIRECTION('',(" + a.XString + "," + a.YString + "," + a.ZString + "));";
                
                ChildrenStepId_AXIS2_PLACEMENT_3D[i] = StepManager.NextId;

                transformRef += ",#" + StepManager.NextId;
                StepManager.NextId += 4;
            }

            string[] assemblyCoordianteSystem = new[] {
                @"#" + StepId_SHAPE_REPRESENTATION + " = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + StepManager.NextId + ");",               
            };

            // add assembly footer lines
            string[] footer = new[] {
                @"#" + (StepManager.NextId + 0) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 4) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );",       //  legacy #23
                @"#" + (StepManager.NextId + 1) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (StepManager.NextId + 2) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (StepManager.NextId + 3) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (StepManager.NextId + 4) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 1) + ",'distance_accuracy_value','confusion accuracy');"                
            };

            StepManager.NextId = (StepManager.NextId + 5);
            
            var parentHierarchy = new[]
            {
                @"#" + (StepManager.NextId + 0) + " = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 3) + ");",   // #788
                @"#" + (StepManager.NextId + 1) + " = ( REPRESENTATION_RELATIONSHIP('','',#" + StepId_SHAPE_REPRESENTATION + ",#" + Parent.StepId_SHAPE_REPRESENTATION + ") REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#" + (StepManager.NextId + 2) + ") SHAPE_REPRESENTATION_RELATIONSHIP() );",  // box2 to assembly (SHAPE_REPRESENTATION to SHAPE_REPRESENTATION)         
                @"#" + (StepManager.NextId + 2) + " = ITEM_DEFINED_TRANSFORMATION('','',#11,#" + Parent.ChildrenStepId_AXIS2_PLACEMENT_3D[childIndex] + ");",  // #11 (root) to assembly (box2 transfoprm defined in assembly) (AXIS2_PLACEMENT_3D to AXIS2_PLACEMENT_3D)
                @"#" + (StepManager.NextId + 3) + " = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#" + (StepManager.NextId + 4) + ");",
                @"#" + (StepManager.NextId + 4) + " = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + (StepManager.ObjectIndex + 0) + "','=>[0:1:1:" + (StepManager.ObjectIndex + 0) + "]','',#" + Parent.StepId_PRODUCT_DEFINITION + ",#" + StepId_PRODUCT_DEFINITION + ",$);",     // assembly to box2  (PRODUCT_DEFINITION to PRODUCT_DEFINITION)
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#" + stepId_PRODUCT + "));"   // box2 PRODUCT
            };

            StepManager.ObjectIndex += 1;
            StepManager.NextId = (StepManager.NextId + 6);

            return (header.Concat(assemblyCoordianteSystem).Concat(childrenCoordinateSystems).Concat(footer).Concat(parentHierarchy)).ToArray();
        }

    }

}
