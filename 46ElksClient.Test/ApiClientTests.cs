using System;
using System.Net;
using System.Runtime.Remoting.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FourtySixElksClient.Test
{
    [TestClass]
    public class ApiClientTests
    {

        private const string TestUser = "";
        private const string TestPassword = "";

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void RequiresUsernameAndPassword()
        {
            new ApiClient(null, null);

        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void SendTextRequiresSender()
        {
            var client = new ApiClient(TestUser, TestPassword);
            client.SendSMSAsync(null, null, null, false);
        }


        [TestMethod]
        public async void SendSMS()
        {
            var client = new ApiClient(TestUser, TestPassword);
            var id = await client.SendSMSAsync("+46702386266", "+46702386266", "I know where you live!");
            Assert.IsFalse(string.IsNullOrEmpty(id));
        }

        [TestMethod]
        public async void SendFlashSMS()
        {
            var client = new ApiClient(TestUser, TestPassword);
            var id = await client.SendSMSAsync("+46702386266", "+46702386266", "The answer is 42", true);
            Assert.IsFalse(string.IsNullOrEmpty(id));

        }

        [TestMethod]
        public async void SendWithConfirmationUrl()
        {
            var client = new ApiClient(TestUser, TestPassword);

            var id = await client.SendSMSAsync("+46702386266", "+46702386266", "I know you have read this!", false, new Uri("http://sms.tomv.io/confirm"));
            Assert.IsFalse(string.IsNullOrEmpty(id));

        }

        [TestMethod]
        public async void CreateNumber()
        {
            var client = new ApiClient(TestUser, TestPassword);

            var id = await client.CreateNumberAsync("se", new Uri("http://sms.tomv.io/receive"), null, null);
            Assert.IsFalse(string.IsNullOrEmpty(id));
        }
    }
}
