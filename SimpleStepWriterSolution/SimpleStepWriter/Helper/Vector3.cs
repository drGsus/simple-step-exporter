namespace SimpleStepWriter.Helper
{
    public class Vector3
    {
        public static Vector3 Zero { get; } = new Vector3(0f, 0f, 0f);
        public static Vector3 One { get; } = new Vector3(1f, 1f, 1f);
        
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public string XString
        {
            get
            {
                return X.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string YString
        {
            get
            {
                return Y.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string ZString
        {
            get
            {
                return Z.ToString("0.00000000", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public Vector3()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
                
        public static Vector3 Sub(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        public static Vector3 Div(Vector3 vector1, float value)
        {
            return new Vector3(vector1.X / value, vector1.Y / value, vector1.Z / value);
        }

    }

}
