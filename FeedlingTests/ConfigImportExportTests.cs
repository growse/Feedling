using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Feedling;
using Feedling.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeedlingTests
{
    [TestClass]
    public class ConfigImportExportTests
    {
        /// <summary>
        /// XmlImportTest1 XML should have 16 feed items.
        /// </summary>
        [TestMethod]
        public void XmlImportTest1()
        {
            var actual = new FeedConfigItemList();
            var serializer = new XmlSerializer(actual.GetType());
            var sr = new StringReader(Properties.Resources.XmlImportTest1);
            var xmlr = XmlReader.Create(sr);
            if (serializer.CanDeserialize(xmlr))
            {
                actual = (FeedConfigItemList)serializer.Deserialize(xmlr);
            }
            xmlr.Close();
            sr.Close();
            Assert.AreEqual(16, actual.Items.Count);
        }
    }
}
