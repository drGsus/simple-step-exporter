using SimpleStepWriter.Helper;
using System.Collections.Generic;
using System.Text;

namespace SimpleStepWriter.Content
{
    /// <summary> Box class representing a part in the CAD hierarchy with a simple solid in box form that has a size, position, rotation, color and name.
    /// 
    ///   
    ///          E---------H
    ///         /|        /|
    ///        / |       / |
    ///       F---------G  |
    ///       |   |     |  |
    ///       |  A------|- D
    ///       | /       | /
    ///       |/        |/
    ///       B---------C
    ///
    ///     Box point Setup
    ///
    ///
    /// 
    ///          Y                              
    ///          |      
    ///          |
    ///          |     
    ///          0------- X
    ///         /     
    ///        /     
    ///       Z
    ///       
    ///     Right-handed coordinate system (Y up) used for solid creation
    ///     
    /// 
    /// </summary>
    internal class Box : IContent, IChild
    {
        // IContent implementation
        public IStepManager StepManager { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; } = "DefaultBoxName";
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }

        // IChild implementation  
        public IParent Parent { get; set; }

        // additional user provided values
        public Vector3 Scale { get; private set; }
        public Color Color { get; private set; }

        // internally calculated box points
        private Vector3 pointA;
        private Vector3 pointB;
        private Vector3 pointC;
        private Vector3 pointD;
        private Vector3 pointE;
        private Vector3 pointF;
        private Vector3 pointG;
        private Vector3 pointH;

        /// <summary>
        /// Create a new box with parameters.
        /// </summary>
        /// <param name="stepManager">Manager that keeps track of global values relevant for the entire STEP file</param>
        /// <param name="name">Name of the box that is visible in the CAD hierarchy.</param>     
        /// <param name="position">Position of the box relative to parent. Also the center of the box.</param>
        /// <param name="dimension">Box dimension (it's the complete length of an edge, not half of it)</param>
        /// <param name="rotation">Box rotation relative to parent (around the provided position).</param>
        /// <param name="color">Box color. Transparency not supported yet.</param>
        /// <param name="id">Uniquely identifies this IContent object in the entire assembly.</param>       
        public Box(IStepManager stepManager, string name, Vector3 position, Vector3 dimension, Vector3 rotation, Color color, int id)
        {
            this.StepManager = stepManager;
            this.Id = id;
            this.Name = name;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = dimension;
            this.Color = color;

            // calculate all vertices of the box
            pointA = new Vector3(-(Scale.X / 2), -(Scale.Y / 2), -(Scale.Z / 2));
            pointB = new Vector3(-(Scale.X / 2), -(Scale.Y / 2), (Scale.Z / 2));
            pointC = new Vector3((Scale.X / 2), -(Scale.Y / 2), (Scale.Z / 2));
            pointD = new Vector3((Scale.X / 2), -(Scale.Y / 2), -(Scale.Z / 2));
            pointE = new Vector3(-(Scale.X / 2), (Scale.Y / 2), -(Scale.Z / 2));
            pointF = new Vector3(-(Scale.X / 2), (Scale.Y / 2), (Scale.Z / 2));
            pointG = new Vector3((Scale.X / 2), (Scale.Y / 2), (Scale.Z / 2));
            pointH = new Vector3((Scale.X / 2), (Scale.Y / 2), -(Scale.Z / 2));
        }

        /// <summary>
        /// Get the STEP lines dependent on the appropriate StepFile object.
        /// </summary>
        /// <param name="childIndex">Child index of this object based on parent. 
        /// </param>     
        /// <returns>The text we append to the STEP file.</returns>
        public void GetLines(int childIndex, in StringBuilder sb, in List<string> stepEntries)
        {
            int nextId = StepManager.NextId;

#region PART DEFINITION
            sb.AppendLine().Append("#").Append(nextId + 0).Append(" = SHAPE_DEFINITION_REPRESENTATION(#").Append(nextId + 1).Append(",#").Append(nextId + 7).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 1).Append(" = PRODUCT_DEFINITION_SHAPE('','',#").Append(nextId + 2).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 2).Append(" = PRODUCT_DEFINITION('design','',#").Append(nextId + 3).Append(",#").Append(nextId + 6).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 3).Append(" = PRODUCT_DEFINITION_FORMATION('','',#").Append(nextId + 4).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 4).Append(" = PRODUCT('" + Name + "','" + Name + "','',(#").Append(nextId + 5).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 5).Append(" = PRODUCT_CONTEXT('',#2,'mechanical');");
            sb.AppendLine().Append("#").Append(nextId + 6).Append(" = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');");
            sb.AppendLine().Append("#").Append(nextId + 7).Append(" = SHAPE_REPRESENTATION('',(#11,#").Append(nextId + 8).Append("),#").Append(nextId + 12).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 8).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 9).Append(",#").Append(nextId + 10).Append(",#").Append(nextId + 11).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 9).Append(" = CARTESIAN_POINT('',(0.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 10).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 11).Append(" = DIRECTION('',(1.,0.,0.));");
#endregion
#region PART SCALE
            sb.AppendLine().Append("#").Append(nextId + 12).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#").Append(nextId + 16).Append(")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#").Append(nextId + 13).Append(",#").Append(nextId + 14).Append(",#").Append(nextId + 15).Append(")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') );");
            sb.AppendLine().Append("#").Append(nextId + 13).Append(" = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );");
            sb.AppendLine().Append("#").Append(nextId + 14).Append(" = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );");
            sb.AppendLine().Append("#").Append(nextId + 15).Append(" = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );");
            sb.AppendLine().Append("#").Append(nextId + 16).Append(" = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#").Append(nextId + 13).Append(",'distance_accuracy_value','confusion accuracy');");
#endregion
#region GEOMETRY
            sb.AppendLine().Append("#").Append(nextId + 17).Append(" = ADVANCED_BREP_SHAPE_REPRESENTATION('',(#11,#").Append(nextId + 18).Append("),#").Append(nextId + 348).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 18).Append(" = MANIFOLD_SOLID_BREP('',#").Append(nextId + 19).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 19).Append(" = CLOSED_SHELL('',(#").Append(nextId + 20).Append(",#").Append(nextId + 140).Append(",#").Append(nextId + 240).Append(",#").Append(nextId + 287).Append(",#").Append(nextId + 334).Append(",#").Append(nextId + 341).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 20).Append(" = ADVANCED_FACE('',(#").Append(nextId + 21).Append("),#").Append(nextId + 35).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 21).Append(" = FACE_BOUND('',#").Append(nextId + 22).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 22).Append(" = EDGE_LOOP('',(#").Append(nextId + 23).Append(",#").Append(nextId + 58).Append(",#").Append(nextId + 86).Append(",#").Append(nextId + 114).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 23).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 24).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 24).Append(" = EDGE_CURVE('',#").Append(nextId + 25).Append(",#").Append(nextId + 27).Append(",#").Append(nextId + 29).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 25).Append(" = VERTEX_POINT('',#").Append(nextId + 26).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 26).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 27).Append(" = VERTEX_POINT('',#").Append(nextId + 28).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 28).Append(" = CARTESIAN_POINT('',(" + pointB.XString + "," + pointB.YString + "," + pointB.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 29).Append(" = SURFACE_CURVE('',#").Append(nextId + 30).Append(",(#").Append(nextId + 34).Append(",#").Append(nextId + 46).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 30).Append(" = LINE('',#").Append(nextId + 31).Append(",#").Append(nextId + 32).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 31).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 32).Append(" = VECTOR('',#").Append(nextId + 33).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 33).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 34).Append(" = PCURVE('',#").Append(nextId + 35).Append(",#").Append(nextId + 40).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 35).Append(" = PLANE('',#").Append(nextId + 36).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 36).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 37).Append(",#").Append(nextId + 38).Append(",#").Append(nextId + 39).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 37).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 38).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 39).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 40).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 41).Append("),#").Append(nextId + 45).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 41).Append(" = LINE('',#").Append(nextId + 42).Append(",#").Append(nextId + 43).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 42).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 43).Append(" = VECTOR('',#").Append(nextId + 44).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 44).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 45).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 46).Append(" = PCURVE('',#").Append(nextId + 47).Append(",#").Append(nextId + 52).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 47).Append(" = PLANE('',#").Append(nextId + 48).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 48).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 49).Append(",#").Append(nextId + 50).Append(",#").Append(nextId + 51).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 49).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 50).Append(" = DIRECTION('',(0.,1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 51).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 52).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 53).Append("),#").Append(nextId + 57).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 53).Append(" = LINE('',#").Append(nextId + 54).Append(",#").Append(nextId + 55).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 54).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 55).Append(" = VECTOR('',#").Append(nextId + 56).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 56).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 57).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 58).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 59).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 59).Append(" = EDGE_CURVE('',#").Append(nextId + 25).Append(",#").Append(nextId + 60).Append(",#").Append(nextId + 62).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 60).Append(" = VERTEX_POINT('',#").Append(nextId + 61).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 61).Append(" = CARTESIAN_POINT('',(" + pointE.XString + "," + pointE.YString + "," + pointE.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 62).Append(" = SURFACE_CURVE('',#").Append(nextId + 63).Append(",(#").Append(nextId + 67).Append(",#").Append(nextId + 74).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 63).Append(" = LINE('',#").Append(nextId + 64).Append(",#").Append(nextId + 65).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 64).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 65).Append(" = VECTOR('',#").Append(nextId + 66).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 66).Append(" = DIRECTION('',(0.,1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 67).Append(" = PCURVE('',#").Append(nextId + 35).Append(",#").Append(nextId + 68).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 68).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 69).Append("),#").Append(nextId + 73).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 69).Append(" = LINE('',#").Append(nextId + 70).Append(",#").Append(nextId + 71).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 70).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 71).Append(" = VECTOR('',#").Append(nextId + 72).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 72).Append(" = DIRECTION('',(0.,-1.));");
            sb.AppendLine().Append("#").Append(nextId + 73).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 74).Append(" = PCURVE('',#").Append(nextId + 75).Append(",#").Append(nextId + 80).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 75).Append(" = PLANE('',#").Append(nextId + 76).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 76).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 77).Append(",#").Append(nextId + 78).Append(",#").Append(nextId + 79).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 77).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 78).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 79).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 80).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 81).Append("),#").Append(nextId + 85).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 81).Append(" = LINE('',#").Append(nextId + 82).Append(",#").Append(nextId + 83).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 82).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 83).Append(" = VECTOR('',#").Append(nextId + 84).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 84).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 85).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 86).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 87).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 87).Append(" = EDGE_CURVE('',#").Append(nextId + 60).Append(",#").Append(nextId + 88).Append(",#").Append(nextId + 90).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 88).Append(" = VERTEX_POINT('',#").Append(nextId + 89).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 89).Append(" = CARTESIAN_POINT('',(" + pointF.XString + "," + pointF.YString + "," + pointF.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 90).Append(" = SURFACE_CURVE('',#").Append(nextId + 91).Append(",(#").Append(nextId + 95).Append(",#").Append(nextId + 102).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 91).Append(" = LINE('',#").Append(nextId + 92).Append(",#").Append(nextId + 93).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 92).Append(" = CARTESIAN_POINT('',(" + pointE.XString + "," + pointE.YString + "," + pointE.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 93).Append(" = VECTOR('',#").Append(nextId + 94).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 94).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 95).Append(" = PCURVE('',#").Append(nextId + 35).Append(",#").Append(nextId + 96).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 96).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 97).Append("),#").Append(nextId + 101).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 97).Append(" = LINE('',#").Append(nextId + 98).Append(",#").Append(nextId + 99).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 98).Append(" = CARTESIAN_POINT('',(0.,-" + Scale.YString + "));");
            sb.AppendLine().Append("#").Append(nextId + 99).Append(" = VECTOR('',#").Append(nextId + 100).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 100).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 101).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 102).Append(" = PCURVE('',#").Append(nextId + 103).Append(",#").Append(nextId + 108).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 103).Append(" = PLANE('',#").Append(nextId + 104).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 104).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 105).Append(",#").Append(nextId + 106).Append(",#").Append(nextId + 107).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 105).Append(" = CARTESIAN_POINT('',(" + pointE.XString + "," + pointE.YString + "," + pointE.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 106).Append(" = DIRECTION('',(0.,1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 107).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 108).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 109).Append("),#").Append(nextId + 113).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 109).Append(" = LINE('',#").Append(nextId + 110).Append(",#").Append(nextId + 111).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 110).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 111).Append(" = VECTOR('',#").Append(nextId + 112).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 112).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 113).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 114).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 115).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 115).Append(" = EDGE_CURVE('',#").Append(nextId + 27).Append(",#").Append(nextId + 88).Append(",#").Append(nextId + 116).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 116).Append(" = SURFACE_CURVE('',#").Append(nextId + 117).Append(",(#").Append(nextId + 121).Append(",#").Append(nextId + 128).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 117).Append(" = LINE('',#").Append(nextId + 118).Append(",#").Append(nextId + 119).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 118).Append(" = CARTESIAN_POINT('',(" + pointB.XString + "," + pointB.YString + "," + pointB.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 119).Append(" = VECTOR('',#").Append(nextId + 120).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 120).Append(" = DIRECTION('',(0.,1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 121).Append(" = PCURVE('',#").Append(nextId + 35).Append(",#").Append(nextId + 122).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 122).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 123).Append("),#").Append(nextId + 127).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 123).Append(" = LINE('',#").Append(nextId + 124).Append(",#").Append(nextId + 125).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 124).Append(" = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));");
            sb.AppendLine().Append("#").Append(nextId + 125).Append(" = VECTOR('',#").Append(nextId + 126).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 126).Append(" = DIRECTION('',(0.,-1.));");
            sb.AppendLine().Append("#").Append(nextId + 127).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 128).Append(" = PCURVE('',#").Append(nextId + 129).Append(",#").Append(nextId + 134).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 129).Append(" = PLANE('',#").Append(nextId + 130).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 130).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 131).Append(",#").Append(nextId + 132).Append(",#").Append(nextId + 133).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 131).Append(" = CARTESIAN_POINT('',(" + pointB.XString + "," + pointB.YString + "," + pointB.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 132).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 133).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 134).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 135).Append("),#").Append(nextId + 139).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 135).Append(" = LINE('',#").Append(nextId + 136).Append(",#").Append(nextId + 137).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 136).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 137).Append(" = VECTOR('',#").Append(nextId + 138).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 138).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 139).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 140).Append(" = ADVANCED_FACE('',(#").Append(nextId + 141).Append("),#").Append(nextId + 155).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 141).Append(" = FACE_BOUND('',#").Append(nextId + 142).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 142).Append(" = EDGE_LOOP('',(#").Append(nextId + 143).Append(",#").Append(nextId + 173).Append(",#").Append(nextId + 196).Append(",#").Append(nextId + 219).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 143).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 144).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 144).Append(" = EDGE_CURVE('',#").Append(nextId + 145).Append(",#").Append(nextId + 147).Append(",#").Append(nextId + 149).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 145).Append(" = VERTEX_POINT('',#").Append(nextId + 146).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 146).Append(" = CARTESIAN_POINT('',(" + pointD.XString + "," + pointD.YString + "," + pointD.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 147).Append(" = VERTEX_POINT('',#").Append(nextId + 148).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 148).Append(" = CARTESIAN_POINT('',(" + pointC.XString + "," + pointC.YString + "," + pointC.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 149).Append(" = SURFACE_CURVE('',#").Append(nextId + 150).Append(",(#").Append(nextId + 154).Append(",#").Append(nextId + 166).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 150).Append(" = LINE('',#").Append(nextId + 151).Append(",#").Append(nextId + 152).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 151).Append(" = CARTESIAN_POINT('',(" + pointD.XString + "," + pointD.YString + "," + pointD.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 152).Append(" = VECTOR('',#").Append(nextId + 153).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 153).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 154).Append(" = PCURVE('',#").Append(nextId + 155).Append(",#").Append(nextId + 160).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 155).Append(" = PLANE('',#").Append(nextId + 156).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 156).Append(" = AXIS2_PLACEMENT_3D('',#").Append(nextId + 157).Append(",#").Append(nextId + 158).Append(",#").Append(nextId + 159).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 157).Append(" = CARTESIAN_POINT('',(" + pointD.XString + "," + pointD.YString + "," + pointD.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 158).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 159).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 160).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 161).Append("),#").Append(nextId + 165).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 161).Append(" = LINE('',#").Append(nextId + 162).Append(",#").Append(nextId + 163).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 162).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 163).Append(" = VECTOR('',#").Append(nextId + 164).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 164).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 165).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 166).Append(" = PCURVE('',#").Append(nextId + 47).Append(",#").Append(nextId + 167).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 167).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 168).Append("),#").Append(nextId + 172).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 168).Append(" = LINE('',#").Append(nextId + 169).Append(",#").Append(nextId + 170).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 169).Append(" = CARTESIAN_POINT('',(0.," + Scale.XString + "));");
            sb.AppendLine().Append("#").Append(nextId + 170).Append(" = VECTOR('',#").Append(nextId + 171).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 171).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 172).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 173).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 174).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 174).Append(" = EDGE_CURVE('',#").Append(nextId + 145).Append(",#").Append(nextId + 175).Append(",#").Append(nextId + 177).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 175).Append(" = VERTEX_POINT('',#").Append(nextId + 176).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 176).Append(" = CARTESIAN_POINT('',(" + pointH.XString + "," + pointH.YString + "," + pointH.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 177).Append(" = SURFACE_CURVE('',#").Append(nextId + 178).Append(",(#").Append(nextId + 182).Append(",#").Append(nextId + 189).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 178).Append(" = LINE('',#").Append(nextId + 179).Append(",#").Append(nextId + 180).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 179).Append(" = CARTESIAN_POINT('',(" + pointD.XString + "," + pointD.YString + "," + pointD.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 180).Append(" = VECTOR('',#").Append(nextId + 181).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 181).Append(" = DIRECTION('',(0.,1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 182).Append(" = PCURVE('',#").Append(nextId + 155).Append(",#").Append(nextId + 183).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 183).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 184).Append("),#").Append(nextId + 188).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 184).Append(" = LINE('',#").Append(nextId + 185).Append(",#").Append(nextId + 186).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 185).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 186).Append(" = VECTOR('',#").Append(nextId + 187).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 187).Append(" = DIRECTION('',(0.,-1.));");
            sb.AppendLine().Append("#").Append(nextId + 188).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 189).Append(" = PCURVE('',#").Append(nextId + 75).Append(",#").Append(nextId + 190).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 190).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 191).Append("),#").Append(nextId + 195).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 191).Append(" = LINE('',#").Append(nextId + 192).Append(",#").Append(nextId + 193).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 192).Append(" = CARTESIAN_POINT('',(" + Scale.XString + ",0.));");
            sb.AppendLine().Append("#").Append(nextId + 193).Append(" = VECTOR('',#").Append(nextId + 194).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 194).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 195).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 196).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 197).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 197).Append(" = EDGE_CURVE('',#").Append(nextId + 175).Append(",#").Append(nextId + 198).Append(",#").Append(nextId + 200).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 198).Append(" = VERTEX_POINT('',#").Append(nextId + 199).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 199).Append(" = CARTESIAN_POINT('',(" + pointG.XString + "," + pointG.YString + "," + pointG.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 200).Append(" = SURFACE_CURVE('',#").Append(nextId + 201).Append(",(#").Append(nextId + 205).Append(",#").Append(nextId + 212).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 201).Append(" = LINE('',#").Append(nextId + 202).Append(",#").Append(nextId + 203).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 202).Append(" = CARTESIAN_POINT('',(" + pointH.XString + "," + pointH.YString + "," + pointH.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 203).Append(" = VECTOR('',#").Append(nextId + 204).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 204).Append(" = DIRECTION('',(0.,0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 205).Append(" = PCURVE('',#").Append(nextId + 155).Append(",#").Append(nextId + 206).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 206).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 207).Append("),#").Append(nextId + 211).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 207).Append(" = LINE('',#").Append(nextId + 208).Append(",#").Append(nextId + 209).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 208).Append(" = CARTESIAN_POINT('',(0.,-" + Scale.YString + "));");
            sb.AppendLine().Append("#").Append(nextId + 209).Append(" = VECTOR('',#").Append(nextId + 210).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 210).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 211).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 212).Append(" = PCURVE('',#").Append(nextId + 103).Append(",#").Append(nextId + 213).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 213).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 214).Append("),#").Append(nextId + 218).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 214).Append(" = LINE('',#").Append(nextId + 215).Append(",#").Append(nextId + 216).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 215).Append(" = CARTESIAN_POINT('',(0.," + Scale.XString + "));");
            sb.AppendLine().Append("#").Append(nextId + 216).Append(" = VECTOR('',#").Append(nextId + 217).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 217).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 218).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 219).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 220).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 220).Append(" = EDGE_CURVE('',#").Append(nextId + 147).Append(",#").Append(nextId + 198).Append(",#").Append(nextId + 221).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 221).Append(" = SURFACE_CURVE('',#").Append(nextId + 222).Append(",(#").Append(nextId + 226).Append(",#").Append(nextId + 233).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 222).Append(" = LINE('',#").Append(nextId + 223).Append(",#").Append(nextId + 224).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 223).Append(" = CARTESIAN_POINT('',(" + pointC.XString + "," + pointC.YString + "," + pointC.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 224).Append(" = VECTOR('',#").Append(nextId + 225).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 225).Append(" = DIRECTION('',(0.,1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 226).Append(" = PCURVE('',#").Append(nextId + 155).Append(",#").Append(nextId + 227).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 227).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 228).Append("),#").Append(nextId + 232).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 228).Append(" = LINE('',#").Append(nextId + 229).Append(",#").Append(nextId + 230).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 229).Append(" = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));");
            sb.AppendLine().Append("#").Append(nextId + 230).Append(" = VECTOR('',#").Append(nextId + 231).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 231).Append(" = DIRECTION('',(0.,-1.));");
            sb.AppendLine().Append("#").Append(nextId + 232).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 233).Append(" = PCURVE('',#").Append(nextId + 129).Append(",#").Append(nextId + 234).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 234).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 235).Append("),#").Append(nextId + 239).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 235).Append(" = LINE('',#").Append(nextId + 236).Append(",#").Append(nextId + 237).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 236).Append(" = CARTESIAN_POINT('',(" + Scale.XString + ",0.));");
            sb.AppendLine().Append("#").Append(nextId + 237).Append(" = VECTOR('',#").Append(nextId + 238).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 238).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 239).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 240).Append(" = ADVANCED_FACE('',(#").Append(nextId + 241).Append("),#").Append(nextId + 47).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 241).Append(" = FACE_BOUND('',#").Append(nextId + 242).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 242).Append(" = EDGE_LOOP('',(#").Append(nextId + 243).Append(",#").Append(nextId + 264).Append(",#").Append(nextId + 265).Append(",#").Append(nextId + 286).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 243).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 244).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 244).Append(" = EDGE_CURVE('',#").Append(nextId + 25).Append(",#").Append(nextId + 145).Append(",#").Append(nextId + 245).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 245).Append(" = SURFACE_CURVE('',#").Append(nextId + 246).Append(",(#").Append(nextId + 250).Append(",#").Append(nextId + 257).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 246).Append(" = LINE('',#").Append(nextId + 247).Append(",#").Append(nextId + 248).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 247).Append(" = CARTESIAN_POINT('',(" + pointA.XString + "," + pointA.YString + "," + pointA.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 248).Append(" = VECTOR('',#").Append(nextId + 249).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 249).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 250).Append(" = PCURVE('',#").Append(nextId + 47).Append(",#").Append(nextId + 251).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 251).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 252).Append("),#").Append(nextId + 256).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 252).Append(" = LINE('',#").Append(nextId + 253).Append(",#").Append(nextId + 254).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 253).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 254).Append(" = VECTOR('',#").Append(nextId + 255).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 255).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 256).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 257).Append(" = PCURVE('',#").Append(nextId + 75).Append(",#").Append(nextId + 258).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 258).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 259).Append("),#").Append(nextId + 263).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 259).Append(" = LINE('',#").Append(nextId + 260).Append(",#").Append(nextId + 261).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 260).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 261).Append(" = VECTOR('',#").Append(nextId + 262).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 262).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 263).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 264).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 24).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 265).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 266).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 266).Append(" = EDGE_CURVE('',#").Append(nextId + 27).Append(",#").Append(nextId + 147).Append(",#").Append(nextId + 267).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 267).Append(" = SURFACE_CURVE('',#").Append(nextId + 268).Append(",(#").Append(nextId + 272).Append(",#").Append(nextId + 279).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 268).Append(" = LINE('',#").Append(nextId + 269).Append(",#").Append(nextId + 270).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 269).Append(" = CARTESIAN_POINT('',(" + pointB.XString + "," + pointB.YString + "," + pointB.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 270).Append(" = VECTOR('',#").Append(nextId + 271).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 271).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 272).Append(" = PCURVE('',#").Append(nextId + 47).Append(",#").Append(nextId + 273).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 273).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 274).Append("),#").Append(nextId + 278).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 274).Append(" = LINE('',#").Append(nextId + 275).Append(",#").Append(nextId + 276).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 275).Append(" = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));");
            sb.AppendLine().Append("#").Append(nextId + 276).Append(" = VECTOR('',#").Append(nextId + 277).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 277).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 278).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 279).Append(" = PCURVE('',#").Append(nextId + 129).Append(",#").Append(nextId + 280).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 280).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 281).Append("),#").Append(nextId + 285).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 281).Append(" = LINE('',#").Append(nextId + 282).Append(",#").Append(nextId + 283).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 282).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 283).Append(" = VECTOR('',#").Append(nextId + 284).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 284).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 285).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 286).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 144).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 287).Append(" = ADVANCED_FACE('',(#").Append(nextId + 288).Append("),#").Append(nextId + 103).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 288).Append(" = FACE_BOUND('',#").Append(nextId + 289).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 289).Append(" = EDGE_LOOP('',(#").Append(nextId + 290).Append(",#").Append(nextId + 311).Append(",#").Append(nextId + 312).Append(",#").Append(nextId + 333).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 290).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 291).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 291).Append(" = EDGE_CURVE('',#").Append(nextId + 60).Append(",#").Append(nextId + 175).Append(",#").Append(nextId + 292).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 292).Append(" = SURFACE_CURVE('',#").Append(nextId + 293).Append(",(#").Append(nextId + 297).Append(",#").Append(nextId + 304).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 293).Append(" = LINE('',#").Append(nextId + 294).Append(",#").Append(nextId + 295).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 294).Append(" = CARTESIAN_POINT('',(" + pointE.XString + "," + pointE.YString + "," + pointE.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 295).Append(" = VECTOR('',#").Append(nextId + 296).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 296).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 297).Append(" = PCURVE('',#").Append(nextId + 103).Append(",#").Append(nextId + 298).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 298).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 299).Append("),#").Append(nextId + 303).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 299).Append(" = LINE('',#").Append(nextId + 300).Append(",#").Append(nextId + 301).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 300).Append(" = CARTESIAN_POINT('',(0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 301).Append(" = VECTOR('',#").Append(nextId + 302).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 302).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 303).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 304).Append(" = PCURVE('',#").Append(nextId + 75).Append(",#").Append(nextId + 305).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 305).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 306).Append("),#").Append(nextId + 310).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 306).Append(" = LINE('',#").Append(nextId + 307).Append(",#").Append(nextId + 308).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 307).Append(" = CARTESIAN_POINT('',(0.," + Scale.YString + "));");
            sb.AppendLine().Append("#").Append(nextId + 308).Append(" = VECTOR('',#").Append(nextId + 309).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 309).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 310).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 311).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 87).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 312).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 313).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 313).Append(" = EDGE_CURVE('',#").Append(nextId + 88).Append(",#").Append(nextId + 198).Append(",#").Append(nextId + 314).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 314).Append(" = SURFACE_CURVE('',#").Append(nextId + 315).Append(",(#").Append(nextId + 319).Append(",#").Append(nextId + 326).Append("),.PCURVE_S1.);");
            sb.AppendLine().Append("#").Append(nextId + 315).Append(" = LINE('',#").Append(nextId + 316).Append(",#").Append(nextId + 317).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 316).Append(" = CARTESIAN_POINT('',(" + pointF.XString + "," + pointF.YString + "," + pointF.ZString + "));");
            sb.AppendLine().Append("#").Append(nextId + 317).Append(" = VECTOR('',#").Append(nextId + 318).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 318).Append(" = DIRECTION('',(1.,0.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 319).Append(" = PCURVE('',#").Append(nextId + 103).Append(",#").Append(nextId + 320).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 320).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 321).Append("),#").Append(nextId + 325).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 321).Append(" = LINE('',#").Append(nextId + 322).Append(",#").Append(nextId + 323).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 322).Append(" = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));");
            sb.AppendLine().Append("#").Append(nextId + 323).Append(" = VECTOR('',#").Append(nextId + 324).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 324).Append(" = DIRECTION('',(0.,1.));");
            sb.AppendLine().Append("#").Append(nextId + 325).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 326).Append(" = PCURVE('',#").Append(nextId + 129).Append(",#").Append(nextId + 327).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 327).Append(" = DEFINITIONAL_REPRESENTATION('',(#").Append(nextId + 328).Append("),#").Append(nextId + 332).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 328).Append(" = LINE('',#").Append(nextId + 329).Append(",#").Append(nextId + 330).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 329).Append(" = CARTESIAN_POINT('',(0.," + Scale.YString + "));");
            sb.AppendLine().Append("#").Append(nextId + 330).Append(" = VECTOR('',#").Append(nextId + 331).Append(",1.);");
            sb.AppendLine().Append("#").Append(nextId + 331).Append(" = DIRECTION('',(1.,0.));");
            sb.AppendLine().Append("#").Append(nextId + 332).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );");
            sb.AppendLine().Append("#").Append(nextId + 333).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 197).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 334).Append(" = ADVANCED_FACE('',(#").Append(nextId + 335).Append("),#").Append(nextId + 75).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 335).Append(" = FACE_BOUND('',#").Append(nextId + 336).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 336).Append(" = EDGE_LOOP('',(#").Append(nextId + 337).Append(",#").Append(nextId + 338).Append(",#").Append(nextId + 339).Append(",#").Append(nextId + 340).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 337).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 59).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 338).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 244).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 339).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 174).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 340).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 291).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 341).Append(" = ADVANCED_FACE('',(#").Append(nextId + 342).Append("),#").Append(nextId + 129).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 342).Append(" = FACE_BOUND('',#").Append(nextId + 343).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 343).Append(" = EDGE_LOOP('',(#").Append(nextId + 344).Append(",#").Append(nextId + 345).Append(",#").Append(nextId + 346).Append(",#").Append(nextId + 347).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 344).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 115).Append(",.F.);");
            sb.AppendLine().Append("#").Append(nextId + 345).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 266).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 346).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 220).Append(",.T.);");
            sb.AppendLine().Append("#").Append(nextId + 347).Append(" = ORIENTED_EDGE('',*,*,#").Append(nextId + 313).Append(",.F.);");
#endregion
#region SOLID SCALE
            sb.AppendLine().Append("#").Append(nextId + 348).Append(" = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#").Append(nextId + 352).Append(")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#").Append(nextId + 349).Append(",#").Append(nextId + 350).Append(",#").Append(nextId + 351).Append(")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY'));");
            sb.AppendLine().Append("#").Append(nextId + 349).Append(" = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );");
            sb.AppendLine().Append("#").Append(nextId + 350).Append(" = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );");
            sb.AppendLine().Append("#").Append(nextId + 351).Append(" = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );");
            sb.AppendLine().Append("#").Append(nextId + 352).Append(" = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#").Append(nextId + 349).Append(",'distance_accuracy_value','confusion accuracy');");
#endregion
#region SOLID DEFINITION
            sb.AppendLine().Append("#").Append(nextId + 353).Append(" = SHAPE_DEFINITION_REPRESENTATION(#").Append(nextId + 354).Append(",#").Append(nextId + 17).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 354).Append(" = PRODUCT_DEFINITION_SHAPE('','',#").Append(nextId + 355).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 355).Append(" = PRODUCT_DEFINITION('design','',#").Append(nextId + 356).Append(",#").Append(nextId + 359).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 356).Append(" = PRODUCT_DEFINITION_FORMATION('','',#").Append(nextId + 357).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 357).Append(" = PRODUCT('" + Name + "_box_solid" + "','" + Name + "_box_solid" + "','',(#").Append(nextId + 358).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 358).Append(" = PRODUCT_CONTEXT('',#2,'mechanical');");
            sb.AppendLine().Append("#").Append(nextId + 359).Append(" = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');");
#endregion
#region HIERARCHY
            // solid to box part             
            sb.AppendLine().Append("#").Append(nextId + 360).Append(" = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#").Append(nextId + 361).Append(",#").Append(nextId + 363).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 361).Append(" = ( REPRESENTATION_RELATIONSHIP('','',#").Append(nextId + 17).Append(",#").Append(nextId + 7).Append(") REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#").Append(nextId + 362).Append(") SHAPE_REPRESENTATION_RELATIONSHIP() );");
            sb.AppendLine().Append("#").Append(nextId + 362).Append(" = ITEM_DEFINED_TRANSFORMATION('','',#11,#").Append(nextId + 8).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 363).Append(" = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#").Append(nextId + 364).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 364).Append(" = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + (StepManager.ObjectIndex + 0) + "','=>[0:1:1:" + (StepManager.ObjectIndex + 0) + "]','',#").Append(nextId + 2).Append(",#").Append(nextId + 355).Append(",$);");
            sb.AppendLine().Append("#").Append(nextId + 365).Append(" = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#").Append(nextId + 357).Append("));");
            // part to parent part
            sb.AppendLine().Append("#").Append(nextId + 366).Append(" = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#").Append(nextId + 367).Append(",#").Append(nextId + 369).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 367).Append(" = ( REPRESENTATION_RELATIONSHIP('','',#").Append(nextId + 7).Append(",#" + Parent.StepId_SHAPE_REPRESENTATION + ") REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#").Append(nextId + 368).Append(") SHAPE_REPRESENTATION_RELATIONSHIP() );");
            sb.AppendLine().Append("#").Append(nextId + 368).Append(" = ITEM_DEFINED_TRANSFORMATION('','',#11,#" + Parent.ChildrenStepId_AXIS2_PLACEMENT_3D[childIndex] + ");");
            sb.AppendLine().Append("#").Append(nextId + 369).Append(" = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#").Append(nextId + 370).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 370).Append(" = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + (StepManager.ObjectIndex + 1) + "','=>[0:1:1:" + (StepManager.ObjectIndex + 1) + "]','',#" + Parent.StepId_PRODUCT_DEFINITION + ",#").Append(nextId + 2).Append(",$);");
            sb.AppendLine().Append("#").Append(nextId + 371).Append(" = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#").Append(nextId + 4).Append("));");
#endregion
#region MATERIAL           
            sb.AppendLine().Append("#").Append(nextId + 372).Append(" = MECHANICAL_DESIGN_GEOMETRIC_PRESENTATION_REPRESENTATION('',(#").Append(nextId + 373).Append("),#").Append(nextId + 348).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 373).Append(" = STYLED_ITEM('color',(#").Append(nextId + 374).Append("),#").Append(nextId + 18).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 374).Append(" = PRESENTATION_STYLE_ASSIGNMENT((#").Append(nextId + 375).Append(",#").Append(nextId + 381).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 375).Append(" = SURFACE_STYLE_USAGE(.BOTH.,#").Append(nextId + 376).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 376).Append(" = SURFACE_SIDE_STYLE('',(#").Append(nextId + 377).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 377).Append(" = SURFACE_STYLE_FILL_AREA(#").Append(nextId + 378).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 378).Append(" = FILL_AREA_STYLE('',(#").Append(nextId + 379).Append("));");
            sb.AppendLine().Append("#").Append(nextId + 379).Append(" = FILL_AREA_STYLE_COLOUR('',#").Append(nextId + 380).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 380).Append(" = COLOUR_RGB(''," + Color.R.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "," + Color.G.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "," + Color.B.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + ");");
            sb.AppendLine().Append("#").Append(nextId + 381).Append(" = CURVE_STYLE('',#").Append(nextId + 382).Append(",POSITIVE_LENGTH_MEASURE(0.1),#").Append(nextId + 380).Append(");");
            sb.AppendLine().Append("#").Append(nextId + 382).Append(" = DRAUGHTING_PRE_DEFINED_CURVE_FONT('continuous');");
#endregion

            StepManager.ObjectIndex += 2;
            StepManager.NextId = (nextId + 383);

            stepEntries.Add(sb.ToString());
            sb.Clear();
        }

    }

}
