using System.Windows;
using Feedling;
using Feedling.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeedlingTests
{


    /// <summary>
    ///This is a test class for FontConversionsTest and is intended
    ///to contain all FontConversionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FontConversionsTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for FontStyleFromString
        ///</summary>
        [TestMethod]
        public void FontStyleFromStringTest()
        {
            const string value = "Italic";
            var expected = FontStyles.Italic;
            var actual = FontConversions.FontStyleFromString(value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FontStyleToString
        ///</summary>
        [TestMethod]
        public void FontStyleToStringTest()
        {
            var value = FontStyles.Oblique;
            const string expected = "Oblique";
            var actual = FontConversions.FontStyleToString(value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FontWeightFromString
        ///</summary>
        [TestMethod]
        public void FontWeightFromStringTest()
        {
            const string value = "Heavy";
            var expected = FontWeights.Heavy;
            var actual = FontConversions.FontWeightFromString(value);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for FontWeightToString
        ///</summary>
        [TestMethod]
        public void FontWeightToStringTest()
        {
            var value = FontWeights.SemiBold;
            const string expected = "SemiBold";
            var actual = FontConversions.FontWeightToString(value);
            Assert.AreEqual(expected, actual);
        }
    }
}
