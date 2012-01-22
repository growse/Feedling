using System.Reflection;
using Feedling;
using Feedling.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FeedHanderPluginInterface;
using System.Windows.Media;
using System.Windows;
using System.Net;

namespace FeedlingTests
{


    /// <summary>
    ///This is a test class for FeedConfigItemTest and is intended
    ///to contain all FeedConfigItemTest Unit Tests
    ///</summary>
    [TestClass]
    public class FeedConfigItemTest
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
        ///A test for Copy Results should have the same properties but different GUIDs.
        ///</summary>
        [TestMethod]
        public void CopyTest()
        {
            var expected = new FeedConfigItem();
            var actual = expected.Copy();
            var properties = typeof(FeedConfigItem).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(Guid))
                {
                    Assert.AreEqual(property.GetValue(actual, null), property.GetValue(expected, null));
                }
            }
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            var target = new FeedConfigItem { Url = "http://feedling.net" };
            const string expected = "http://feedling.net";
            var actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AuthType
        ///</summary>
        [TestMethod]
        public void AuthTypeTest()
        {
            var target = new FeedConfigItem { AuthType = FeedAuthTypes.Other };
            const FeedAuthTypes expected = FeedAuthTypes.Other;
            target.AuthType = expected;
            var actual = target.AuthType;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DefaultColor
        ///</summary>
        [TestMethod]
        public void DefaultColorTest()
        {
            var target = new FeedConfigItem { DefaultColorB = 111, DefaultColorG = 192, DefaultColorR = 128 };
            var expected = Color.FromArgb(255, 128, 192, 111);
            target.DefaultColor = expected;
            var actual = target.DefaultColor;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for DisplayedItems
        ///</summary>
        [TestMethod]
        public void DisplayedItemsTest()
        {
            var target = new FeedConfigItem { DisplayedItems = 25 };
            const int expected = 25;
            target.DisplayedItems = expected;
            var actual = target.DisplayedItems;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FontFamily
        ///</summary>
        [TestMethod]
        public void FontFamilyTest()
        {
            var target = new FeedConfigItem { FontFamilyString = "Arial" };
            var expected = new FontFamily("Arial");
            target.FontFamily = expected;
            var actual = target.FontFamily;
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for FontSize
        ///</summary>
        [TestMethod]
        public void FontSizeTest()
        {
            var target = new FeedConfigItem { FontSize = 12 };
            const double expected = 12F;
            target.FontSize = expected;
            var actual = target.FontSize;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FontStyle
        ///</summary>
        [TestMethod]
        public void FontStyleTest()
        {
            var target = new FeedConfigItem { FontStyleString = "Italic" };
            var expected = FontStyles.Italic;
            target.FontStyle = expected;
            var actual = target.FontStyle;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FontWeight
        ///</summary>
        [TestMethod]
        public void FontWeightTest()
        {
            var target = new FeedConfigItem { FontWeightString = "SemiBold" };
            var expected = FontWeights.SemiBold;
            target.FontWeight = expected;
            var actual = target.FontWeight;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Guid
        ///</summary>
        [TestMethod]
        public void GuidTest()
        {
            var testguid = Guid.NewGuid();
            var target = new FeedConfigItem { Guid = testguid };
            var expected = testguid;
            target.Guid = expected;
            var actual = target.Guid;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for HoverColor
        ///</summary>
        [TestMethod]
        public void HoverColorTest()
        {
            var target = new FeedConfigItem { HoverColorR = 18, HoverColorG = 211, HoverColorB = 255 };
            var expected = Color.FromArgb(255, 18, 211, 255);
            target.HoverColor = expected;
            var actual = target.HoverColor;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Password
        ///</summary>
        [TestMethod]
        public void PasswordTest()
        {
            var target = new FeedConfigItem { Password = "Passw0rd" }; // TODO: Initialize to an appropriate value
            const string expected = "Passw0rd";
            target.Password = expected;
            string actual = target.Password;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Position
        ///</summary>
        [TestMethod]
        public void PositionTest()
        {
            var target = new FeedConfigItem { XPos = 30, YPos = 299 };
            var expected = new Point(30, 299);
            target.Position = expected;
            var actual = target.Position;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Proxy
        ///</summary>
        [TestMethod]
        public void ProxyTest()
        {
            var target = new FeedConfigItem { ProxyHost = "1.2.3.4", ProxyPass = "ProxyPassword", ProxyType = HttpProxyHelper.ProxyType.Custom, ProxyUser = "proxydomain\\proxyusername", ProxyAuth = true, ProxyPort = 3848 };
            IWebProxy expected = new WebProxy { Address = new Uri("http://1.2.3.4:3848"), Credentials = new NetworkCredential("proxyusername", "ProxyPassword", "proxydomain") };
            var actual = target.Proxy;
            Assert.AreEqual(expected.GetProxy(new Uri("http://www.google.com")), actual.GetProxy(new Uri("http://www.google.com")));
            Assert.AreEqual(expected.Credentials.GetCredential(new Uri("http://www.google.com"), "").Domain,
                            actual.Credentials.GetCredential(new Uri("http://www.google.com"), "").Domain);
            Assert.AreEqual(expected.Credentials.GetCredential(new Uri("http://www.google.com"), "").UserName,
                            actual.Credentials.GetCredential(new Uri("http://www.google.com"), "").UserName);
            Assert.AreEqual(expected.Credentials.GetCredential(new Uri("http://www.google.com"), "").Password,
                            actual.Credentials.GetCredential(new Uri("http://www.google.com"), "").Password);
        }

        /// <summary>
        ///A test for TitleFontFamily
        ///</summary>
        [TestMethod]
        public void TitleFontFamilyTest()
        {
            var target = new FeedConfigItem { TitleFontFamilyString = "Arial" };
            var expected = new FontFamily("Arial");
            target.TitleFontFamily = expected;
            var actual = target.TitleFontFamily;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TitleFontSize
        ///</summary>
        [TestMethod]
        public void TitleFontSizeTest()
        {
            var target = new FeedConfigItem { TitleFontSize = 24 };
            const double expected = 24F;
            target.TitleFontSize = expected;
            var actual = target.TitleFontSize;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TitleFontStyle
        ///</summary>
        [TestMethod]
        public void TitleFontStyleTest()
        {
            var target = new FeedConfigItem { FontStyleString = "Oblique" };
            var expected = FontStyles.Oblique;
            target.TitleFontStyle = expected;
            var actual = target.TitleFontStyle;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TitleFontWeight
        ///</summary>
        [TestMethod]
        public void TitleFontWeightTest()
        {
            var target = new FeedConfigItem { FontWeightString = "UltraBlack" };
            var expected = FontWeights.UltraBlack;
            target.TitleFontWeight = expected;
            var actual = target.TitleFontWeight;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateInterval
        ///</summary>
        [TestMethod]
        public void UpdateIntervalTest()
        {
            var target = new FeedConfigItem { UpdateInterval = 483 };
            const int expected = 483;
            target.UpdateInterval = expected;
            var actual = target.UpdateInterval;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Url
        ///</summary>
        [TestMethod]
        public void UrlTest()
        {
            var target = new FeedConfigItem { Url = "http://www.feedling.net" };
            const string expected = "http://www.feedling.net";
            target.Url = expected;
            var actual = target.Url;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UserName
        ///</summary>
        [TestMethod]
        public void UserNameTest()
        {
            var target = new FeedConfigItem { UserName = "testusername" };
            const string expected = "testusername";
            target.UserName = expected;
            var actual = target.UserName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Width
        ///</summary>
        [TestMethod]
        public void WidthTest()
        {
            var target = new FeedConfigItem { Width = 3948 };
            const double expected = 3948F;
            target.Width = expected;
            var actual = target.Width;
            Assert.AreEqual(expected, actual);
        }


    }
}
