using NUnit.Framework;
using System;

namespace SimpleStepWriter.Tests
{
    [TestFixture]
    public class UnitTestSample
    {
        private int number1;
        private int number2;

        [SetUp]
        public void SetUpTest()
        {
            number1 = 2;
            number2 = 4;
        }

        [Test]
        public void TestMethod1()
        {
            var actual = number1 + number2;
            int expected = 6;
           
            Assert.AreEqual(expected, actual);
        }
    }
}
