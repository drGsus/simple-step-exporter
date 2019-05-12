using System.Collections.Generic;
using System.Globalization;

namespace SimpleStepWriter.Helper
{
    public sealed class Vector3
    {        
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public string XString
        {
            get
            {
                return X.ToString("0.000000000000000", CultureInfo.InvariantCulture);
            }
        }

        public string YString
        {
            get
            {
                return Y.ToString("0.000000000000000", CultureInfo.InvariantCulture);
            }
        }

        public string ZString
        {
            get
            {
                return Z.ToString("0.000000000000000", CultureInfo.InvariantCulture);
            }
        }

        public Vector3()
        {
            X = 0.0d;
            Y = 0.0d;
            Z = 0.0d;
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

        public override string ToString()
        {
            return XString + " " + YString + " " + ZString;
        }

        public static Vector3 Zero { get; } = new Vector3(0d, 0d, 0d);
        public static Vector3 One { get; } = new Vector3(1d, 1d, 1d);

        internal static Vector3 Add(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        internal static Vector3 Sub(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        internal static Vector3 Div(Vector3 vector1, double scalar)
        {
            return new Vector3(vector1.X / scalar, vector1.Y / scalar, vector1.Z / scalar);
        }

        internal static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3
                    (
                        (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y),
                        (vector1.Z * vector2.X) - (vector1.X * vector2.Z),
                        (vector1.X * vector2.Y) - (vector1.Y * vector2.X)
                    );
        }

        internal static double Dot(Vector3 vector1, Vector3 vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z));
        }

    }

}
