using SimpleStepWriter.Helper;

namespace SimpleStepWriter.Content
{
    /// <summary> Box class representing a part with a simple solid in box form that has a size, position, rotation, color and name.
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
    ///     Right-handed coordinate system
    ///     
    /// 
    /// </summary>
    public class Box : IContent
    {
        // interface implementation
        public IStepManager StepManager { get; private set; }

        // user provided values
        public string Name { get; private set; } = "DefaultName";
        public Vector3 Center { get; private set; } = Vector3.Zero;
        public Vector3 Scale { get; private set; } = Vector3.One;
        public Vector3 Rotation { get; private set; } = Vector3.Zero;
        public Color Color { get; private set; } = Color.White;
                
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
        /// Get STEP file lines.
        /// </summary>
        /// <param name="partCoordinateSystemId">Id that references the part coordiante system.</param>
        /// <param name="childIndex">This childs index based on all children of the root assembly.</param>
        /// <returns>The content, line by line, that we append to the step file.</returns>
        public string[] GetLines(long partCoordinateSystemId, long childIndex)
        {
            // calculate all points for the box
            pointA = new Vector3( -(Scale.X / 2), -(Scale.Y / 2), -(Scale.Z / 2));  
            pointB = new Vector3( -(Scale.X / 2), -(Scale.Y / 2),  (Scale.Z / 2));
            pointC = new Vector3(  (Scale.X / 2), -(Scale.Y / 2),  (Scale.Z / 2));
            pointD = new Vector3(  (Scale.X / 2), -(Scale.Y / 2), -(Scale.Z / 2)); 
            pointE = new Vector3( -(Scale.X / 2),  (Scale.Y / 2), -(Scale.Z / 2)); 
            pointF = new Vector3( -(Scale.X / 2),  (Scale.Y / 2),  (Scale.Z / 2));
            pointG = new Vector3(  (Scale.X / 2),  (Scale.Y / 2),  (Scale.Z / 2));
            pointH = new Vector3(  (Scale.X / 2),  (Scale.Y / 2), -(Scale.Z / 2));
            
            // each object (part and solid) require a globally unique index value starting with 1
            long objectIndex = childIndex * 2 - 1;
            
            // return lines that represent a box that considers the previously provided information
            string[] lines = new string[] {
                // part section start
                @"#" + (StepManager.NextId + 0) + " = SHAPE_DEFINITION_REPRESENTATION(#" + (StepManager.NextId + 1) + ",#" + (StepManager.NextId + 7) + ");",             
                @"#" + (StepManager.NextId + 1) + " = PRODUCT_DEFINITION_SHAPE('','',#" + (StepManager.NextId + 2) + ");",
                @"#" + (StepManager.NextId + 2) + " = PRODUCT_DEFINITION('design','',#" + (StepManager.NextId + 3) + ",#" + (StepManager.NextId + 6) + ");",
                @"#" + (StepManager.NextId + 3) + " = PRODUCT_DEFINITION_FORMATION('','',#" + (StepManager.NextId + 4) + ");",
                @"#" + (StepManager.NextId + 4) + " = PRODUCT('" + Name + "','" + Name + "','',(#" + (StepManager.NextId + 5) + "));",
                @"#" + (StepManager.NextId + 5) + " = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#" + (StepManager.NextId + 6) + " = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');",
                @"#" + (StepManager.NextId + 7) + " = SHAPE_REPRESENTATION('',(#11,#" + (StepManager.NextId + 8) + "),#" + (StepManager.NextId + 12) + ");",
                @"#" + (StepManager.NextId + 8) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 9) + ",#" + (StepManager.NextId + 10) + ",#" + (StepManager.NextId + 11) + ");",
                @"#" + (StepManager.NextId + 9) + " = CARTESIAN_POINT('',(0.,0.,0.));",
                @"#" + (StepManager.NextId + 10) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 11) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 12) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 16) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 13) + ",#" + (StepManager.NextId + 14) + ",#" + (StepManager.NextId + 15) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY') ); ",
                @"#" + (StepManager.NextId + 13) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (StepManager.NextId + 14) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (StepManager.NextId + 15) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (StepManager.NextId + 16) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 13) + ",'distance_accuracy_value','confusion accuracy'); ",
                @"#" + (StepManager.NextId + 17) + " = ADVANCED_BREP_SHAPE_REPRESENTATION('',(#11,#" + (StepManager.NextId + 18) + "),#" + (StepManager.NextId + 348) + ");",
                @"#" + (StepManager.NextId + 18) + " = MANIFOLD_SOLID_BREP('',#" + (StepManager.NextId + 19) + ");",
                // geometry section start
                @"#" + (StepManager.NextId + 19) + " = CLOSED_SHELL('',(#" + (StepManager.NextId + 20) + ",#" + (StepManager.NextId + 140) + ",#" + (StepManager.NextId + 240) + ",#" + (StepManager.NextId + 287) + ",#" + (StepManager.NextId + 334) + ",#" + (StepManager.NextId + 341) + "));",
                @"#" + (StepManager.NextId + 20) + " = ADVANCED_FACE('',(#" + (StepManager.NextId + 21) + "),#" + (StepManager.NextId + 35) + ",.F.);",
                @"#" + (StepManager.NextId + 21) + " = FACE_BOUND('',#" + (StepManager.NextId + 22) + ",.F.);",
                @"#" + (StepManager.NextId + 22) + " = EDGE_LOOP('',(#" + (StepManager.NextId + 23) + ",#" + (StepManager.NextId + 58) + ",#" + (StepManager.NextId + 86) + ",#" + (StepManager.NextId + 114) + "));",
                @"#" + (StepManager.NextId + 23) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 24) + ",.F.);",
                @"#" + (StepManager.NextId + 24) + " = EDGE_CURVE('',#" + (StepManager.NextId + 25) + ",#" + (StepManager.NextId + 27) + ",#" + (StepManager.NextId + 29) + ",.T.);",
                @"#" + (StepManager.NextId + 25) + " = VERTEX_POINT('',#" + (StepManager.NextId + 26) + ");",
                @"#" + (StepManager.NextId + 26) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 27) + " = VERTEX_POINT('',#" + (StepManager.NextId + 28) + ");",
                @"#" + (StepManager.NextId + 28) + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + (StepManager.NextId + 29) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 30) + ",(#" + (StepManager.NextId + 34) + ",#" + (StepManager.NextId + 46) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 30) + " = LINE('',#" + (StepManager.NextId + 31) + ",#" + (StepManager.NextId + 32) + ");",
                @"#" + (StepManager.NextId + 31) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 32) + " = VECTOR('',#" + (StepManager.NextId + 33) + ",1.);",
                @"#" + (StepManager.NextId + 33) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 34) + " = PCURVE('',#" + (StepManager.NextId + 35) + ",#" + (StepManager.NextId + 40) + ");",
                @"#" + (StepManager.NextId + 35) + " = PLANE('',#" + (StepManager.NextId + 36) + ");",
                @"#" + (StepManager.NextId + 36) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 37) + ",#" + (StepManager.NextId + 38) + ",#" + (StepManager.NextId + 39) + ");",
                @"#" + (StepManager.NextId + 37) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 38) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 39) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 40) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 41) + "),#" + (StepManager.NextId + 45) + ");",
                @"#" + (StepManager.NextId + 41) + " = LINE('',#" + (StepManager.NextId + 42) + ",#" + (StepManager.NextId + 43) + ");",
                @"#" + (StepManager.NextId + 42) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 43) + " = VECTOR('',#" + (StepManager.NextId + 44) + ",1.);",
                @"#" + (StepManager.NextId + 44) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 45) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 46) + " = PCURVE('',#" + (StepManager.NextId + 47) + ",#" + (StepManager.NextId + 52) + ");",
                @"#" + (StepManager.NextId + 47) + " = PLANE('',#" + (StepManager.NextId + 48) + ");",
                @"#" + (StepManager.NextId + 48) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 49) + ",#" + (StepManager.NextId + 50) + ",#" + (StepManager.NextId + 51) + ");",
                @"#" + (StepManager.NextId + 49) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 50) + " = DIRECTION('',(0.,1.,0.));",
                @"#" + (StepManager.NextId + 51) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 52) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 53) + "),#" + (StepManager.NextId + 57) + ");",
                @"#" + (StepManager.NextId + 53) + " = LINE('',#" + (StepManager.NextId + 54) + ",#" + (StepManager.NextId + 55) + ");",
                @"#" + (StepManager.NextId + 54) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 55) + " = VECTOR('',#" + (StepManager.NextId + 56) + ",1.);",
                @"#" + (StepManager.NextId + 56) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 57) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 58) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 59) + ",.T.);",
                @"#" + (StepManager.NextId + 59) + " = EDGE_CURVE('',#" + (StepManager.NextId + 25) + ",#" + (StepManager.NextId + 60) + ",#" + (StepManager.NextId + 62) + ",.T.);",
                @"#" + (StepManager.NextId + 60) + " = VERTEX_POINT('',#" + (StepManager.NextId + 61) + ");",
                @"#" + (StepManager.NextId + 61) + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," + GetEPointZ() + "));",
                @"#" + (StepManager.NextId + 62) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 63) + ",(#" + (StepManager.NextId + 67) + ",#" + (StepManager.NextId + 74) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 63) + " = LINE('',#" + (StepManager.NextId + 64) + ",#" + (StepManager.NextId + 65) + ");",
                @"#" + (StepManager.NextId + 64) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 65) + " = VECTOR('',#" + (StepManager.NextId + 66) + ",1.);",
                @"#" + (StepManager.NextId + 66) + " = DIRECTION('',(0.,1.,0.));",
                @"#" + (StepManager.NextId + 67) + " = PCURVE('',#" + (StepManager.NextId + 35) + ",#" + (StepManager.NextId + 68) + ");",
                @"#" + (StepManager.NextId + 68) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 69) + "),#" + (StepManager.NextId + 73) + ");",
                @"#" + (StepManager.NextId + 69) + " = LINE('',#" + (StepManager.NextId + 70) + ",#" + (StepManager.NextId + 71) + ");",
                @"#" + (StepManager.NextId + 70) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 71) + " = VECTOR('',#" + (StepManager.NextId + 72) + ",1.);",
                @"#" + (StepManager.NextId + 72) + " = DIRECTION('',(0.,-1.));",
                @"#" + (StepManager.NextId + 73) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 74) + " = PCURVE('',#" + (StepManager.NextId + 75) + ",#" + (StepManager.NextId + 80) + ");",
                @"#" + (StepManager.NextId + 75) + " = PLANE('',#" + (StepManager.NextId + 76) + ");",
                @"#" + (StepManager.NextId + 76) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 77) + ",#" + (StepManager.NextId + 78) + ",#" + (StepManager.NextId + 79) + ");",
                @"#" + (StepManager.NextId + 77) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 78) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 79) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 80) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 81) + "),#" + (StepManager.NextId + 85) + ");",
                @"#" + (StepManager.NextId + 81) + " = LINE('',#" + (StepManager.NextId + 82) + ",#" + (StepManager.NextId + 83) + ");",
                @"#" + (StepManager.NextId + 82) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 83) + " = VECTOR('',#" + (StepManager.NextId + 84) + ",1.);",
                @"#" + (StepManager.NextId + 84) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 85) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 86) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 87) + ",.T.);",
                @"#" + (StepManager.NextId + 87) + " = EDGE_CURVE('',#" + (StepManager.NextId + 60) + ",#" + (StepManager.NextId + 88) + ",#" + (StepManager.NextId + 90) + ",.T.);",
                @"#" + (StepManager.NextId + 88) + " = VERTEX_POINT('',#" + (StepManager.NextId + 89) + ");",
                @"#" + (StepManager.NextId + 89) + " = CARTESIAN_POINT('',(" + GetFPointX() + "," + GetFPointY() + "," + GetFPointZ() + "));",
                @"#" + (StepManager.NextId + 90) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 91) + ",(#" + (StepManager.NextId + 95) + ",#" + (StepManager.NextId + 102) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 91) + " = LINE('',#" + (StepManager.NextId + 92) + ",#" + (StepManager.NextId + 93) + ");",
                @"#" + (StepManager.NextId + 92) + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," + GetEPointZ() + "));",
                @"#" + (StepManager.NextId + 93) + " = VECTOR('',#" + (StepManager.NextId + 94) + ",1.);",
                @"#" + (StepManager.NextId + 94) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 95) + " = PCURVE('',#" + (StepManager.NextId + 35) + ",#" + (StepManager.NextId + 96) + ");",
                @"#" + (StepManager.NextId + 96) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 97) + "),#" + (StepManager.NextId + 101) + ");",
                @"#" + (StepManager.NextId + 97) + " = LINE('',#" + (StepManager.NextId + 98) + ",#" + (StepManager.NextId + 99) + ");",
                @"#" + (StepManager.NextId + 98) + " = CARTESIAN_POINT('',(0.,-" + Scale.YString + "));",
                @"#" + (StepManager.NextId + 99) + " = VECTOR('',#" + (StepManager.NextId + 100) + ",1.);",
                @"#" + (StepManager.NextId + 100) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 101) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 102) + " = PCURVE('',#" + (StepManager.NextId + 103) + ",#" + (StepManager.NextId + 108) + ");",
                @"#" + (StepManager.NextId + 103) + " = PLANE('',#" + (StepManager.NextId + 104) + ");",
                @"#" + (StepManager.NextId + 104) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 105) + ",#" + (StepManager.NextId + 106) + ",#" + (StepManager.NextId + 107) + ");",
                @"#" + (StepManager.NextId + 105) + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," + GetEPointZ() + "));",
                @"#" + (StepManager.NextId + 106) + " = DIRECTION('',(0.,1.,0.));",
                @"#" + (StepManager.NextId + 107) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 108) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 109) + "),#" + (StepManager.NextId + 113) + ");",
                @"#" + (StepManager.NextId + 109) + " = LINE('',#" + (StepManager.NextId + 110) + ",#" + (StepManager.NextId + 111) + ");",
                @"#" + (StepManager.NextId + 110) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 111) + " = VECTOR('',#" + (StepManager.NextId + 112) + ",1.);",
                @"#" + (StepManager.NextId + 112) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 113) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 114) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 115) + ",.F.);",
                @"#" + (StepManager.NextId + 115) + " = EDGE_CURVE('',#" + (StepManager.NextId + 27) + ",#" + (StepManager.NextId + 88) + ",#" + (StepManager.NextId + 116) + ",.T.);",
                @"#" + (StepManager.NextId + 116) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 117) + ",(#" + (StepManager.NextId + 121) + ",#" + (StepManager.NextId + 128) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 117) + " = LINE('',#" + (StepManager.NextId + 118) + ",#" + (StepManager.NextId + 119) + ");",
                @"#" + (StepManager.NextId + 118) + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + (StepManager.NextId + 119) + " = VECTOR('',#" + (StepManager.NextId + 120) + ",1.);",
                @"#" + (StepManager.NextId + 120) + " = DIRECTION('',(0.,1.,0.));",
                @"#" + (StepManager.NextId + 121) + " = PCURVE('',#" + (StepManager.NextId + 35) + ",#" + (StepManager.NextId + 122) + ");",
                @"#" + (StepManager.NextId + 122) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 123) + "),#" + (StepManager.NextId + 127) + ");",
                @"#" + (StepManager.NextId + 123) + " = LINE('',#" + (StepManager.NextId + 124) + ",#" + (StepManager.NextId + 125) + ");",
                @"#" + (StepManager.NextId + 124) + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + (StepManager.NextId + 125) + " = VECTOR('',#" + (StepManager.NextId + 126) + ",1.);",
                @"#" + (StepManager.NextId + 126) + " = DIRECTION('',(0.,-1.));",
                @"#" + (StepManager.NextId + 127) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 128) + " = PCURVE('',#" + (StepManager.NextId + 129) + ",#" + (StepManager.NextId + 134) + ");",
                @"#" + (StepManager.NextId + 129) + " = PLANE('',#" + (StepManager.NextId + 130) + ");",
                @"#" + (StepManager.NextId + 130) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 131) + ",#" + (StepManager.NextId + 132) + ",#" + (StepManager.NextId + 133) + ");",
                @"#" + (StepManager.NextId + 131) + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + (StepManager.NextId + 132) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 133) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 134) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 135) + "),#" + (StepManager.NextId + 139) + ");",
                @"#" + (StepManager.NextId + 135) + " = LINE('',#" + (StepManager.NextId + 136) + ",#" + (StepManager.NextId + 137) + ");",
                @"#" + (StepManager.NextId + 136) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 137) + " = VECTOR('',#" + (StepManager.NextId + 138) + ",1.);",
                @"#" + (StepManager.NextId + 138) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 139) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 140) + " = ADVANCED_FACE('',(#" + (StepManager.NextId + 141) + "),#" + (StepManager.NextId + 155) + ",.T.);",
                @"#" + (StepManager.NextId + 141) + " = FACE_BOUND('',#" + (StepManager.NextId + 142) + ",.T.);",
                @"#" + (StepManager.NextId + 142) + " = EDGE_LOOP('',(#" + (StepManager.NextId + 143) + ",#" + (StepManager.NextId + 173) + ",#" + (StepManager.NextId + 196) + ",#" + (StepManager.NextId + 219) + "));",
                @"#" + (StepManager.NextId + 143) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 144) + ",.F.);",
                @"#" + (StepManager.NextId + 144) + " = EDGE_CURVE('',#" + (StepManager.NextId + 145) + ",#" + (StepManager.NextId + 147) + ",#" + (StepManager.NextId + 149) + ",.T.);",
                @"#" + (StepManager.NextId + 145) + " = VERTEX_POINT('',#" + (StepManager.NextId + 146) + ");",
                @"#" + (StepManager.NextId + 146) + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + (StepManager.NextId + 147) + " = VERTEX_POINT('',#" + (StepManager.NextId + 148) + ");",
                @"#" + (StepManager.NextId + 148) + " = CARTESIAN_POINT('',(" + GetCPointX() + "," + GetCPointY() + "," + GetCPointZ() + "));",
                @"#" + (StepManager.NextId + 149) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 150) + ",(#" + (StepManager.NextId + 154) + ",#" + (StepManager.NextId + 166) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 150) + " = LINE('',#" + (StepManager.NextId + 151) + ",#" + (StepManager.NextId + 152) + ");",
                @"#" + (StepManager.NextId + 151) + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + (StepManager.NextId + 152) + " = VECTOR('',#" + (StepManager.NextId + 153) + ",1.);",
                @"#" + (StepManager.NextId + 153) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 154) + " = PCURVE('',#" + (StepManager.NextId + 155) + ",#" + (StepManager.NextId + 160) + ");",
                @"#" + (StepManager.NextId + 155) + " = PLANE('',#" + (StepManager.NextId + 156) + ");",
                @"#" + (StepManager.NextId + 156) + " = AXIS2_PLACEMENT_3D('',#" + (StepManager.NextId + 157) + ",#" + (StepManager.NextId + 158) + ",#" + (StepManager.NextId + 159) + ");",
                @"#" + (StepManager.NextId + 157) + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + (StepManager.NextId + 158) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 159) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 160) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 161) + "),#" + (StepManager.NextId + 165) + ");",
                @"#" + (StepManager.NextId + 161) + " = LINE('',#" + (StepManager.NextId + 162) + ",#" + (StepManager.NextId + 163) + ");",
                @"#" + (StepManager.NextId + 162) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 163) + " = VECTOR('',#" + (StepManager.NextId + 164) + ",1.);",
                @"#" + (StepManager.NextId + 164) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 165) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 166) + " = PCURVE('',#" + (StepManager.NextId + 47) + ",#" + (StepManager.NextId + 167) + ");",
                @"#" + (StepManager.NextId + 167) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 168) + "),#" + (StepManager.NextId + 172) + ");",
                @"#" + (StepManager.NextId + 168) + " = LINE('',#" + (StepManager.NextId + 169) + ",#" + (StepManager.NextId + 170) + ");",
                @"#" + (StepManager.NextId + 169) + " = CARTESIAN_POINT('',(0.," + Scale.XString + "));",
                @"#" + (StepManager.NextId + 170) + " = VECTOR('',#" + (StepManager.NextId + 171) + ",1.);",
                @"#" + (StepManager.NextId + 171) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 172) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 173) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 174) + ",.T.);",
                @"#" + (StepManager.NextId + 174) + " = EDGE_CURVE('',#" + (StepManager.NextId + 145) + ",#" + (StepManager.NextId + 175) + ",#" + (StepManager.NextId + 177) + ",.T.);",
                @"#" + (StepManager.NextId + 175) + " = VERTEX_POINT('',#" + (StepManager.NextId + 176) + ");",
                @"#" + (StepManager.NextId + 176) + " = CARTESIAN_POINT('',(" + GetHPointX() + "," + GetHPointY() + "," + GetHPointZ() + "));",
                @"#" + (StepManager.NextId + 177) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 178) + ",(#" + (StepManager.NextId + 182) + ",#" + (StepManager.NextId + 189) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 178) + " = LINE('',#" + (StepManager.NextId + 179) + ",#" + (StepManager.NextId + 180) + ");",
                @"#" + (StepManager.NextId + 179) + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + (StepManager.NextId + 180) + " = VECTOR('',#" + (StepManager.NextId + 181) + ",1.);",
                @"#" + (StepManager.NextId + 181) + " = DIRECTION('',(0.,1.,0.));",
                @"#" + (StepManager.NextId + 182) + " = PCURVE('',#" + (StepManager.NextId + 155) + ",#" + (StepManager.NextId + 183) + ");",
                @"#" + (StepManager.NextId + 183) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 184) + "),#" + (StepManager.NextId + 188) + ");",
                @"#" + (StepManager.NextId + 184) + " = LINE('',#" + (StepManager.NextId + 185) + ",#" + (StepManager.NextId + 186) + ");",
                @"#" + (StepManager.NextId + 185) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 186) + " = VECTOR('',#" + (StepManager.NextId + 187) + ",1.);",
                @"#" + (StepManager.NextId + 187) + " = DIRECTION('',(0.,-1.));",
                @"#" + (StepManager.NextId + 188) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 189) + " = PCURVE('',#" + (StepManager.NextId + 75) + ",#" + (StepManager.NextId + 190) + ");",
                @"#" + (StepManager.NextId + 190) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 191) + "),#" + (StepManager.NextId + 195) + ");",
                @"#" + (StepManager.NextId + 191) + " = LINE('',#" + (StepManager.NextId + 192) + ",#" + (StepManager.NextId + 193) + ");",
                @"#" + (StepManager.NextId + 192) + " = CARTESIAN_POINT('',(" + Scale.XString + ",0.));",
                @"#" + (StepManager.NextId + 193) + " = VECTOR('',#" + (StepManager.NextId + 194) + ",1.);",
                @"#" + (StepManager.NextId + 194) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 195) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 196) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 197) + ",.T.);",
                @"#" + (StepManager.NextId + 197) + " = EDGE_CURVE('',#" + (StepManager.NextId + 175) + ",#" + (StepManager.NextId + 198) + ",#" + (StepManager.NextId + 200) + ",.T.);",
                @"#" + (StepManager.NextId + 198) + " = VERTEX_POINT('',#" + (StepManager.NextId + 199) + ");",
                @"#" + (StepManager.NextId + 199) + " = CARTESIAN_POINT('',(" + GetGPointX() + "," + GetGPointY() + "," + GetGPointZ() + "));",
                @"#" + (StepManager.NextId + 200) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 201) + ",(#" + (StepManager.NextId + 205) + ",#" + (StepManager.NextId + 212) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 201) + " = LINE('',#" + (StepManager.NextId + 202) + ",#" + (StepManager.NextId + 203) + ");",
                @"#" + (StepManager.NextId + 202) + " = CARTESIAN_POINT('',(" + GetHPointX() + "," + GetHPointY() + "," + GetHPointZ() + "));",
                @"#" + (StepManager.NextId + 203) + " = VECTOR('',#" + (StepManager.NextId + 204) + ",1.);",
                @"#" + (StepManager.NextId + 204) + " = DIRECTION('',(0.,0.,1.));",
                @"#" + (StepManager.NextId + 205) + " = PCURVE('',#" + (StepManager.NextId + 155) + ",#" + (StepManager.NextId + 206) + ");",
                @"#" + (StepManager.NextId + 206) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 207) + "),#" + (StepManager.NextId + 211) + ");",
                @"#" + (StepManager.NextId + 207) + " = LINE('',#" + (StepManager.NextId + 208) + ",#" + (StepManager.NextId + 209) + ");",
                @"#" + (StepManager.NextId + 208) + " = CARTESIAN_POINT('',(0.,-" + Scale.YString + "));",
                @"#" + (StepManager.NextId + 209) + " = VECTOR('',#" + (StepManager.NextId + 210) + ",1.);",
                @"#" + (StepManager.NextId + 210) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 211) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 212) + " = PCURVE('',#" + (StepManager.NextId + 103) + ",#" + (StepManager.NextId + 213) + ");",
                @"#" + (StepManager.NextId + 213) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 214) + "),#" + (StepManager.NextId + 218) + ");",
                @"#" + (StepManager.NextId + 214) + " = LINE('',#" + (StepManager.NextId + 215) + ",#" + (StepManager.NextId + 216) + ");",
                @"#" + (StepManager.NextId + 215) + " = CARTESIAN_POINT('',(0.," + Scale.XString + "));",
                @"#" + (StepManager.NextId + 216) + " = VECTOR('',#" + (StepManager.NextId + 217) + ",1.);",
                @"#" + (StepManager.NextId + 217) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 218) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 219) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 220) + ",.F.);",
                @"#" + (StepManager.NextId + 220) + " = EDGE_CURVE('',#" + (StepManager.NextId + 147) + ",#" + (StepManager.NextId + 198) + ",#" + (StepManager.NextId + 221) + ",.T.);",
                @"#" + (StepManager.NextId + 221) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 222) + ",(#" + (StepManager.NextId + 226) + ",#" + (StepManager.NextId + 233) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 222) + " = LINE('',#" + (StepManager.NextId + 223) + ",#" + (StepManager.NextId + 224) + ");",
                @"#" + (StepManager.NextId + 223) + " = CARTESIAN_POINT('',(" + GetCPointX() + "," + GetCPointY() + "," + GetCPointZ() + "));",
                @"#" + (StepManager.NextId + 224) + " = VECTOR('',#" + (StepManager.NextId + 225) + ",1.);",
                @"#" + (StepManager.NextId + 225) + " = DIRECTION('',(0.,1.,0.));",
                @"#" + (StepManager.NextId + 226) + " = PCURVE('',#" + (StepManager.NextId + 155) + ",#" + (StepManager.NextId + 227) + ");",
                @"#" + (StepManager.NextId + 227) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 228) + "),#" + (StepManager.NextId + 232) + ");",
                @"#" + (StepManager.NextId + 228) + " = LINE('',#" + (StepManager.NextId + 229) + ",#" + (StepManager.NextId + 230) + ");",
                @"#" + (StepManager.NextId + 229) + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + (StepManager.NextId + 230) + " = VECTOR('',#" + (StepManager.NextId + 231) + ",1.);",
                @"#" + (StepManager.NextId + 231) + " = DIRECTION('',(0.,-1.));",
                @"#" + (StepManager.NextId + 232) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 233) + " = PCURVE('',#" + (StepManager.NextId + 129) + ",#" + (StepManager.NextId + 234) + ");",
                @"#" + (StepManager.NextId + 234) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 235) + "),#" + (StepManager.NextId + 239) + ");",
                @"#" + (StepManager.NextId + 235) + " = LINE('',#" + (StepManager.NextId + 236) + ",#" + (StepManager.NextId + 237) + ");",
                @"#" + (StepManager.NextId + 236) + " = CARTESIAN_POINT('',(" + Scale.XString + ",0.));",
                @"#" + (StepManager.NextId + 237) + " = VECTOR('',#" + (StepManager.NextId + 238) + ",1.);",
                @"#" + (StepManager.NextId + 238) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 239) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 240) + " = ADVANCED_FACE('',(#" + (StepManager.NextId + 241) + "),#" + (StepManager.NextId + 47) + ",.F.);",
                @"#" + (StepManager.NextId + 241) + " = FACE_BOUND('',#" + (StepManager.NextId + 242) + ",.F.);",
                @"#" + (StepManager.NextId + 242) + " = EDGE_LOOP('',(#" + (StepManager.NextId + 243) + ",#" + (StepManager.NextId + 264) + ",#" + (StepManager.NextId + 265) + ",#" + (StepManager.NextId + 286) + "));",
                @"#" + (StepManager.NextId + 243) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 244) + ",.F.);",
                @"#" + (StepManager.NextId + 244) + " = EDGE_CURVE('',#" + (StepManager.NextId + 25) + ",#" + (StepManager.NextId + 145) + ",#" + (StepManager.NextId + 245) + ",.T.);",
                @"#" + (StepManager.NextId + 245) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 246) + ",(#" + (StepManager.NextId + 250) + ",#" + (StepManager.NextId + 257) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 246) + " = LINE('',#" + (StepManager.NextId + 247) + ",#" + (StepManager.NextId + 248) + ");",
                @"#" + (StepManager.NextId + 247) + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + (StepManager.NextId + 248) + " = VECTOR('',#" + (StepManager.NextId + 249) + ",1.);",
                @"#" + (StepManager.NextId + 249) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 250) + " = PCURVE('',#" + (StepManager.NextId + 47) + ",#" + (StepManager.NextId + 251) + ");",
                @"#" + (StepManager.NextId + 251) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 252) + "),#" + (StepManager.NextId + 256) + ");",
                @"#" + (StepManager.NextId + 252) + " = LINE('',#" + (StepManager.NextId + 253) + ",#" + (StepManager.NextId + 254) + ");",
                @"#" + (StepManager.NextId + 253) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 254) + " = VECTOR('',#" + (StepManager.NextId + 255) + ",1.);",
                @"#" + (StepManager.NextId + 255) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 256) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 257) + " = PCURVE('',#" + (StepManager.NextId + 75) + ",#" + (StepManager.NextId + 258) + ");",
                @"#" + (StepManager.NextId + 258) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 259) + "),#" + (StepManager.NextId + 263) + ");",
                @"#" + (StepManager.NextId + 259) + " = LINE('',#" + (StepManager.NextId + 260) + ",#" + (StepManager.NextId + 261) + ");",
                @"#" + (StepManager.NextId + 260) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 261) + " = VECTOR('',#" + (StepManager.NextId + 262) + ",1.);",
                @"#" + (StepManager.NextId + 262) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 263) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 264) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 24) + ",.T.);",
                @"#" + (StepManager.NextId + 265) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 266) + ",.T.);",
                @"#" + (StepManager.NextId + 266) + " = EDGE_CURVE('',#" + (StepManager.NextId + 27) + ",#" + (StepManager.NextId + 147) + ",#" + (StepManager.NextId + 267) + ",.T.);",
                @"#" + (StepManager.NextId + 267) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 268) + ",(#" + (StepManager.NextId + 272) + ",#" + (StepManager.NextId + 279) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 268) + " = LINE('',#" + (StepManager.NextId + 269) + ",#" + (StepManager.NextId + 270) + ");",
                @"#" + (StepManager.NextId + 269) + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + (StepManager.NextId + 270) + " = VECTOR('',#" + (StepManager.NextId + 271) + ",1.);",
                @"#" + (StepManager.NextId + 271) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 272) + " = PCURVE('',#" + (StepManager.NextId + 47) + ",#" + (StepManager.NextId + 273) + ");",
                @"#" + (StepManager.NextId + 273) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 274) + "),#" + (StepManager.NextId + 278) + ");",
                @"#" + (StepManager.NextId + 274) + " = LINE('',#" + (StepManager.NextId + 275) + ",#" + (StepManager.NextId + 276) + ");",
                @"#" + (StepManager.NextId + 275) + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + (StepManager.NextId + 276) + " = VECTOR('',#" + (StepManager.NextId + 277) + ",1.);",
                @"#" + (StepManager.NextId + 277) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 278) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 279) + " = PCURVE('',#" + (StepManager.NextId + 129) + ",#" + (StepManager.NextId + 280) + ");",
                @"#" + (StepManager.NextId + 280) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 281) + "),#" + (StepManager.NextId + 285) + ");",
                @"#" + (StepManager.NextId + 281) + " = LINE('',#" + (StepManager.NextId + 282) + ",#" + (StepManager.NextId + 283) + ");",
                @"#" + (StepManager.NextId + 282) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 283) + " = VECTOR('',#" + (StepManager.NextId + 284) + ",1.);",
                @"#" + (StepManager.NextId + 284) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 285) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 286) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 144) + ",.F.);",
                @"#" + (StepManager.NextId + 287) + " = ADVANCED_FACE('',(#" + (StepManager.NextId + 288) + "),#" + (StepManager.NextId + 103) + ",.T.);",
                @"#" + (StepManager.NextId + 288) + " = FACE_BOUND('',#" + (StepManager.NextId + 289) + ",.T.);",
                @"#" + (StepManager.NextId + 289) + " = EDGE_LOOP('',(#" + (StepManager.NextId + 290) + ",#" + (StepManager.NextId + 311) + ",#" + (StepManager.NextId + 312) + ",#" + (StepManager.NextId + 333) + "));",
                @"#" + (StepManager.NextId + 290) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 291) + ",.F.);",
                @"#" + (StepManager.NextId + 291) + " = EDGE_CURVE('',#" + (StepManager.NextId + 60) + ",#" + (StepManager.NextId + 175) + ",#" + (StepManager.NextId + 292) + ",.T.);",
                @"#" + (StepManager.NextId + 292) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 293) + ",(#" + (StepManager.NextId + 297) + ",#" + (StepManager.NextId + 304) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 293) + " = LINE('',#" + (StepManager.NextId + 294) + ",#" + (StepManager.NextId + 295) + ");",
                @"#" + (StepManager.NextId + 294) + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," + GetEPointZ() + "));",
                @"#" + (StepManager.NextId + 295) + " = VECTOR('',#" + (StepManager.NextId + 296) + ",1.);",
                @"#" + (StepManager.NextId + 296) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 297) + " = PCURVE('',#" + (StepManager.NextId + 103) + ",#" + (StepManager.NextId + 298) + ");",
                @"#" + (StepManager.NextId + 298) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 299) + "),#" + (StepManager.NextId + 303) + ");",
                @"#" + (StepManager.NextId + 299) + " = LINE('',#" + (StepManager.NextId + 300) + ",#" + (StepManager.NextId + 301) + ");",
                @"#" + (StepManager.NextId + 300) + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + (StepManager.NextId + 301) + " = VECTOR('',#" + (StepManager.NextId + 302) + ",1.);",
                @"#" + (StepManager.NextId + 302) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 303) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 304) + " = PCURVE('',#" + (StepManager.NextId + 75) + ",#" + (StepManager.NextId + 305) + ");",
                @"#" + (StepManager.NextId + 305) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 306) + "),#" + (StepManager.NextId + 310) + ");",
                @"#" + (StepManager.NextId + 306) + " = LINE('',#" + (StepManager.NextId + 307) + ",#" + (StepManager.NextId + 308) + ");",
                @"#" + (StepManager.NextId + 307) + " = CARTESIAN_POINT('',(0.," + Scale.YString + "));",
                @"#" + (StepManager.NextId + 308) + " = VECTOR('',#" + (StepManager.NextId + 309) + ",1.);",
                @"#" + (StepManager.NextId + 309) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 310) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 311) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 87) + ",.T.);",
                @"#" + (StepManager.NextId + 312) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 313) + ",.T.);",
                @"#" + (StepManager.NextId + 313) + " = EDGE_CURVE('',#" + (StepManager.NextId + 88) + ",#" + (StepManager.NextId + 198) + ",#" + (StepManager.NextId + 314) + ",.T.);",
                @"#" + (StepManager.NextId + 314) + " = SURFACE_CURVE('',#" + (StepManager.NextId + 315) + ",(#" + (StepManager.NextId + 319) + ",#" + (StepManager.NextId + 326) + "),.PCURVE_S1.);",
                @"#" + (StepManager.NextId + 315) + " = LINE('',#" + (StepManager.NextId + 316) + ",#" + (StepManager.NextId + 317) + ");",
                @"#" + (StepManager.NextId + 316) + " = CARTESIAN_POINT('',(" + GetFPointX() + "," + GetFPointY() + "," + GetFPointZ() + "));",
                @"#" + (StepManager.NextId + 317) + " = VECTOR('',#" + (StepManager.NextId + 318) + ",1.);",
                @"#" + (StepManager.NextId + 318) + " = DIRECTION('',(1.,0.,0.));",
                @"#" + (StepManager.NextId + 319) + " = PCURVE('',#" + (StepManager.NextId + 103) + ",#" + (StepManager.NextId + 320) + ");",
                @"#" + (StepManager.NextId + 320) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 321) + "),#" + (StepManager.NextId + 325) + ");",
                @"#" + (StepManager.NextId + 321) + " = LINE('',#" + (StepManager.NextId + 322) + ",#" + (StepManager.NextId + 323) + ");",
                @"#" + (StepManager.NextId + 322) + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + (StepManager.NextId + 323) + " = VECTOR('',#" + (StepManager.NextId + 324) + ",1.);",
                @"#" + (StepManager.NextId + 324) + " = DIRECTION('',(0.,1.));",
                @"#" + (StepManager.NextId + 325) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 326) + " = PCURVE('',#" + (StepManager.NextId + 129) + ",#" + (StepManager.NextId + 327) + ");",
                @"#" + (StepManager.NextId + 327) + " = DEFINITIONAL_REPRESENTATION('',(#" + (StepManager.NextId + 328) + "),#" + (StepManager.NextId + 332) + ");",
                @"#" + (StepManager.NextId + 328) + " = LINE('',#" + (StepManager.NextId + 329) + ",#" + (StepManager.NextId + 330) + ");",
                @"#" + (StepManager.NextId + 329) + " = CARTESIAN_POINT('',(0.," + Scale.YString + "));",
                @"#" + (StepManager.NextId + 330) + " = VECTOR('',#" + (StepManager.NextId + 331) + ",1.);",
                @"#" + (StepManager.NextId + 331) + " = DIRECTION('',(1.,0.));",
                @"#" + (StepManager.NextId + 332) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + (StepManager.NextId + 333) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 197) + ",.F.);",
                @"#" + (StepManager.NextId + 334) + " = ADVANCED_FACE('',(#" + (StepManager.NextId + 335) + "),#" + (StepManager.NextId + 75) + ",.F.);",
                @"#" + (StepManager.NextId + 335) + " = FACE_BOUND('',#" + (StepManager.NextId + 336) + ",.F.);",
                @"#" + (StepManager.NextId + 336) + " = EDGE_LOOP('',(#" + (StepManager.NextId + 337) + ",#" + (StepManager.NextId + 338) + ",#" + (StepManager.NextId + 339) + ",#" + (StepManager.NextId + 340) + "));",
                @"#" + (StepManager.NextId + 337) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 59) + ",.F.);",
                @"#" + (StepManager.NextId + 338) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 244) + ",.T.);",
                @"#" + (StepManager.NextId + 339) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 174) + ",.T.);",
                @"#" + (StepManager.NextId + 340) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 291) + ",.F.);",
                @"#" + (StepManager.NextId + 341) + " = ADVANCED_FACE('',(#" + (StepManager.NextId + 342) + "),#" + (StepManager.NextId + 129) + ",.T.);",
                @"#" + (StepManager.NextId + 342) + " = FACE_BOUND('',#" + (StepManager.NextId + 343) + ",.T.);",
                @"#" + (StepManager.NextId + 343) + " = EDGE_LOOP('',(#" + (StepManager.NextId + 344) + ",#" + (StepManager.NextId + 345) + ",#" + (StepManager.NextId + 346) + ",#" + (StepManager.NextId + 347) + "));",
                @"#" + (StepManager.NextId + 344) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 115) + ",.F.);",
                @"#" + (StepManager.NextId + 345) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 266) + ",.T.);",
                @"#" + (StepManager.NextId + 346) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 220) + ",.T.);",
                @"#" + (StepManager.NextId + 347) + " = ORIENTED_EDGE('',*,*,#" + (StepManager.NextId + 313) + ",.F.);",
                // geometry section end & solid info start
                @"#" + (StepManager.NextId + 348) + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 352) + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT((#" + (StepManager.NextId + 349) + ",#" + (StepManager.NextId + 350) + ",#" + (StepManager.NextId + 351) + ")) REPRESENTATION_CONTEXT('Context #1','3D Context with UNIT and UNCERTAINTY')); ",
                @"#" + (StepManager.NextId + 349) + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + (StepManager.NextId + 350) + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + (StepManager.NextId + 351) + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + (StepManager.NextId + 352) + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + (StepManager.NextId + 349) + ",'distance_accuracy_value','confusion accuracy'); ",
                @"#" + (StepManager.NextId + 353) + " = SHAPE_DEFINITION_REPRESENTATION(#" + (StepManager.NextId + 354) + ",#" + (StepManager.NextId + 17) + ");",
                @"#" + (StepManager.NextId + 354) + " = PRODUCT_DEFINITION_SHAPE('','',#" + (StepManager.NextId + 355) + ");",
                @"#" + (StepManager.NextId + 355) + " = PRODUCT_DEFINITION('design','',#" + (StepManager.NextId + 356) + ",#" + (StepManager.NextId + 359) + ");",
                @"#" + (StepManager.NextId + 356) + " = PRODUCT_DEFINITION_FORMATION('','',#" + (StepManager.NextId + 357) + ");",
                @"#" + (StepManager.NextId + 357) + " = PRODUCT('" + Name + "_box_solid" + "','" + Name + "_box_solid" + "','',(#" + (StepManager.NextId + 358) + "));",
                @"#" + (StepManager.NextId + 358) + " = PRODUCT_CONTEXT('',#2,'mechanical');",
                @"#" + (StepManager.NextId + 359) + " = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');",
                @"#" + (StepManager.NextId + 360) + " = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#" + (StepManager.NextId + 361) + ",#" + (StepManager.NextId + 363) + ");",
                @"#" + (StepManager.NextId + 361) + " = ( REPRESENTATION_RELATIONSHIP('','',#" + (StepManager.NextId + 17) + ",#" + (StepManager.NextId + 7) + ") REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#" + (StepManager.NextId + 362) + ") SHAPE_REPRESENTATION_RELATIONSHIP());",
                @"#" + (StepManager.NextId + 362) + " = ITEM_DEFINED_TRANSFORMATION('','',#11,#" + (StepManager.NextId + 8) + ");",
                @"#" + (StepManager.NextId + 363) + " = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#" + (StepManager.NextId + 364) + ");",
                @"#" + (StepManager.NextId + 364) + " = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + objectIndex + "','=>[0:1:1:" + objectIndex + "]','',#" + (StepManager.NextId + 2) + ",#" + (StepManager.NextId + 355) + ",$);",
                @"#" + (StepManager.NextId + 365) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#" + (StepManager.NextId + 357) + "));",
                @"#" + (StepManager.NextId + 366) + " = CONTEXT_DEPENDENT_SHAPE_REPRESENTATION(#" + (StepManager.NextId + 367) + ",#" + (StepManager.NextId + 369) + ");",
                @"#" + (StepManager.NextId + 367) + " = ( REPRESENTATION_RELATIONSHIP('','',#" + (StepManager.NextId + 7) + ",#10) REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION(#" + (StepManager.NextId + 368) + ") SHAPE_REPRESENTATION_RELATIONSHIP());",
                @"#" + (StepManager.NextId + 368) + " = ITEM_DEFINED_TRANSFORMATION('','',#11,#" + partCoordinateSystemId + ");",
                @"#" + (StepManager.NextId + 369) + " = PRODUCT_DEFINITION_SHAPE('Placement','Placement of an item',#" + (StepManager.NextId + 370) + ");",
                @"#" + (StepManager.NextId + 370) + " = NEXT_ASSEMBLY_USAGE_OCCURRENCE('" + (objectIndex + 1) + "','=>[0:1:1:" + (objectIndex + 1) + "]','',#5,#" + (StepManager.NextId + 2) + ",$);",
                @"#" + (StepManager.NextId + 371) + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#" + (StepManager.NextId + 4) + "));",
                @"#" + (StepManager.NextId + 372) + " = MECHANICAL_DESIGN_GEOMETRIC_PRESENTATION_REPRESENTATION('',(#" + (StepManager.NextId + 373) + "),#" + (StepManager.NextId + 348) + ");",
                // material section start
                @"#" + (StepManager.NextId + 373) + " = STYLED_ITEM('color',(#" + (StepManager.NextId + 374) + "),#" + (StepManager.NextId + 18) + ");",
                @"#" + (StepManager.NextId + 374) + " = PRESENTATION_STYLE_ASSIGNMENT((#" + (StepManager.NextId + 375) + ",#" + (StepManager.NextId + 381) + "));",
                @"#" + (StepManager.NextId + 375) + " = SURFACE_STYLE_USAGE(.BOTH.,#" + (StepManager.NextId + 376) + ");",
                @"#" + (StepManager.NextId + 376) + " = SURFACE_SIDE_STYLE('',(#" + (StepManager.NextId + 377) + "));",
                @"#" + (StepManager.NextId + 377) + " = SURFACE_STYLE_FILL_AREA(#" + (StepManager.NextId + 378) + ");",
                @"#" + (StepManager.NextId + 378) + " = FILL_AREA_STYLE('',(#" + (StepManager.NextId + 379) + "));",
                @"#" + (StepManager.NextId + 379) + " = FILL_AREA_STYLE_COLOUR('',#" + (StepManager.NextId + 380) + ");",
                @"#" + (StepManager.NextId + 380) + " = COLOUR_RGB(''," + Color.R.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "," + Color.G.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "," + Color.B.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + ");",
                @"#" + (StepManager.NextId + 381) + " = CURVE_STYLE('',#" + (StepManager.NextId + 382) + ",POSITIVE_LENGTH_MEASURE(0.1),#" + (StepManager.NextId + 380) + ");",
                @"#" + (StepManager.NextId + 382) + " = DRAUGHTING_PRE_DEFINED_CURVE_FONT('continuous');"
            };

            StepManager.NextId = (StepManager.NextId + 383);

            return lines;
        }

        /// <summary>
        /// Create a new instance of a box with given parameters.
        /// </summary>
        /// <param name="name">Name of the box that is visible in the CAD hierarchy.</param>
        /// <param name="center">Box position (center) in world space.</param>
        /// <param name="dimension">Box dimension (it's the complete length of an edge, not half of it)</param>
        /// <param name="rotation">Box rotation (around the provided position).</param>
        /// <param name="color">Box color. Transparency not supported yet.</param>
        public Box(IStepManager stepManager, string name, Vector3 center, Vector3 dimension, Vector3 rotation, Color color)
        {
            this.StepManager = stepManager;
            this.Name = name;
            this.Center = center;
            this.Scale = dimension;
            this.Rotation = rotation;
            this.Color = color;
        }

        #region Methods that provide the correct string for each point of the box

        // A

        public string GetAPointX()
        {
            return pointA.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);           
        }

        public string GetAPointY()
        {
            return pointA.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetAPointZ()
        {
            return pointA.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        // B

        public string GetBPointX()
        {
            return pointB.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetBPointY()
        {
            return pointB.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetBPointZ()
        {
            return pointB.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        // C

        public string GetCPointX()
        {
            return pointC.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetCPointY()
        {
            return pointC.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetCPointZ()
        {
            return pointC.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        // D

        public string GetDPointX()
        {
            return pointD.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetDPointY()
        {
            return pointD.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetDPointZ()
        {
            return pointD.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        // E

        public string GetEPointX()
        {
            return pointE.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetEPointY()
        {
            return pointE.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetEPointZ()
        {
            return pointE.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        // F

        public string GetFPointX()
        {
            return pointF.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetFPointY()
        {
            return pointF.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetFPointZ()
        {
            return pointF.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        // G

        public string GetGPointX()
        {
            return pointG.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetGPointY()
        {
            return pointG.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetGPointZ()
        {
            return pointG.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        // H

        public string GetHPointX()
        {
            return pointH.X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetHPointY()
        {
            return pointH.Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);            
        }

        public string GetHPointZ()
        {
            return pointH.Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
        }
                
        #endregion

    }

}
