using System.Linq;

namespace SimpleStepWriter.Content.Internal
{
    /// <summary>
    /// Base assembly that we require for export.
    /// </summary>
    internal class RootAssembly
    {
        private IStepManager stepManager;
        private string assemblyName;
        
        internal RootAssembly(IStepManager stepManager, string name)
        {
            this.stepManager = stepManager;
            this.assemblyName = name;
        }

        /// <summary>
        /// Get all STEP lines required for root assembly.
        /// </summary>
        /// <param name="childrenCount">Amount of child boxes (solids).</param>
        /// <param name="childrenCoordinateSystemsIds">ID references to child coordinate systems.</param>
        /// <returns></returns>
        internal string[] GetLines(long childrenCount, out long[] childrenCoordinateSystemsIds)
        {            
            // lines defining the beginning of our root assembly
            string[] header = new[] {
                @"#3 = SHAPE_DEFINITION_REPRESENTATION(#4,#10);",                  
                @"#4 = PRODUCT_DEFINITION_SHAPE('','',#5);",
                @"#5 = PRODUCT_DEFINITION('design','',#6,#9);",
                @"#6 = PRODUCT_DEFINITION_FORMATION('','',#7);",
                @"#7 = PRODUCT('" + assemblyName + "','" + assemblyName + "','',(#8));",
                @"#8 = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#9 = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');"                            
            };

            // We generate a separate coordiante system for each child box.
            // In addition we have one global (root assembly dependent and mandatory) coordinate system (#10-#14).           
                      
            stepManager.NextId = 15;    

            string[] childrenCoordinateSystems = new string[childrenCount * 4];              
            string transformRef = "";
            childrenCoordinateSystemsIds = new long[childrenCount];
            for (int i = 0; i < childrenCount; i++)
            {
                childrenCoordinateSystems[i * 4 + 0]
                    = @"#" + (stepManager.NextId + 0) + " = AXIS2_PLACEMENT_3D('',#" + (stepManager.NextId + 1) + ",#" + (stepManager.NextId + 2) + ",#" + (stepManager.NextId + 3) + ");";     // #15
                childrenCoordinateSystems[i * 4 + 1]
                    = @"#" + (stepManager.NextId + 1) + " = CARTESIAN_POINT('',(0.,0.,0.));";
                childrenCoordinateSystems[i * 4 + 2]
                    = @"#" + (stepManager.NextId + 2) + " = DIRECTION('',(0.,0.,1.));";
                childrenCoordinateSystems[i * 4 + 3]
                    = @"#" + (stepManager.NextId + 3) + " = DIRECTION('',(1.,0.,0.));";

                childrenCoordinateSystemsIds[i] = stepManager.NextId;

                transformRef += ",#" + stepManager.NextId;
                stepManager.NextId += 4;
            }          
                        
            string[] assemblyCoordianteSystem = new[] {               
                @"#10 = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + stepManager.NextId + ");",
                @"#11 = AXIS2_PLACEMENT_3D('',#12,#13,#14);",
                @"#12 = CARTESIAN_POINT('',(0.,0.,0.));",
                @"#13 = DIRECTION('',(0.,0.,1.));",
                @"#14 = DIRECTION('',(1.,0.,-0.));",
            };
            
            // add assembly footer lines
            string[] footer = new[] {                
                @"#" + (stepManager.NextId + 0) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (stepManager.NextId + 4) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (stepManager.NextId + 1) + ",#" + (stepManager.NextId + 2) + ",#" + (stepManager.NextId + 3) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );",       //  legacy #23
                @"#" + (stepManager.NextId + 1) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (stepManager.NextId + 2) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (stepManager.NextId + 3) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (stepManager.NextId + 4) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (stepManager.NextId + 1) + ",'distance_accuracy_value','confusion accuracy');",
                @"#" + (stepManager.NextId + 5) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#7));"
            };

            stepManager.NextId = (stepManager.NextId + 6);
            
            return (header.Concat(assemblyCoordianteSystem).Concat(childrenCoordinateSystems).Concat(footer)).ToArray();
        }

    }
}
