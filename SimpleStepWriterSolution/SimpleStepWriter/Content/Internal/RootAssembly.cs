using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStepWriter.Content.Internal
{
    /// <summary>
    /// Base assembly that we require for export.
    /// </summary>
    internal class RootAssembly : IContent, IParent
    {
        public IStepManager StepManager { get; private set; }
        public long Guid { get; private set; }

        public List<IChild> Children { get; set; }
        public string Name { get; private set; }
                
        public RootAssembly(IStepManager stepManager, string name, long guid)
        {
            this.StepManager = stepManager;            
            this.Name = name;
            this.Guid = guid;
            this.Children = new List<IChild>();

        }

        /// <summary>
        /// Get all STEP lines required for root assembly.
        /// </summary>
        /// <param name="childrenCount">Amount of child boxes (solids).</param>
        /// <param name="childrenCoordinateSystemsIds">ID references to child coordinate systems.</param>
        /// <returns></returns>
        public string[] GetLines(Box[] boxes, out long[] childrenCoordinateSystemsIds)
        {            
            // lines defining the beginning of our root assembly
            string[] header = new[] {
                @"#3 = SHAPE_DEFINITION_REPRESENTATION(#4,#10);",                  
                @"#4 = PRODUCT_DEFINITION_SHAPE('','',#5);",
                @"#5 = PRODUCT_DEFINITION('design','',#6,#9);",
                @"#6 = PRODUCT_DEFINITION_FORMATION('','',#7);",
                @"#7 = PRODUCT('" + Name + "','" + Name + "','',(#8));",
                @"#8 = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#9 = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');"                            
            };

            // We generate a separate coordiante system for each child box.
            // In addition we have one global (root assembly dependent and mandatory) coordinate system (#10-#14).           
                      
            StepManager.NextId = 15;    

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
                        
            string[] assemblyCoordianteSystem = new[] {               
                @"#10 = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + StepManager.NextId + ");",
                @"#11 = AXIS2_PLACEMENT_3D('',#12,#13,#14);",
                @"#12 = CARTESIAN_POINT('',(0.,0.,0.));",
                @"#13 = DIRECTION('',(0.,0.,1.));",
                @"#14 = DIRECTION('',(1.,0.,-0.));",
            };
            
            // add assembly footer lines
            string[] footer = new[] {                
                @"#" + (StepManager.NextId + 0) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 4) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );",       //  legacy #23
                @"#" + (StepManager.NextId + 1) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (StepManager.NextId + 2) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (StepManager.NextId + 3) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (StepManager.NextId + 4) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 1) + ",'distance_accuracy_value','confusion accuracy');",
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#7));"
            };

            StepManager.NextId = (StepManager.NextId + 6);
            
            return (header.Concat(assemblyCoordianteSystem).Concat(childrenCoordinateSystems).Concat(footer)).ToArray();
        }

    }
}
