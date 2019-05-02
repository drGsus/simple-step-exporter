using SimpleStepWriter.Helper;
using System;
using System.Collections.Generic;

namespace SimpleStepWriter.Content
{
    /// <summary> Box class representing a simple Box that has a size, position, rotation, color and name.
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
    class Box
    {
        // user provided values
        public string Name { get; private set; } = "DefaultName";
        public Vector3 Center { get; private set; } = Vector3.Zero;
        public Vector3 Scale { get; private set; } = Vector3.One;
        public Vector3 Rotation { get; private set; } = Vector3.Zero;
        public Color Color { get; private set; } = Color.White;
        
        // internally calculated values
        public long LinesAdded { get; private set; } = 0;
        public Vector3 Position { get; private set; } = Vector3.Zero;

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
        /// <param name="id">The next uniquw ID we can use for defining step data.</param>
        /// <returns>The content, line by line, that we append to the step file.</returns>
        public string[] GenerateDynamicBoxData(long id)
        {
            // mapping from relative box ID's to absolute STEP file ID's
            Dictionary<long, string> idLookup = new Dictionary<long, string>();
            for (long i = 0; i <= 358; i++)
            {
                idLookup.Add(i, (i + id).ToString());
                LinesAdded++;
            }
            
            // we calculate all other box points based on point A of the box
            Position = Vector3.Sub(Center, Vector3.Div(Scale, 2));

            // calculate all points for the box
            pointA = RotateMe(Position, Center);
            pointB = RotateMe(new Vector3(Position.X, Position.Y, Position.Z + Scale.Z), Center);
            pointC = RotateMe(new Vector3(Position.X + Scale.X, Position.Y, Position.Z + Scale.Z), Center);
            pointD = RotateMe(new Vector3(Position.X + Scale.X, Position.Y, Position.Z), Center);
            pointE = RotateMe(new Vector3(Position.X, Position.Y + Scale.Y, Position.Z), Center);
            pointF = RotateMe(new Vector3(Position.X, Position.Y + Scale.Y, Position.Z + Scale.Z), Center);
            pointG = RotateMe(new Vector3(Position.X + Scale.X, Position.Y + Scale.Y, Position.Z + Scale.Z), Center);
            pointH = RotateMe(new Vector3(Position.X + Scale.X, Position.Y + Scale.Y, Position.Z), Center);
            
            // return lines that represent a box that considers the previously provided information
            return new string[] {
                @"#" + idLookup[0] + " = SHAPE_DEFINITION_REPRESENTATION(#" + idLookup[1] + ",#" + idLookup[7] + ");",
                @"#" + idLookup[1] + " = PRODUCT_DEFINITION_SHAPE('','',#" + idLookup[2] + ");",
                @"#" + idLookup[2] + " = PRODUCT_DEFINITION('design','',#" + idLookup[3] + ",#" + idLookup[6] + ");",
                @"#" + idLookup[3] + " = PRODUCT_DEFINITION_FORMATION('','',#" + idLookup[4] + ");",
                @"#" + idLookup[4] + " = PRODUCT('" + Name + "','" + Name + "','',(#" + idLookup[5] + "));",
                @"#" + idLookup[5] + " = PRODUCT_CONTEXT('',#2,'mechanical');",                                                 
                @"#" + idLookup[6] + " = PRODUCT_DEFINITION_CONTEXT('part definition',#2,'design');",                     
                @"#" + idLookup[7] + " = ADVANCED_BREP_SHAPE_REPRESENTATION('',(#" + idLookup[8] + ",#" + idLookup[12] + "),#" + idLookup[342] + ");",
                @"#" + idLookup[8] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[9] + ",#" + idLookup[10] + ",#" + idLookup[11] + ");",
                @"#" + idLookup[9] + " = CARTESIAN_POINT('',(0.,0.,0.));",
                @"#" + idLookup[10] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[11] + " = DIRECTION('',(1.,0.,-0.));",
                @"#" + idLookup[12] + " = MANIFOLD_SOLID_BREP('',#" + idLookup[13] + ");",
                @"#" + idLookup[13] + " = CLOSED_SHELL('',(#" + idLookup[14] + ",#" + idLookup[134] + ",#" + idLookup[234] + ",#" + idLookup[281] + ",#" + idLookup[328] + ",#" + idLookup[335] + "));",
                @"#" + idLookup[14] + " = ADVANCED_FACE('',(#" + idLookup[15] + "),#" + idLookup[29] + ",.F.);",
                @"#" + idLookup[15] + " = FACE_BOUND('',#" + idLookup[16] + ",.F.);",
                @"#" + idLookup[16] + " = EDGE_LOOP('',(#" + idLookup[17] + ",#" + idLookup[52] + ",#" + idLookup[80] + ",#" + idLookup[108] + "));",
                @"#" + idLookup[17] + " = ORIENTED_EDGE('',*,*,#" + idLookup[18] + ",.F.);",
                @"#" + idLookup[18] + " = EDGE_CURVE('',#" + idLookup[19] + ",#" + idLookup[21] + ",#" + idLookup[23] + ",.T.);",
                @"#" + idLookup[19] + " = VERTEX_POINT('',#" + idLookup[20] + ");",
                @"#" + idLookup[20] + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[21] + " = VERTEX_POINT('',#" + idLookup[22] + ");",
                @"#" + idLookup[22] + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + idLookup[23] + " = SURFACE_CURVE('',#" + idLookup[24] + ",(#" + idLookup[28] + ",#" + idLookup[40] + "),.PCURVE_S1.);",
                @"#" + idLookup[24] + " = LINE('',#" + idLookup[25] + ",#" + idLookup[26] + ");",
                @"#" + idLookup[25] + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[26] + " = VECTOR('',#" + idLookup[27] + ",1.);",
                @"#" + idLookup[27] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[28] + " = PCURVE('',#" + idLookup[29] + ",#" + idLookup[34] + ");",
                @"#" + idLookup[29] + " = PLANE('',#" + idLookup[30] + ");",
                @"#" + idLookup[30] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[31] + ",#" + idLookup[32] + ",#" + idLookup[33] + ");",
                @"#" + idLookup[31] + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[32] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[33] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[34] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[35] + "),#" + idLookup[39] + ");",
                @"#" + idLookup[35] + " = LINE('',#" + idLookup[36] + ",#" + idLookup[37] + ");",
                @"#" + idLookup[36] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[37] + " = VECTOR('',#" + idLookup[38] + ",1.);",
                @"#" + idLookup[38] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[39] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[40] + " = PCURVE('',#" + idLookup[41] + ",#" + idLookup[46] + ");",
                @"#" + idLookup[41] + " = PLANE('',#" + idLookup[42] + ");",
                @"#" + idLookup[42] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[43] + ",#" + idLookup[44] + ",#" + idLookup[45] + ");",
                @"#" + idLookup[43] + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[44] + " = DIRECTION('',(0.,1.,0.));",
                @"#" + idLookup[45] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[46] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[47] + "),#" + idLookup[51] + ");",
                @"#" + idLookup[47] + " = LINE('',#" + idLookup[48] + ",#" + idLookup[49] + ");",
                @"#" + idLookup[48] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[49] + " = VECTOR('',#" + idLookup[50] + ",1.);",
                @"#" + idLookup[50] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[51] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[52] + " = ORIENTED_EDGE('',*,*,#" + idLookup[53] + ",.T.);",
                @"#" + idLookup[53] + " = EDGE_CURVE('',#" + idLookup[19] + ",#" + idLookup[54] + ",#" + idLookup[56] + ",.T.);",
                @"#" + idLookup[54] + " = VERTEX_POINT('',#" + idLookup[55] + ");",
                @"#" + idLookup[55] + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," + GetEPointZ() + "));",
                @"#" + idLookup[56] + " = SURFACE_CURVE('',#" + idLookup[57] + ",(#" + idLookup[61] + ",#" + idLookup[68] + "),.PCURVE_S1.);",
                @"#" + idLookup[57] + " = LINE('',#" + idLookup[58] + ",#" + idLookup[59] + ");",
                @"#" + idLookup[58] + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[59] + " = VECTOR('',#" + idLookup[60] + ",1.);",
                @"#" + idLookup[60] + " = DIRECTION('',(0.,1.,0.));",
                @"#" + idLookup[61] + " = PCURVE('',#" + idLookup[29] + ",#" + idLookup[62] + ");",
                @"#" + idLookup[62] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[63] + "),#" + idLookup[67] + ");",
                @"#" + idLookup[63] + " = LINE('',#" + idLookup[64] + ",#" + idLookup[65] + ");",
                @"#" + idLookup[64] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[65] + " = VECTOR('',#" + idLookup[66] + ",1.);",
                @"#" + idLookup[66] + " = DIRECTION('',(0.,-1.));",
                @"#" + idLookup[67] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[68] + " = PCURVE('',#" + idLookup[69] + ",#" + idLookup[74] + ");",
                @"#" + idLookup[69] + " = PLANE('',#" + idLookup[70] + ");",
                @"#" + idLookup[70] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[71] + ",#" + idLookup[72] + ",#" + idLookup[73] + ");",
                @"#" + idLookup[71] + " = CARTESIAN_POINT('',(" +  GetAPointX() + "," +  GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[72] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[73] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[74] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[75] + "),#" + idLookup[79] + ");",
                @"#" + idLookup[75] + " = LINE('',#" + idLookup[76] + ",#" + idLookup[77] + ");",
                @"#" + idLookup[76] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[77] + " = VECTOR('',#" + idLookup[78] + ",1.);",
                @"#" + idLookup[78] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[79] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[80] + " = ORIENTED_EDGE('',*,*,#" + idLookup[81] + ",.T.);",
                @"#" + idLookup[81] + " = EDGE_CURVE('',#" + idLookup[54] + ",#" + idLookup[82] + ",#" + idLookup[84] + ",.T.);",
                @"#" + idLookup[82] + " = VERTEX_POINT('',#" + idLookup[83] + ");",
                @"#" + idLookup[83] + " = CARTESIAN_POINT('',(" + GetFPointX() + "," + GetFPointY() + "," + GetFPointZ() + "));",
                @"#" + idLookup[84] + " = SURFACE_CURVE('',#" + idLookup[85] + ",(#" + idLookup[89] + ",#" + idLookup[96] + "),.PCURVE_S1.);",
                @"#" + idLookup[85] + " = LINE('',#" + idLookup[86] + ",#" + idLookup[87] + ");",
                @"#" + idLookup[86] + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," +GetEPointZ() + "));",
                @"#" + idLookup[87] + " = VECTOR('',#" + idLookup[88] + ",1.);",
                @"#" + idLookup[88] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[89] + " = PCURVE('',#" + idLookup[29] + ",#" + idLookup[90] + ");",
                @"#" + idLookup[90] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[91] + "),#" + idLookup[95] + ");",
                @"#" + idLookup[91] + " = LINE('',#" + idLookup[92] + ",#" + idLookup[93] + ");",
                @"#" + idLookup[92] + " = CARTESIAN_POINT('',(0.,-" + Scale.YString + "));",
                @"#" + idLookup[93] + " = VECTOR('',#" + idLookup[94] + ",1.);",
                @"#" + idLookup[94] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[95] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[96] + " = PCURVE('',#" + idLookup[97] + ",#" + idLookup[102] + ");",
                @"#" + idLookup[97] + " = PLANE('',#" + idLookup[98] + ");",
                @"#" + idLookup[98] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[99] + ",#" + idLookup[100] + ",#" + idLookup[101] + ");",
                @"#" + idLookup[99] + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," +GetEPointZ() + "));",
                @"#" + idLookup[100] + " = DIRECTION('',(0.,1.,0.));",
                @"#" + idLookup[101] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[102] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[103] + "),#" + idLookup[107] + ");",
                @"#" + idLookup[103] + " = LINE('',#" + idLookup[104] + ",#" + idLookup[105] + ");",
                @"#" + idLookup[104] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[105] + " = VECTOR('',#" + idLookup[106] + ",1.);",
                @"#" + idLookup[106] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[107] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[108] + " = ORIENTED_EDGE('',*,*,#" + idLookup[109] + ",.F.);",
                @"#" + idLookup[109] + " = EDGE_CURVE('',#" + idLookup[21] + ",#" + idLookup[82] + ",#" + idLookup[110] + ",.T.);",
                @"#" + idLookup[110] + " = SURFACE_CURVE('',#" + idLookup[111] + ",(#" + idLookup[115] + ",#" + idLookup[122] + "),.PCURVE_S1.);",
                @"#" + idLookup[111] + " = LINE('',#" + idLookup[112] + ",#" + idLookup[113] + ");",
                @"#" + idLookup[112] + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + idLookup[113] + " = VECTOR('',#" + idLookup[114] + ",1.);",
                @"#" + idLookup[114] + " = DIRECTION('',(0.,1.,0.));",
                @"#" + idLookup[115] + " = PCURVE('',#" + idLookup[29] + ",#" + idLookup[116] + ");",
                @"#" + idLookup[116] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[117] + "),#" + idLookup[121] + ");",
                @"#" + idLookup[117] + " = LINE('',#" + idLookup[118] + ",#" + idLookup[119] + ");",
                @"#" + idLookup[118] + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + idLookup[119] + " = VECTOR('',#" + idLookup[120] + ",1.);",
                @"#" + idLookup[120] + " = DIRECTION('',(0.,-1.));",
                @"#" + idLookup[121] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[122] + " = PCURVE('',#" + idLookup[123] + ",#" + idLookup[128] + ");",
                @"#" + idLookup[123] + " = PLANE('',#" + idLookup[124] + ");",
                @"#" + idLookup[124] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[125] + ",#" + idLookup[126] + ",#" + idLookup[127] + ");",
                @"#" + idLookup[125] + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + idLookup[126] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[127] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[128] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[129] + "),#" + idLookup[133] + ");",
                @"#" + idLookup[129] + " = LINE('',#" + idLookup[130] + ",#" + idLookup[131] + ");",
                @"#" + idLookup[130] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[131] + " = VECTOR('',#" + idLookup[132] + ",1.);",
                @"#" + idLookup[132] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[133] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[134] + " = ADVANCED_FACE('',(#" + idLookup[135] + "),#" + idLookup[149] + ",.T.);",
                @"#" + idLookup[135] + " = FACE_BOUND('',#" + idLookup[136] + ",.T.);",
                @"#" + idLookup[136] + " = EDGE_LOOP('',(#" + idLookup[137] + ",#" + idLookup[167] + ",#" + idLookup[190] + ",#" + idLookup[213] + "));",
                @"#" + idLookup[137] + " = ORIENTED_EDGE('',*,*,#" + idLookup[138] + ",.F.);",
                @"#" + idLookup[138] + " = EDGE_CURVE('',#" + idLookup[139] + ",#" + idLookup[141] + ",#" + idLookup[143] + ",.T.);",
                @"#" + idLookup[139] + " = VERTEX_POINT('',#" + idLookup[140] + ");",
                @"#" + idLookup[140] + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + idLookup[141] + " = VERTEX_POINT('',#" + idLookup[142] + ");",
                @"#" + idLookup[142] + " = CARTESIAN_POINT('',(" + GetCPointX() + "," + GetCPointY() + "," + GetCPointZ() + "));",
                @"#" + idLookup[143] + " = SURFACE_CURVE('',#" + idLookup[144] + ",(#" + idLookup[148] + ",#" + idLookup[160] + "),.PCURVE_S1.);",
                @"#" + idLookup[144] + " = LINE('',#" + idLookup[145] + ",#" + idLookup[146] + ");",
                @"#" + idLookup[145] + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + idLookup[146] + " = VECTOR('',#" + idLookup[147] + ",1.);",
                @"#" + idLookup[147] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[148] + " = PCURVE('',#" + idLookup[149] + ",#" + idLookup[154] + ");",
                @"#" + idLookup[149] + " = PLANE('',#" + idLookup[150] + ");",
                @"#" + idLookup[150] + " = AXIS2_PLACEMENT_3D('',#" + idLookup[151] + ",#" + idLookup[152] + ",#" + idLookup[153] + ");",
                @"#" + idLookup[151] + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + idLookup[152] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[153] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[154] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[155] + "),#" + idLookup[159] + ");",
                @"#" + idLookup[155] + " = LINE('',#" + idLookup[156] + ",#" + idLookup[157] + ");",
                @"#" + idLookup[156] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[157] + " = VECTOR('',#" + idLookup[158] + ",1.);",
                @"#" + idLookup[158] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[159] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[160] + " = PCURVE('',#" + idLookup[41] + ",#" + idLookup[161] + ");",
                @"#" + idLookup[161] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[162] + "),#" + idLookup[166] + ");",
                @"#" + idLookup[162] + " = LINE('',#" + idLookup[163] + ",#" + idLookup[164] + ");",
                @"#" + idLookup[163] + " = CARTESIAN_POINT('',(0.," + Scale.XString + "));",
                @"#" + idLookup[164] + " = VECTOR('',#" + idLookup[165] + ",1.);",
                @"#" + idLookup[165] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[166] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[167] + " = ORIENTED_EDGE('',*,*,#" + idLookup[168] + ",.T.);",
                @"#" + idLookup[168] + " = EDGE_CURVE('',#" + idLookup[139] + ",#" + idLookup[169] + ",#" + idLookup[171] + ",.T.);",
                @"#" + idLookup[169] + " = VERTEX_POINT('',#" + idLookup[170] + ");",
                @"#" + idLookup[170] + " = CARTESIAN_POINT('',(" + GetHPointX() + "," + GetHPointY() + "," + GetHPointZ() + "));",
                @"#" + idLookup[171] + " = SURFACE_CURVE('',#" + idLookup[172] + ",(#" + idLookup[176] + ",#" + idLookup[183] + "),.PCURVE_S1.);",
                @"#" + idLookup[172] + " = LINE('',#" + idLookup[173] + ",#" + idLookup[174] + ");",
                @"#" + idLookup[173] + " = CARTESIAN_POINT('',(" + GetDPointX() + "," + GetDPointY() + "," + GetDPointZ() + "));",
                @"#" + idLookup[174] + " = VECTOR('',#" + idLookup[175] + ",1.);",
                @"#" + idLookup[175] + " = DIRECTION('',(0.,1.,0.));",
                @"#" + idLookup[176] + " = PCURVE('',#" + idLookup[149] + ",#" + idLookup[177] + ");",
                @"#" + idLookup[177] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[178] + "),#" + idLookup[182] + ");",
                @"#" + idLookup[178] + " = LINE('',#" + idLookup[179] + ",#" + idLookup[180] + ");",
                @"#" + idLookup[179] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[180] + " = VECTOR('',#" + idLookup[181] + ",1.);",
                @"#" + idLookup[181] + " = DIRECTION('',(0.,-1.));",
                @"#" + idLookup[182] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[183] + " = PCURVE('',#" + idLookup[69] + ",#" + idLookup[184] + ");",
                @"#" + idLookup[184] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[185] + "),#" + idLookup[189] + ");",
                @"#" + idLookup[185] + " = LINE('',#" + idLookup[186] + ",#" + idLookup[187] + ");",
                @"#" + idLookup[186] + " = CARTESIAN_POINT('',(" + Scale.XString + ",0.));",
                @"#" + idLookup[187] + " = VECTOR('',#" + idLookup[188] + ",1.);",
                @"#" + idLookup[188] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[189] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[190] + " = ORIENTED_EDGE('',*,*,#" + idLookup[191] + ",.T.);",
                @"#" + idLookup[191] + " = EDGE_CURVE('',#" + idLookup[169] + ",#" + idLookup[192] + ",#" + idLookup[194] + ",.T.);",
                @"#" + idLookup[192] + " = VERTEX_POINT('',#" + idLookup[193] + ");",
                @"#" + idLookup[193] + " = CARTESIAN_POINT('',(" + GetGPointX() + "," + GetGPointY() + "," + GetGPointZ() + "));",
                @"#" + idLookup[194] + " = SURFACE_CURVE('',#" + idLookup[195] + ",(#" + idLookup[199] + ",#" + idLookup[206] + "),.PCURVE_S1.);",
                @"#" + idLookup[195] + " = LINE('',#" + idLookup[196] + ",#" + idLookup[197] + ");",
                @"#" + idLookup[196] + " = CARTESIAN_POINT('',(" + GetHPointX() + "," + GetHPointY() + "," + GetHPointZ() + "));",
                @"#" + idLookup[197] + " = VECTOR('',#" + idLookup[198] + ",1.);",
                @"#" + idLookup[198] + " = DIRECTION('',(0.,0.,1.));",
                @"#" + idLookup[199] + " = PCURVE('',#" + idLookup[149] + ",#" + idLookup[200] + ");",
                @"#" + idLookup[200] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[201] + "),#" + idLookup[205] + ");",
                @"#" + idLookup[201] + " = LINE('',#" + idLookup[202] + ",#" + idLookup[203] + ");",
                @"#" + idLookup[202] + " = CARTESIAN_POINT('',(0.,-" + Scale.YString + "));",
                @"#" + idLookup[203] + " = VECTOR('',#" + idLookup[204] + ",1.);",
                @"#" + idLookup[204] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[205] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[206] + " = PCURVE('',#" + idLookup[97] + ",#" + idLookup[207] + ");",
                @"#" + idLookup[207] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[208] + "),#" + idLookup[212] + ");",
                @"#" + idLookup[208] + " = LINE('',#" + idLookup[209] + ",#" + idLookup[210] + ");",
                @"#" + idLookup[209] + " = CARTESIAN_POINT('',(0.," + Scale.XString + "));",
                @"#" + idLookup[210] + " = VECTOR('',#" + idLookup[211] + ",1.);",
                @"#" + idLookup[211] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[212] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[213] + " = ORIENTED_EDGE('',*,*,#" + idLookup[214] + ",.F.);",
                @"#" + idLookup[214] + " = EDGE_CURVE('',#" + idLookup[141] + ",#" + idLookup[192] + ",#" + idLookup[215] + ",.T.);",
                @"#" + idLookup[215] + " = SURFACE_CURVE('',#" + idLookup[216] + ",(#" + idLookup[220] + ",#" + idLookup[227] + "),.PCURVE_S1.);",
                @"#" + idLookup[216] + " = LINE('',#" + idLookup[217] + ",#" + idLookup[218] + ");",
                @"#" + idLookup[217] + " = CARTESIAN_POINT('',(" + GetCPointX() + "," + GetCPointY() + "," + GetCPointZ() + "));",
                @"#" + idLookup[218] + " = VECTOR('',#" + idLookup[219] + ",1.);",
                @"#" + idLookup[219] + " = DIRECTION('',(0.,1.,0.));",
                @"#" + idLookup[220] + " = PCURVE('',#" + idLookup[149] + ",#" + idLookup[221] + ");",
                @"#" + idLookup[221] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[222] + "),#" + idLookup[226] + ");",
                @"#" + idLookup[222] + " = LINE('',#" + idLookup[223] + ",#" + idLookup[224] + ");",
                @"#" + idLookup[223] + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + idLookup[224] + " = VECTOR('',#" + idLookup[225] + ",1.);",
                @"#" + idLookup[225] + " = DIRECTION('',(0.,-1.));",
                @"#" + idLookup[226] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[227] + " = PCURVE('',#" + idLookup[123] + ",#" + idLookup[228] + ");",
                @"#" + idLookup[228] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[229] + "),#" + idLookup[233] + ");",
                @"#" + idLookup[229] + " = LINE('',#" + idLookup[230] + ",#" + idLookup[231] + ");",
                @"#" + idLookup[230] + " = CARTESIAN_POINT('',(" + Scale.XString + ",0.));",
                @"#" + idLookup[231] + " = VECTOR('',#" + idLookup[232] + ",1.);",
                @"#" + idLookup[232] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[233] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[234] + " = ADVANCED_FACE('',(#" + idLookup[235] + "),#" + idLookup[41] + ",.F.);",
                @"#" + idLookup[235] + " = FACE_BOUND('',#" + idLookup[236] + ",.F.);",
                @"#" + idLookup[236] + " = EDGE_LOOP('',(#" + idLookup[237] + ",#" + idLookup[258] + ",#" + idLookup[259] + ",#" + idLookup[280] + "));",
                @"#" + idLookup[237] + " = ORIENTED_EDGE('',*,*,#" + idLookup[238] + ",.F.);",
                @"#" + idLookup[238] + " = EDGE_CURVE('',#" + idLookup[19] + ",#" + idLookup[139] + ",#" + idLookup[239] + ",.T.);",
                @"#" + idLookup[239] + " = SURFACE_CURVE('',#" + idLookup[240] + ",(#" + idLookup[244] + ",#" + idLookup[251] + "),.PCURVE_S1.);",
                @"#" + idLookup[240] + " = LINE('',#" + idLookup[241] + ",#" + idLookup[242] + ");",
                @"#" + idLookup[241] + " = CARTESIAN_POINT('',(" + GetAPointX() + "," + GetAPointY() + "," + GetAPointZ() + "));",
                @"#" + idLookup[242] + " = VECTOR('',#" + idLookup[243] + ",1.);",
                @"#" + idLookup[243] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[244] + " = PCURVE('',#" + idLookup[41] + ",#" + idLookup[245] + ");",
                @"#" + idLookup[245] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[246] + "),#" + idLookup[250] + ");",
                @"#" + idLookup[246] + " = LINE('',#" + idLookup[247] + ",#" + idLookup[248] + ");",
                @"#" + idLookup[247] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[248] + " = VECTOR('',#" + idLookup[249] + ",1.);",
                @"#" + idLookup[249] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[250] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[251] + " = PCURVE('',#" + idLookup[69] + ",#" + idLookup[252] + ");",
                @"#" + idLookup[252] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[253] + "),#" + idLookup[257] + ");",
                @"#" + idLookup[253] + " = LINE('',#" + idLookup[254] + ",#" + idLookup[255] + ");",
                @"#" + idLookup[254] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[255] + " = VECTOR('',#" + idLookup[256] + ",1.);",
                @"#" + idLookup[256] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[257] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[258] + " = ORIENTED_EDGE('',*,*,#" + idLookup[18] + ",.T.);",
                @"#" + idLookup[259] + " = ORIENTED_EDGE('',*,*,#" + idLookup[260] + ",.T.);",
                @"#" + idLookup[260] + " = EDGE_CURVE('',#" + idLookup[21] + ",#" + idLookup[141] + ",#" + idLookup[261] + ",.T.);",
                @"#" + idLookup[261] + " = SURFACE_CURVE('',#" + idLookup[262] + ",(#" + idLookup[266] + ",#" + idLookup[273] + "),.PCURVE_S1.);",
                @"#" + idLookup[262] + " = LINE('',#" + idLookup[263] + ",#" + idLookup[264] + ");",
                @"#" + idLookup[263] + " = CARTESIAN_POINT('',(" + GetBPointX() + "," + GetBPointY() + "," + GetBPointZ() + "));",
                @"#" + idLookup[264] + " = VECTOR('',#" + idLookup[265] + ",1.);",
                @"#" + idLookup[265] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[266] + " = PCURVE('',#" + idLookup[41] + ",#" + idLookup[267] + ");",
                @"#" + idLookup[267] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[268] + "),#" + idLookup[272] + ");",
                @"#" + idLookup[268] + " = LINE('',#" + idLookup[269] + ",#" + idLookup[270] + ");",
                @"#" + idLookup[269] + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + idLookup[270] + " = VECTOR('',#" + idLookup[271] + ",1.);",
                @"#" + idLookup[271] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[272] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[273] + " = PCURVE('',#" + idLookup[123] + ",#" + idLookup[274] + ");",
                @"#" + idLookup[274] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[275] + "),#" + idLookup[279] + ");",
                @"#" + idLookup[275] + " = LINE('',#" + idLookup[276] + ",#" + idLookup[277] + ");",
                @"#" + idLookup[276] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[277] + " = VECTOR('',#" + idLookup[278] + ",1.);",
                @"#" + idLookup[278] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[279] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[280] + " = ORIENTED_EDGE('',*,*,#" + idLookup[138] + ",.F.);",
                @"#" + idLookup[281] + " = ADVANCED_FACE('',(#" + idLookup[282] + "),#" + idLookup[97] + ",.T.);",
                @"#" + idLookup[282] + " = FACE_BOUND('',#" + idLookup[283] + ",.T.);",
                @"#" + idLookup[283] + " = EDGE_LOOP('',(#" + idLookup[284] + ",#" + idLookup[305] + ",#" + idLookup[306] + ",#" + idLookup[327] + "));",
                @"#" + idLookup[284] + " = ORIENTED_EDGE('',*,*,#" + idLookup[285] + ",.F.);",
                @"#" + idLookup[285] + " = EDGE_CURVE('',#" + idLookup[54] + ",#" + idLookup[169] + ",#" + idLookup[286] + ",.T.);",
                @"#" + idLookup[286] + " = SURFACE_CURVE('',#" + idLookup[287] + ",(#" + idLookup[291] + ",#" + idLookup[298] + "),.PCURVE_S1.);",
                @"#" + idLookup[287] + " = LINE('',#" + idLookup[288] + ",#" + idLookup[289] + ");",
                @"#" + idLookup[288] + " = CARTESIAN_POINT('',(" + GetEPointX() + "," + GetEPointY() + "," + GetEPointZ() + "));",
                @"#" + idLookup[289] + " = VECTOR('',#" + idLookup[290] + ",1.);",
                @"#" + idLookup[290] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[291] + " = PCURVE('',#" + idLookup[97] + ",#" + idLookup[292] + ");",
                @"#" + idLookup[292] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[293] + "),#" + idLookup[297] + ");",
                @"#" + idLookup[293] + " = LINE('',#" + idLookup[294] + ",#" + idLookup[295] + ");",
                @"#" + idLookup[294] + " = CARTESIAN_POINT('',(0.,0.));",
                @"#" + idLookup[295] + " = VECTOR('',#" + idLookup[296] + ",1.);",
                @"#" + idLookup[296] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[297] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[298] + " = PCURVE('',#" + idLookup[69] + ",#" + idLookup[299] + ");",
                @"#" + idLookup[299] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[300] + "),#" + idLookup[304] + ");",
                @"#" + idLookup[300] + " = LINE('',#" + idLookup[301] + ",#" + idLookup[302] + ");",
                @"#" + idLookup[301] + " = CARTESIAN_POINT('',(0.," + Scale.YString + "));",
                @"#" + idLookup[302] + " = VECTOR('',#" + idLookup[303] + ",1.);",
                @"#" + idLookup[303] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[304] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[305] + " = ORIENTED_EDGE('',*,*,#" + idLookup[81] + ",.T.);",
                @"#" + idLookup[306] + " = ORIENTED_EDGE('',*,*,#" + idLookup[307] + ",.T.);",
                @"#" + idLookup[307] + " = EDGE_CURVE('',#" + idLookup[82] + ",#" + idLookup[192] + ",#" + idLookup[308] + ",.T.);",
                @"#" + idLookup[308] + " = SURFACE_CURVE('',#" + idLookup[309] + ",(#" + idLookup[313] + ",#" + idLookup[320] + "),.PCURVE_S1.);",
                @"#" + idLookup[309] + " = LINE('',#" + idLookup[310] + ",#" + idLookup[311] + ");",
                @"#" + idLookup[310] + " = CARTESIAN_POINT('',(" + GetFPointX() + "," + GetFPointY() + "," + GetFPointZ() + "));",
                @"#" + idLookup[311] + " = VECTOR('',#" + idLookup[312] + ",1.);",
                @"#" + idLookup[312] + " = DIRECTION('',(1.,0.,0.));",
                @"#" + idLookup[313] + " = PCURVE('',#" + idLookup[97] + ",#" + idLookup[314] + ");",
                @"#" + idLookup[314] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[315] + "),#" + idLookup[319] + ");",
                @"#" + idLookup[315] + " = LINE('',#" + idLookup[316] + ",#" + idLookup[317] + ");",
                @"#" + idLookup[316] + " = CARTESIAN_POINT('',(" + Scale.ZString + ",0.));",
                @"#" + idLookup[317] + " = VECTOR('',#" + idLookup[318] + ",1.);",
                @"#" + idLookup[318] + " = DIRECTION('',(0.,1.));",
                @"#" + idLookup[319] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[320] + " = PCURVE('',#" + idLookup[123] + ",#" + idLookup[321] + ");",
                @"#" + idLookup[321] + " = DEFINITIONAL_REPRESENTATION('',(#" + idLookup[322] + "),#" + idLookup[326] + ");",
                @"#" + idLookup[322] + " = LINE('',#" + idLookup[323] + ",#" + idLookup[324] + ");",
                @"#" + idLookup[323] + " = CARTESIAN_POINT('',(0.," + Scale.YString + "));",
                @"#" + idLookup[324] + " = VECTOR('',#" + idLookup[325] + ",1.);",
                @"#" + idLookup[325] + " = DIRECTION('',(1.,0.));",
                @"#" + idLookup[326] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(2) PARAMETRIC_REPRESENTATION_CONTEXT() REPRESENTATION_CONTEXT('2D SPACE','') );",
                @"#" + idLookup[327] + " = ORIENTED_EDGE('',*,*,#" + idLookup[191] + ",.F.);",
                @"#" + idLookup[328] + " = ADVANCED_FACE('',(#" + idLookup[329] + "),#" + idLookup[69] + ",.F.);",
                @"#" + idLookup[329] + " = FACE_BOUND('',#" + idLookup[330] + ",.F.);",
                @"#" + idLookup[330] + " = EDGE_LOOP('',(#" + idLookup[331] + ",#" + idLookup[332] + ",#" + idLookup[333] + ",#" + idLookup[334] + "));",
                @"#" + idLookup[331] + " = ORIENTED_EDGE('',*,*,#" + idLookup[53] + ",.F.);",
                @"#" + idLookup[332] + " = ORIENTED_EDGE('',*,*,#" + idLookup[238] + ",.T.);",
                @"#" + idLookup[333] + " = ORIENTED_EDGE('',*,*,#" + idLookup[168] + ",.T.);",
                @"#" + idLookup[334] + " = ORIENTED_EDGE('',*,*,#" + idLookup[285] + ",.F.);",
                @"#" + idLookup[335] + " = ADVANCED_FACE('',(#" + idLookup[336] + "),#" + idLookup[123] + ",.T.);",
                @"#" + idLookup[336] + " = FACE_BOUND('',#" + idLookup[337] + ",.T.);",
                @"#" + idLookup[337] + " = EDGE_LOOP('',(#" + idLookup[338] + ",#" + idLookup[339] + ",#" + idLookup[340] + ",#" + idLookup[341] + "));",
                @"#" + idLookup[338] + " = ORIENTED_EDGE('',*,*,#" + idLookup[109] + ",.F.);",
                @"#" + idLookup[339] + " = ORIENTED_EDGE('',*,*,#" + idLookup[260] + ",.T.);",
                @"#" + idLookup[340] + " = ORIENTED_EDGE('',*,*,#" + idLookup[214] + ",.T.);",
                @"#" + idLookup[341] + " = ORIENTED_EDGE('',*,*,#" + idLookup[307] + ",.F.);",
                @"#" + idLookup[342] + " = ( GEOMETRIC_REPRESENTATION_CONTEXT(3) GLOBAL_UNCERTAINTY_ASSIGNED_CONTEXT((#" + idLookup[346] + ")) GLOBAL_UNIT_ASSIGNED_CONTEXT ((#" + idLookup[343] + ",#" + idLookup[344] + ",#" + idLookup[345] + ")) REPRESENTATION_CONTEXT('Context #1', '3D Context with UNIT and UNCERTAINTY') );",
                @"#" + idLookup[343] + " = ( LENGTH_UNIT() NAMED_UNIT(*) SI_UNIT(.MILLI.,.METRE.) );",
                @"#" + idLookup[344] + " = ( NAMED_UNIT(*) PLANE_ANGLE_UNIT() SI_UNIT($,.RADIAN.) );",
                @"#" + idLookup[345] + " = ( NAMED_UNIT(*) SI_UNIT($,.STERADIAN.) SOLID_ANGLE_UNIT() );",
                @"#" + idLookup[346] + " = UNCERTAINTY_MEASURE_WITH_UNIT(LENGTH_MEASURE(1.E-07),#" + idLookup[343] + ", 'distance_accuracy_value','confusion accuracy');",
                @"#" + idLookup[347] + " = PRODUCT_RELATED_PRODUCT_CATEGORY('part',$,(#" + idLookup[4] + "));",
                @"#" + idLookup[348] + " = MECHANICAL_DESIGN_GEOMETRIC_PRESENTATION_REPRESENTATION('',(#" + idLookup[349] + "),#" + idLookup[342] + ");",
                @"#" + idLookup[349] + " = STYLED_ITEM('color',(#" + idLookup[350] + "),#" + idLookup[12] + ");",
                @"#" + idLookup[350] + " = PRESENTATION_STYLE_ASSIGNMENT((#" + idLookup[351] + ",#" + idLookup[357] + "));",
                @"#" + idLookup[351] + " = SURFACE_STYLE_USAGE(.BOTH.,#" + idLookup[352] + ");",
                @"#" + idLookup[352] + " = SURFACE_SIDE_STYLE('',(#" + idLookup[353] + "));",
                @"#" + idLookup[353] + " = SURFACE_STYLE_FILL_AREA(#" + idLookup[354] + ");",
                @"#" + idLookup[354] + " = FILL_AREA_STYLE('',(#" + idLookup[355] + "));",
                @"#" + idLookup[355] + " = FILL_AREA_STYLE_COLOUR('',#" + idLookup[356] + ");",
                @"#" + idLookup[356] + " = COLOUR_RGB(''," + Color.R.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "," + Color.G.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + "," + Color.B.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture) + ");",
                @"#" + idLookup[357] + " = CURVE_STYLE('',#" + idLookup[358] + ",POSITIVE_LENGTH_MEASURE(0.1),#" + idLookup[356] + ");",
                @"#" + idLookup[358] + " = DRAUGHTING_PRE_DEFINED_CURVE_FONT('continuous');"
            };
        }

        /// <summary>
        /// Create a new instance of a box with given parameters.
        /// </summary>
        /// <param name="name">Name of the box that is visible in the CAD hierarchy.</param>
        /// <param name="center">Box position (center) in world space.</param>
        /// <param name="dimension">Box dimension (it's the complete length of an edge, not half of it)</param>
        /// <param name="rotation">Box rotation (around the provided position).</param>
        /// <param name="color">Box color. Transparency not supported yet.</param>
        public Box(string name, Vector3 center, Vector3 dimension, Vector3 rotation, Color color)
        {
            this.Name = name;
            this.Center = center;
            this.Scale = dimension;
            this.Rotation = rotation;
            this.Color = color;
        }

        /// <summary>
        /// Helper method for rotating a point around another point.
        /// https://www.gamefromscratch.com/post/2012/11/24/GameDev-math-recipes-Rotating-one-point-around-another-point.aspx
        /// </summary>
        /// <param name="point">The point you want to rotate.</param>
        /// <param name="center">The point you want to rotate around.</param>
        /// <returns>New rotated point.</returns>
        private Vector3 RotateMe(Vector3 point, Vector3 center)
        {
            double radianAngleX = (Rotation.X) * (Math.PI / 180);
            var rotatedY = Math.Cos(radianAngleX) * (point.Y - center.Y) - Math.Sin(radianAngleX) * (point.Z - center.Z) + center.Y;
            var rotatedZ = Math.Sin(radianAngleX) * (point.Y - center.Y) + Math.Cos(radianAngleX) * (point.Z - center.Z) + center.Z;

            double radianAngleY = (Rotation.Y) * (Math.PI / 180);
            var rotatedXX = Math.Cos(radianAngleY) * (point.X - center.X) - Math.Sin(radianAngleY) * (rotatedZ - center.Z) + center.X;
            var rotatedZZ = Math.Sin(radianAngleY) * (point.X - center.X) + Math.Cos(radianAngleY) * (rotatedZ - center.Z) + center.Z;

            double radianAngleZ = (Rotation.Z) * (Math.PI / 180);
            var rotatedXXX = Math.Cos(radianAngleZ) * (rotatedXX - center.X) - Math.Sin(radianAngleZ) * (rotatedY - center.Y) + center.X;
            var rotatedYYY = Math.Sin(radianAngleZ) * (rotatedXX - center.X) + Math.Cos(radianAngleZ) * (rotatedY - center.Y) + center.Y;

            return new Vector3((float)rotatedXXX, (float)rotatedYYY, (float)rotatedZZ);
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
