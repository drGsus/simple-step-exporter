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
            this.Center = Vector3.Zero;
            this.Rotation = Vector3.Zero;
        }

        /// <summary>
        /// Get all STEP lines required for root assembly.
        /// </summary>
        /// <param name="childrenCount">Amount of child boxes (solids).</param>
        /// <param name="childrenCoordinateSystemsIds">ID references to child coordinate systems.</param>
        /// <returns></returns>
        public string[] GetLines(Box[] boxes, out long[] childrenCoordinateSystemsIds)
        {
            // TODO: check references
            // lines defining the beginning of our root assembly
            string[] header = new[] {
                @"#" + (StepManager.NextId + 0) + " = SHAPE_DEFINITION_REPRESENTATION(#4,#10);",               // #3
                @"#" + (StepManager.NextId + 1) + " = PRODUCT_DEFINITION_SHAPE('','',#5);",
                @"#" + (StepManager.NextId + 2) + " = PRODUCT_DEFINITION('design','',#6,#9);",
                @"#" + (StepManager.NextId + 3) + " = PRODUCT_DEFINITION_FORMATION('','',#7);",
                @"#" + (StepManager.NextId + 4) + " = PRODUCT('" + Name + "','" + Name + "','',(#8));",
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#" + (StepManager.NextId + 6) + " = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');"
            };

            // We generate a separate coordiante system for each child.
            // In addition we have one global (root assembly dependent and mandatory) coordinate system (#10-#14).           

            //StepManager.NextId += 12;

            string[] childrenCoordinateSystems = new string[boxes.Length * 4];
            string transformRef = "";
            childrenCoordinateSystemsIds = new long[boxes.Length];
            for (int i = 0; i < boxes.Length; i++)
            {
                // see page 51 ff.: https://www.prostep.org/fileadmin/downloads/ProSTEP-iViP_Implementation-Guideline_PDM-Schema_4.3.pdf 
                Matrix3x3 rotationMatrix = Matrix3x3.EulerAnglesToMatrix3x3(boxes[i].Rotation);
                var z = new Vector3(rotationMatrix.A13, rotationMatrix.A23, rotationMatrix.A33);
                var a = new Vector3(rotationMatrix.A11, rotationMatrix.A21, rotationMatrix.A31);

                childrenCoordinateSystems[i * 4 + 0]
                    = @"#" + (StepManager.NextId + 0) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ");";     // #15
                childrenCoordinateSystems[i * 4 + 1]
                    = @"#" + (StepManager.NextId + 1) + " = CARTESIAN_POINT('',(" + boxes[i].Center.XString + "," + boxes[i].Center.YString + "," + boxes[i].Center.ZString + "));";
                childrenCoordinateSystems[i * 4 + 2]
                    = @"#" + (StepManager.NextId + 2) + " = DIRECTION('',(" + z.XString + "," + z.YString + "," + z.ZString + "));";
                childrenCoordinateSystems[i * 4 + 3]
                    = @"#" + (StepManager.NextId + 3) + " = DIRECTION('',(" + a.XString + "," + a.YString + "," + a.ZString + "));";

                childrenCoordinateSystemsIds[i] = StepManager.NextId;

                transformRef += ",#" + StepManager.NextId;
                StepManager.NextId += 4;
            }

            string[] assemblyCoordianteSystemLink = new[] {
                @"#" + (StepManager.NextId + 7) + " = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + (StepManager.NextId + 8) + ");"           // #10               
            };

            // add assembly footer lines
            string[] scale = new[] {
                @"#" + (StepManager.NextId + 8) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 12) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 9) + ",#" + (StepManager.NextId + 10) + ",#" + (StepManager.NextId + 11) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );",       //  legacy #23
                @"#" + (StepManager.NextId + 9) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (StepManager.NextId + 10) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (StepManager.NextId + 11) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (StepManager.NextId + 12) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 9) + ",'distance_accuracy_value','confusion accuracy');"
                //@"#" + (StepManager.NextId + 13) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#7));"
            };

            // TODO: search and link references
            string[] hierarchy = new[] {
                @"#" + (StepManager.NextId + 13) + " = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#" + (StepManager.NextId + 14) + ",#" + (StepManager.NextId + 16) + ");",       // #794
                @"#" + (StepManager.NextId + 14) + " = ( REPRESENTATION_RELATIONSHIP('','',#36,#10) REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#" + (StepManager.NextId + 15) + ") SHAPE_REPRESENTATION_RELATIONSHIP() );",     // assembly to #10 (root)  (SHAPE_REPRESENTATION to SHAPE_REPRESENTATION)
                @"#" + (StepManager.NextId + 15) + " = ITEM_DEFINED_TRANSFORMATION('','',#11,#15);",                                                                                                                                 // #11 (root) to root (assembly transform defined in root)
                @"#" + (StepManager.NextId + 16) + " = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#" + (StepManager.NextId + 17) + ");",
                @"#" + (StepManager.NextId + 17) + " = NEXT_ASSEMBLY_USAGE_OCCURRENCE('62','=>[0:1:1:5]','',#5,#31,$);",                                                                                                             // #5 (root) to assembly  (PRODUCT_DEFINITION to PRODUCT_DEFINITION)
                @"#" + (StepManager.NextId + 18) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#33));"                                                                                                                            // assembly PRODUCT     
            };

            //StepManager.NextId = (StepManager.NextId + 6);

            return (header.Concat(assemblyCoordianteSystemLink).Concat(childrenCoordinateSystems).Concat(scale)).ToArray();
        }

    }

}
