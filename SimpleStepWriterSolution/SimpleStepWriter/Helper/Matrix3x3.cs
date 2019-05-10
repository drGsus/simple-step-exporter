using System;
using System.Collections.Generic;

namespace SimpleStepWriter.Helper
{
    public class Matrix3x3
    {
        double[,] matrix;
        
        public double this[int i, int j]
        {
            get { return matrix[i, j]; }            
        }

        #region properties
        public double A11 { get { return matrix[0, 0]; } set { matrix[0, 0] = value; } }
        public double A12 { get { return matrix[0, 1]; } set { matrix[0, 1] = value; } }
        public double A13 { get { return matrix[0, 2]; } set { matrix[0, 2] = value; } }
        public double A21 { get { return matrix[1, 0]; } set { matrix[1, 0] = value; } }
        public double A22 { get { return matrix[1, 1]; } set { matrix[1, 1] = value; } }
        public double A23 { get { return matrix[1, 2]; } set { matrix[1, 2] = value; } }
        public double A31 { get { return matrix[2, 0]; } set { matrix[2, 0] = value; } }
        public double A32 { get { return matrix[2, 1]; } set { matrix[2, 1] = value; } }
        public double A33 { get { return matrix[2, 2]; } set { matrix[2, 2] = value; } }
        #endregion        

        public Matrix3x3()
        {
            matrix = new double[3, 3] {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
        }

        public Matrix3x3
        (
            double a11,
            double a12,
            double a13,
            double a21,
            double a22,
            double a23,
            double a31,
            double a32,
            double a33
        )
        {
            matrix = new double[3, 3] {
                { a11, a12, a13 },
                { a21, a22, a23 },
                { a31, a32, a33 }
            };
        }
       
        public override string ToString()
        {
            return
                (
                    "{{" + A11 + ", " + A12 + ", " + A13 + "},"
                    + " {" + A21 + ", " + A22 + ", " + A23 + "},"
                    + " {" + A31 + ", " + A32 + ", " + A33 + "}}"
                );
        }
        
        public override bool Equals(object obj)
        {
            var x = obj as Matrix3x3;
            return x != null &&
                   EqualityComparer<double[,]>.Default.Equals(matrix, x.matrix) &&
                   A11 == x.A11 &&
                   A12 == x.A12 &&
                   A13 == x.A13 &&
                   A21 == x.A21 &&
                   A22 == x.A22 &&
                   A23 == x.A23 &&
                   A31 == x.A31 &&
                   A32 == x.A32 &&
                   A33 == x.A33;
        }

        public override int GetHashCode()
        {
            var hashCode = -929212920;
            hashCode = hashCode * -1521134295 + EqualityComparer<double[,]>.Default.GetHashCode(matrix);
            hashCode = hashCode * -1521134295 + A11.GetHashCode();
            hashCode = hashCode * -1521134295 + A12.GetHashCode();
            hashCode = hashCode * -1521134295 + A13.GetHashCode();
            hashCode = hashCode * -1521134295 + A21.GetHashCode();
            hashCode = hashCode * -1521134295 + A22.GetHashCode();
            hashCode = hashCode * -1521134295 + A23.GetHashCode();
            hashCode = hashCode * -1521134295 + A31.GetHashCode();
            hashCode = hashCode * -1521134295 + A32.GetHashCode();
            hashCode = hashCode * -1521134295 + A33.GetHashCode();
            return hashCode;
        }

        public static Matrix3x3 Add(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            return new Matrix3x3
                    (
                        matrix1.A11 + matrix2.A11,
                        matrix1.A12 + matrix2.A12,
                        matrix1.A13 + matrix2.A13,
                        matrix1.A21 + matrix2.A21,
                        matrix1.A22 + matrix2.A22,
                        matrix1.A23 + matrix2.A23,
                        matrix1.A31 + matrix2.A31,
                        matrix1.A32 + matrix2.A32,
                        matrix1.A33 + matrix2.A33
                    );
        }

        public static Matrix3x3 Sub(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            return new Matrix3x3
                    (
                        matrix1.A11 - matrix2.A11,
                        matrix1.A12 - matrix2.A12,
                        matrix1.A13 - matrix2.A13,
                        matrix1.A21 - matrix2.A21,
                        matrix1.A22 - matrix2.A22,
                        matrix1.A23 - matrix2.A23,
                        matrix1.A31 - matrix2.A31,
                        matrix1.A32 - matrix2.A32,
                        matrix1.A33 - matrix2.A33
                    );
        }

        // see: https://stackoverflow.com/questions/1996957/conversion-euler-to-matrix-and-matrix-to-euler
        public static Matrix3x3 EulerAnglesToMatrix3x3(Vector3 euler)
        {
            // calc radian
            var deg = new Vector3();
            deg.X = Math.PI * euler.X / 180.0d;
            deg.Y = Math.PI * euler.Y / 180.0d;
            deg.Z = Math.PI * euler.Z / 180.0d;

            // return rotation matrix
            return new Matrix3x3
                    (
                        // first row
                        (Math.Cos(deg.Y) * Math.Cos(deg.Z) + Math.Sin(deg.Y) * Math.Sin(deg.X) * Math.Sin(deg.Z)),
                        (Math.Cos(deg.Z) * Math.Sin(deg.Y) * Math.Sin(deg.X) - Math.Sin(deg.Z) * Math.Cos(deg.Y)),
                        (Math.Cos(deg.X) * Math.Sin(deg.Y)),
                        // second row
                        (Math.Cos(deg.X) * Math.Sin(deg.Z)),
                        (Math.Cos(deg.Z) * Math.Cos(deg.X)),
                        (-Math.Sin(deg.X)),
                        // third row
                        (Math.Sin(deg.Z) * Math.Cos(deg.Y) * Math.Sin(deg.X) - Math.Sin(deg.Y) * Math.Cos(deg.Z)),
                        (Math.Sin(deg.Y) * Math.Sin(deg.Z) + Math.Cos(deg.Z) * Math.Cos(deg.Y) * Math.Sin(deg.X)),
                        (Math.Cos(deg.X) * Math.Cos(deg.Y))
                   );
        }

    }
}
