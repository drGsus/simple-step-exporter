using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.Text;

namespace SimpleStepWriter.Content.Internal
{
    /// <summary>
    /// Base assembly that we require for export. At this moment the library only supports a single assembly.
    /// </summary>
    internal class RootAssembly : IContent, IParent
    {
        // IContent implementation
        public IStepManager StepManager { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        // for root assembly rotation and position other than zero is not supported yet
        public Vector3 Rotation { get; } = Vector3.Zero;     
        public Vector3 Position { get; } = Vector3.Zero;

        // IParent implementation
        public List<IChild> Children { get; set; }
        public int[] ChildrenStepId_AXIS2_PLACEMENT_3D { get; private set; }
        public int StepId_SHAPE_REPRESENTATION { get; private set; }
        public int StepId_PRODUCT_DEFINITION { get; private set; }

        /// <summary>
        /// Create a new root assembly with parameters. At the moment we only support a single root assembly per STEP file.
        /// </summary>
        /// <param name="stepManager">Manager that keeps track of global values relevant for the entire STEP file.</param>
        /// <param name="name">Name of the assembly that is visible in the CAD hierarchy.</param>
        /// <param name="id">Uniquely identifies this IContent object in the entire assembly.</param>       
        public RootAssembly(IStepManager stepManager, string name, int id)
        {
            this.StepManager = stepManager;            
            this.Name = name;
            this.Id = id;
            this.Children = new List<IChild>();
        }

        /// <summary>
        /// Get the STEP lines.
        /// </summary>
        /// <param name="childIndex">Child index of this object based on parent. 0 at the moment cause we don't support multiple root assemblies.</param> 
        /// <param name="sb">StringBuilder instance used for creating STEP content. Has to be cleared when string was created.</param>
        /// <param name="stepEntries">Add your content to this list if it should be appended to the current STEP content.</param>
        public void GetLines(int childIndex, in StringBuilder sb, in List<string> stepEntries)
        {
            // lines defining the beginning of our root assembly
            sb.Append(@"#3 = SHAPE_DEFINITION_REPRESENTATION(#4,#10);");
            sb.AppendLine().Append(@"#4 = PRODUCT_DEFINITION_SHAPE('','',#5);");
            sb.AppendLine().Append(@"#5 = PRODUCT_DEFINITION('design','',#6,#9);");
            sb.AppendLine().Append(@"#6 = PRODUCT_DEFINITION_FORMATION('','',#7);");
            sb.AppendLine().Append(@"#7 = PRODUCT('" + Name + "','" + Name + "','',(#8));");
            sb.AppendLine().Append(@"#8 = PRODUCT_CONTEXT('',#2,'mechanical');");
            sb.AppendLine().Append(@"#9 = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');");                           
            
            StepId_PRODUCT_DEFINITION = 5;
            StepManager.NextId = 15;    

            // We generate a separate coordiante system for each child part.
            // In addition we have one global (root assembly dependent and mandatory) coordinate system (#10-#14).           
            
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

            StepId_SHAPE_REPRESENTATION = 10;

            // assembly coordiante system
            sb.AppendLine().Append(@"#10 = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + StepManager.NextId + ");");
            sb.AppendLine().Append(@"#11 = AXIS2_PLACEMENT_3D('',#12,#13,#14);");
            sb.AppendLine().Append(@"#12 = CARTESIAN_POINT('',(0.,0.,0.));");
            sb.AppendLine().Append(@"#13 = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append(@"#14 = DIRECTION('',(1.,0.,-0.));");            

            // now add prepared coordiante system for each child            
            foreach (var line in childrenCoordinateSystems)
            {
                sb.AppendLine().Append(line);
            }

            // scale information
            sb.AppendLine().Append(@"#" + (StepManager.NextId + 0) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 4) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 3) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );");
            sb.AppendLine().Append(@"#" + (StepManager.NextId + 1) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );");
            sb.AppendLine().Append(@"#" + (StepManager.NextId + 2) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );");
            sb.AppendLine().Append(@"#" + (StepManager.NextId + 3) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );");
            sb.AppendLine().Append(@"#" + (StepManager.NextId + 4) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 1) + ",'distance_accuracy_value','confusion accuracy');");
            sb.AppendLine().Append(@"#" + (StepManager.NextId + 5) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#7));");
            
            StepManager.NextId = (StepManager.NextId + 6);

            // let's add the created string to current STEP content
            stepEntries.Add(sb.ToString());
            sb.Clear();
        }

    }
}
