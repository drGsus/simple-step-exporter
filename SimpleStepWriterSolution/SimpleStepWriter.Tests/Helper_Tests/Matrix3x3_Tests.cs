using NUnit.Framework;
using SimpleStepWriter.Helper;

namespace SimpleStepWriter.Tests
{
    [TestFixture]
    public class Matrix3x3_Tests
    {
        private Matrix3x3 m1;
        private Matrix3x3 m2;

        [SetUp]
        public void SetUp()
        {
            m1 = new Matrix3x3(                
                11d, 12d, 13d,
                21d, 22d, 23d,
                31d, 32d, 33d 
            );
            m2 = new Matrix3x3(
                -7d, 8.6d, 1.42d,
                62.3d, -4.2d, 2.1316783d,
                3.02d, 32d, -33.00137d
            );
        }

        [Test]
        public void CanInitMatrix3x3WithZeroValues()
        {
            var actual = new Matrix3x3();

            var expected = 0d;

            Assert.AreEqual(expected, actual.A11);
            Assert.AreEqual(expected, actual.A12);
            Assert.AreEqual(expected, actual.A13);
            Assert.AreEqual(expected, actual.A21);
            Assert.AreEqual(expected, actual.A22);
            Assert.AreEqual(expected, actual.A23);
            Assert.AreEqual(expected, actual.A31);
            Assert.AreEqual(expected, actual.A32);
            Assert.AreEqual(expected, actual.A33);
        }

        [Test]
        public void CanInitMatrix3x3WithCustomValues()
        {
            var actual = new Matrix3x3(
                                    1.1d, 2.2d, 3.3d,
                                    4.4d, 5.5d, 6.6d,
                                    7.7d, 8.8d, 9.9d
                                );

            var expectedA11 = 1.1d;
            var expectedA12 = 2.2d;
            var expectedA13 = 3.3d;
            var expectedA21 = 4.4d;
            var expectedA22 = 5.5d;
            var expectedA23 = 6.6d;
            var expectedA31 = 7.7d;
            var expectedA32 = 8.8d;
            var expectedA33 = 9.9d;

            Assert.AreEqual(expectedA11, actual.A11, 0.00001d);
            Assert.AreEqual(expectedA12, actual.A12, 0.00001d);
            Assert.AreEqual(expectedA13, actual.A13, 0.00001d);
            Assert.AreEqual(expectedA21, actual.A21, 0.00001d);
            Assert.AreEqual(expectedA22, actual.A22, 0.00001d);
            Assert.AreEqual(expectedA23, actual.A23, 0.00001d);
            Assert.AreEqual(expectedA31, actual.A31, 0.00001d);
            Assert.AreEqual(expectedA32, actual.A32, 0.00001d);
            Assert.AreEqual(expectedA33, actual.A33, 0.00001d);
        }

        [Test]
        public void CanAddTwoMatrices()
        {
            var actual = Matrix3x3.Add(m1, m2);
            var expected = new Matrix3x3(
                4d, 20.6d, 14.42d,
                83.3d, 17.8d, 25.1316783,
                34.02, 64d, -0.00137
            );

            Assert.AreEqual(expected.A11, actual.A11, 0.00001d);
            Assert.AreEqual(expected.A12, actual.A12, 0.00001d);
            Assert.AreEqual(expected.A13, actual.A13, 0.00001d);
            Assert.AreEqual(expected.A21, actual.A21, 0.00001d);
            Assert.AreEqual(expected.A22, actual.A22, 0.00001d);
            Assert.AreEqual(expected.A23, actual.A23, 0.00001d);
            Assert.AreEqual(expected.A31, actual.A31, 0.00001d);
            Assert.AreEqual(expected.A32, actual.A32, 0.00001d);
            Assert.AreEqual(expected.A33, actual.A33, 0.00001d);
        }

        [Test]
        public void CanSubstractTwoMatrices()
        {
            var actual = Matrix3x3.Sub(m1, m2);
            var expected = new Matrix3x3(
                18d, 3.4d, 11.58d,
                -41.3d, 26.2d, 20.8683217d,
                27.98d, 0d, 66.00137d
            );
            
            Assert.AreEqual(expected.A11, actual.A11, 0.00001d);
            Assert.AreEqual(expected.A12, actual.A12, 0.00001d);
            Assert.AreEqual(expected.A13, actual.A13, 0.00001d);
            Assert.AreEqual(expected.A21, actual.A21, 0.00001d);
            Assert.AreEqual(expected.A22, actual.A22, 0.00001d);
            Assert.AreEqual(expected.A23, actual.A23, 0.00001d);
            Assert.AreEqual(expected.A31, actual.A31, 0.00001d);
            Assert.AreEqual(expected.A32, actual.A32, 0.00001d);
            Assert.AreEqual(expected.A33, actual.A33, 0.00001d);
        }

        [Test]
        public void CanConvertPositiveEulerAnglesToMatrix3x3()
        {            
            var euler = new Vector3(63.8d, 183.14d, 24.3d);

            var expected = Matrix3x3.EulerAnglesToMatrix3x3(euler);

            // http://danceswithcode.net/engineeringnotes/rotations_in_3d/demo3D/rotations_in_3d_tool.html
            var actual = new Matrix3x3(
                -0.9303, 0.3661, -0.0242,
                0.1816, 0.4023, -0.8973,
                -0.3188, -0.8391, -0.4409
            );

            Assert.AreEqual(expected.A11, actual.A11, 0.001d);
            Assert.AreEqual(expected.A12, actual.A12, 0.001d);
            Assert.AreEqual(expected.A13, actual.A13, 0.001d);
            Assert.AreEqual(expected.A21, actual.A21, 0.001d);
            Assert.AreEqual(expected.A22, actual.A22, 0.001d);
            Assert.AreEqual(expected.A23, actual.A23, 0.001d);
            Assert.AreEqual(expected.A31, actual.A31, 0.001d);
            Assert.AreEqual(expected.A32, actual.A32, 0.001d);
            Assert.AreEqual(expected.A33, actual.A33, 0.001d);
            
        }

        [Test]
        public void CanConvertNegativeEulerAnglesToMatrix3x3()
        {
            var euler = new Vector3(623.8d, -1883.14d, -24.3d);

            var expected = Matrix3x3.EulerAnglesToMatrix3x3(euler);

            // http://danceswithcode.net/engineeringnotes/rotations_in_3d/demo3D/rotations_in_3d_tool.html
            var actual = new Matrix3x3(
                -0.2974, 0.9487, 0.1072,
                0.0444, -0.0985, 0.9941,
                0.9537, 0.3003, -0.0129
            );

            Assert.AreEqual(expected.A11, actual.A11, 0.001d);
            Assert.AreEqual(expected.A12, actual.A12, 0.001d);
            Assert.AreEqual(expected.A13, actual.A13, 0.001d);
            Assert.AreEqual(expected.A21, actual.A21, 0.001d);
            Assert.AreEqual(expected.A22, actual.A22, 0.001d);
            Assert.AreEqual(expected.A23, actual.A23, 0.001d);
            Assert.AreEqual(expected.A31, actual.A31, 0.001d);
            Assert.AreEqual(expected.A32, actual.A32, 0.001d);
            Assert.AreEqual(expected.A33, actual.A33, 0.001d);
        }
    }
}
