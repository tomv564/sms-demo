using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FourtySixElksClient
{
    public class ApiClient
    {
        private readonly string username;
        private readonly string password;

        public ApiClient(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("password");
            this.username = username;
            this.password = password;

        }

        public async Task<string> SendSMSAsync(string @from, string to, string message, bool isFlash = false, Uri deliveryUri = null)
        {
            if (string.IsNullOrEmpty(from)) throw new ArgumentNullException("from");
            if (string.IsNullOrEmpty(to)) throw new ArgumentNullException("to");
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException("message");

            var httpClient = CreateClient();

            var data = new Dictionary<string, string>
            {
                {"from", from},
                {"to", to},
                {"message", message}
            };

            if (isFlash)
                data["flashsms"] = "yes";

            if (deliveryUri != null)
                data["whendelivered"] = deliveryUri.ToString();


            var response = await httpClient.PostAsync("SMS", new FormUrlEncodedContent(data));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return content; //sms id

            throw new FourtySixElksApiException(content);
        }

        public async Task<string> CreateNumberAsync(string country, Uri smsCallbackUri, Uri mmsCallbackUri, string voiceAction)
        {
   

            var httpClient = CreateClient();
          
            var data = new Dictionary<string, string>
            {
                {"country", country},
                {"sms_url", smsCallbackUri.ToString()},
                {"mms_url", mmsCallbackUri.ToString()},
                {"voice_start", voiceAction }
            };

       
            var response = await httpClient.PostAsync("Numbers", new FormUrlEncodedContent(data));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return content; //sms id

            throw new FourtySixElksApiException(content);

//            GET https://api.46elks.com/a1/Numbers/n57c8f48af76bf986a14f251b35389e8b
//
//{
//  "id": "n57c8f48af76bf986a14f251b35389e8b",
//  "active": "yes",
//  "country": "se",
//  "number": "+46766861001",
//  "capabilities": [ "sms", "voice", "mms" ],
//  "sms_url": "http://myservice.se/callback/newsms.php",
//  "mms_url": "http://myservice.se/callback/newmms.php",
//  "voice_start": "http://myservice.se/callback/newcall.php"
//}
        }

        private HttpClient CreateClient()
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(byteArray));

            httpClient.BaseAddress = new Uri("https://api.46elks.com/a1/");
            return httpClient;
        }
    }

    public class FourtySixElksApiException : Exception
    {
        public FourtySixElksApiException(string message) : base(message)
        {
        }
    }
}
