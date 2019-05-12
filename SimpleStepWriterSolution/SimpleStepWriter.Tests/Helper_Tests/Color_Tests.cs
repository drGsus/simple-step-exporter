using NUnit.Framework;
using SimpleStepWriter.Helper;

namespace SimpleStepWriter.Tests
{
    [TestFixture]
    public class Color_Tests
    {
        [Test]
        public void CanInitColorWithGreyValue()
        {
            var actual = new Color();

            var expectedRGB = 0.5f;
            var expectedAlpha = 1f;            

            Assert.AreEqual(expectedRGB, actual.R);
            Assert.AreEqual(expectedRGB, actual.G);
            Assert.AreEqual(expectedRGB, actual.B);
            Assert.AreEqual(expectedAlpha, actual.A);
        }

        [Test]
        public void CanInitColorWithCustomValues()
        {
            var actual = new Color(0.33f, 0.66f, 0.99f, 1f);

            var expectedR = 0.33f;
            var expectedG = 0.66f;
            var expectedB = 0.99f;
            var expectedAlpha = 1f;

            Assert.AreEqual(expectedR, actual.R);
            Assert.AreEqual(expectedG, actual.G);
            Assert.AreEqual(expectedB, actual.B);
            Assert.AreEqual(expectedAlpha, actual.A);
        }

    }
}
