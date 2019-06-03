using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.Text;

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
        /// Get the STEP lines.
        /// </summary>
        /// <param name="childIndex">Child index of this object based on parent.</param>
        /// <param name="sb">StringBuilder instance used for creating STEP content. Has to be cleared when string was created.</param>
        /// <param name="stepEntries">Add your content to this list if it should be appended to the current STEP content.</param>
        public void GetLines(int childIndex, in StringBuilder sb, in List<string> stepEntries)
        {
            // header
            sb.AppendLine().Append("#").Append(StepManager.NextId + 0).Append(" = SHAPE_DEFINITION_REPRESENTATION(#").Append(StepManager.NextId + 1).Append(",#").Append(StepManager.NextId + 7).Append(");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 1).Append(" = PRODUCT_DEFINITION_SHAPE('','',#").Append(StepManager.NextId + 2).Append(");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 2).Append(" = PRODUCT_DEFINITION('design','',#").Append(StepManager.NextId + 3).Append(",#").Append(StepManager.NextId + 6).Append(");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 3).Append(" = PRODUCT_DEFINITION_FORMATION('','',#").Append(StepManager.NextId + 4).Append(");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 4).Append(" = PRODUCT('" + Name + "','" + Name + "','',(#").Append(StepManager.NextId + 5).Append("));");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 5).Append(" = PRODUCT_CONTEXT('',#2,'mechanical');");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 6).Append(" = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');");

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

            // shapeRepresentation
            sb.AppendLine(@"#" + StepId_SHAPE_REPRESENTATION + " = SHAPE_REPRESENTATION('',(#11" + transformRef + "),#" + StepManager.NextId + ");");

            // now add prepared coordinate system for each child            
            foreach(var line in childrenCoordinateSystems)
            {
                sb.AppendLine(line);
            }

            // scale information         
            sb.AppendLine().Append("#").Append(StepManager.NextId + 0).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#").Append(StepManager.NextId + 4).Append(")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#").Append(StepManager.NextId + 1).Append(",#").Append(StepManager.NextId + 2).Append(",#").Append(StepManager.NextId + 3).Append(")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 1).Append(" = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 2).Append(" = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 3).Append(" = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 4).Append(" = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#").Append(StepManager.NextId + 1).Append(",'distance_accuracy_value','confusion accuracy');");

            StepManager.NextId = (StepManager.NextId + 5);

            // hierarchy information (this part to parent relationship)            
            sb.AppendLine().Append("#").Append(StepManager.NextId + 0).Append(" = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#").Append(StepManager.NextId + 1).Append(",#").Append(StepManager.NextId + 3).Append(");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 1).Append(" = ( REPRESENTATION_RELATIONSHIP('','',#" + StepId_SHAPE_REPRESENTATION + ",#" + Parent.StepId_SHAPE_REPRESENTATION + ") REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#").Append(StepManager.NextId + 2).Append(") SHAPE_REPRESENTATION_RELATIONSHIP() );");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 2).Append(" = ITEM_DEFINED_TRANSFORMATION('','',#11,#" + Parent.ChildrenStepId_AXIS2_PLACEMENT_3D[childIndex] + ");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 3).Append(" = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#").Append(StepManager.NextId + 4).Append(");");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 4).Append(" = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + (StepManager.ObjectIndex + 0) + "','=>[0:1:1:" + (StepManager.ObjectIndex + 0) + "]','',#" + Parent.StepId_PRODUCT_DEFINITION + ",#" + StepId_PRODUCT_DEFINITION + ",$);");
            sb.AppendLine().Append("#").Append(StepManager.NextId + 5).Append(" = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#" + stepId_PRODUCT + "));");

            StepManager.ObjectIndex += 1;
            StepManager.NextId = (StepManager.NextId + 6);

            // let's add the created string to current STEP content
            stepEntries.Add(sb.ToString());
            sb.Clear();
        }

    }

}
