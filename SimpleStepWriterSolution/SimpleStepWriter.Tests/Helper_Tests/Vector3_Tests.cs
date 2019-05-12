using NUnit.Framework;
using SimpleStepWriter.Helper;

namespace SimpleStepWriter.Tests
{
    [TestFixture]
    public class Vector3_Tests
    {
        private Vector3 v1;
        private Vector3 v2;
        private double scalar1;

        [SetUp]
        public void SetUp()
        {
            v1 = new Vector3(1.1d, -2.5d, 3.7d);
            v2 = new Vector3(.4d, -6d, 83.3d);
            scalar1 = 2d;
        }

        [Test]
        public void IsVectorZero()
        {
            var actual = Vector3.Zero;

            var expected = 0d;

            Assert.AreEqual(expected, actual.X);
            Assert.AreEqual(expected, actual.Y);
            Assert.AreEqual(expected, actual.Z);
        }

        [Test]
        public void IsVectorOne()
        {
            var actual = Vector3.One;

            var expected = 1d;

            Assert.AreEqual(expected, actual.X);
            Assert.AreEqual(expected, actual.Y);
            Assert.AreEqual(expected, actual.Z);
        }

        [Test]
        public void CanInitVectorWithZeroValues()
        {
            var actual = new Vector3();

            var expected = 0d;

            Assert.AreEqual(expected, actual.X);
            Assert.AreEqual(expected, actual.Y);
            Assert.AreEqual(expected, actual.Z);
        }

        [Test]
        public void CanInitVectorWithCustomValues()
        {
            var actual = new Vector3(1.1d, 2.2d, 3.3d);

            var expectedX = 1.1d;
            var expectedY = 2.2d;
            var expectedZ = 3.3d;

            Assert.AreEqual(expectedX, actual.X);
            Assert.AreEqual(expectedY, actual.Y);
            Assert.AreEqual(expectedZ, actual.Z);
        }

        [Test]
        public void CanAddTwoVectors()
        {      
            Vector3 actual = Vector3.Add(v1, v2);
            Vector3 expected = new Vector3(1.5d, -8.5d, 87d);

            Assert.AreEqual(expected.X, actual.X, 0.00001d);
            Assert.AreEqual(expected.Y, actual.Y, 0.00001d);
            Assert.AreEqual(expected.Z, actual.Z, 0.00001d);
        }

        [Test]
        public void CanSubstractTwoVectors()
        {
            Vector3 actual = Vector3.Sub(v1, v2);
            Vector3 expected = new Vector3(0.7d, 3.5d, -79.6d);
         
            Assert.AreEqual(expected.X, actual.X, 0.00001d);
            Assert.AreEqual(expected.Y, actual.Y, 0.00001d);
            Assert.AreEqual(expected.Z, actual.Z, 0.00001d);
        }

        [Test]
        public void CanDivideVectorByScalar()
        {
            Vector3 actual = Vector3.Div(v1, scalar1);
            Vector3 expected = new Vector3(0.55d, -1.25d, 1.85d);

            Assert.AreEqual(expected.X, actual.X, 0.00001d);
            Assert.AreEqual(expected.Y, actual.Y, 0.00001d);
            Assert.AreEqual(expected.Z, actual.Z, 0.00001d);
        }

        [Test]
        public void CanCalculateCrossProductOfTwoVectors()
        {
            Vector3 actual = Vector3.Cross(v1, v2);
            Vector3 expected = new Vector3(-186.05d, -90.15d, -5.6d);

            Assert.AreEqual(expected.X, actual.X, 0.00001d);
            Assert.AreEqual(expected.Y, actual.Y, 0.00001d);
            Assert.AreEqual(expected.Z, actual.Z, 0.00001d);
        }

        [Test]
        public void CanCalculateDotProductOfTwoVectors()
        {
            double actual = Vector3.Dot(v1, v2);
            double expected = 323.65d;

            Assert.AreEqual(expected, actual, 0.00001d);           
        }

        [Test]
        public void ToStringReturnsAccurateValue()
        {
            var actualX = double.Parse(v1.XString, System.Globalization.CultureInfo.InvariantCulture);
            var actualY = double.Parse(v1.YString, System.Globalization.CultureInfo.InvariantCulture);
            var actualZ = double.Parse(v1.ZString, System.Globalization.CultureInfo.InvariantCulture);

            var expectedX = v1.X;
            var expectedY = v1.Y;
            var expectedZ = v1.Z;
            
            Assert.AreEqual(expectedX, actualX);
            Assert.AreEqual(expectedY, actualY);
            Assert.AreEqual(expectedZ, actualZ);
        }
    }
}
