using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SwedishPersonLookup.Test
{
    

    [TestClass]
    public class PhoneNumberLookupTests
    {
        [TestMethod, ExpectedException(typeof(InvalidPhoneNumberException))]
        public void TestRejectsBadPhoneNumber()
        {
            var client = new PersonLookupClient();
            client.ByPhoneNumber("asdf");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestRequiresPhoneNumber()
        {
            var client = new PersonLookupClient();
            client.ByPhoneNumber(null);
        }

        [TestMethod]
        public void ConvertToNationalNumber()
        {
            Assert.AreEqual("0705555555", PersonLookupClient.ToNationalNumber("+46 70 555 5555"));
        }

        [TestMethod]
        public async Task LookUpJohanna()
        {
            var person = await new PersonLookupClient().ByPhoneNumber("+46702759810");
            Assert.IsNotNull(person);
            Assert.AreEqual("Johanna Östlund", person.FullName);
            Assert.AreEqual("Uppsala", person.City);
        }

        [TestMethod]
        public async Task LookUpTom()
        {
            var person = await new PersonLookupClient().ByPhoneNumber("+46702386266");
            Assert.IsNull(person);
            //Assert.AreEqual("Johanna Östlund", person.FullName);
            //Assert.AreEqual("Uppsala", person.City);
        }
    }

    [TestClass]
    public class OneEighteenOneHundredScraperTests
    {
 

        [TestMethod]
        public void ScrapesOutJohanna()
        {
            string response = null;


            Debug.WriteLine(string.Join("\n",
                Assembly.GetAssembly(typeof (OneEighteenOneHundredScraperTests)).GetManifestResourceNames()));
            
            var stream =
                Assembly.GetAssembly(typeof (OneEighteenOneHundredScraperTests))
                    .GetManifestResourceStream("SwedishPersonLookup.Test.Response.txt");

            using (var reader = new StreamReader(stream))
            {
                response = reader.ReadToEnd();
            }

            var scraper = new OneEighteenOneHundredScraper(response);
            Assert.AreEqual("Johanna Östlund", scraper.GetFirstPersonFullName());

        }

        [TestMethod]
        public void ScrapesOutUppsala()
        {
             string response = null;


            Debug.WriteLine(string.Join("\n",
                Assembly.GetAssembly(typeof (OneEighteenOneHundredScraperTests)).GetManifestResourceNames()));
            
            var stream =
                Assembly.GetAssembly(typeof (OneEighteenOneHundredScraperTests))
                    .GetManifestResourceStream("SwedishPersonLookup.Test.Response.txt");

            using (var reader = new StreamReader(stream))
            {
                response = reader.ReadToEnd();
            }

            var scraper = new OneEighteenOneHundredScraper(response);
            Assert.AreEqual("Uppsala", scraper.GetFirstPersonCity());
        }
    }
}
