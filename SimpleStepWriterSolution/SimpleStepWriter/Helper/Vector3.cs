using System.Collections.Generic;

namespace SimpleStepWriter.Helper
{
    public class Vector3
    {
        public static Vector3 Zero { get; } = new Vector3(0f, 0f, 0f);
        public static Vector3 One { get; } = new Vector3(1f, 1f, 1f);
        
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public string XString
        {
            get
            {
                return X.ToString("0.000000000000000", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string YString
        {
            get
            {
                return Y.ToString("0.000000000000000", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string ZString
        {
            get
            {
                return Z.ToString("0.000000000000000", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public Vector3()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public override bool Equals(object obj)
        {
            var vector = obj as Vector3;
            return vector != null &&
                   X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z &&
                   XString == vector.XString &&
                   YString == vector.YString &&
                   ZString == vector.ZString;
        }

        public override int GetHashCode()
        {
            var hashCode = -818742009;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(XString);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(YString);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ZString);
            return hashCode;
        }

        public static Vector3 Add(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        public static Vector3 Sub(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        public static Vector3 Div(Vector3 vector1, double value)
        {
            return new Vector3(vector1.X / value, vector1.Y / value, vector1.Z / value);
        }

        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3
                    (
                        (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y),
                        (vector1.Z * vector2.X) - (vector1.X * vector2.Z),
                        (vector1.X * vector2.Y) - (vector1.Y * vector2.X)
                    );
        }

        public static double Dot(Vector3 vector1, Vector3 vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z));
        }

    }

}
