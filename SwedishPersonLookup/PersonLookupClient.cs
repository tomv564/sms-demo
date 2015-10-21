using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwedishPersonLookup
{
    public class PersonLookupClient
    {
        private static readonly Regex PhoneNumberRegex = new Regex(@"^\+[\d\s]*$");

        public static string ToNationalNumber(string phoneNumber)
        {
            return phoneNumber.Replace(" ", "").Replace("+46", "0");
        }

        private static string ByPhoneNumberUrl(string nationalPhoneNumber)
        {
            return String.Format("http://www.118100.se/sok-person/{0}", nationalPhoneNumber);
        }

        public async Task<Person> ByPhoneNumber(string internationalPhoneNumber)
        {
            if (internationalPhoneNumber == null)
                throw new ArgumentNullException("internationalPhoneNumber");

            if (!PhoneNumberRegex.IsMatch(internationalPhoneNumber))
                throw new InvalidPhoneNumberException(internationalPhoneNumber);

            string url = ByPhoneNumberUrl(ToNationalNumber(internationalPhoneNumber));

            var httpClient = new HttpClient();
            var page = await httpClient.GetStringAsync(url);


            var scraper = new OneEighteenOneHundredScraper(page);
            var fullName = scraper.GetFirstPersonFullName();
            if (fullName == null)
                return null;

            return new Person() {FullName = fullName, City = scraper.GetFirstPersonCity()};

        }


    }

    public class OneEighteenOneHundredScraper
    {

        private readonly string content;
        private static readonly Regex FirstPersonFullNameRegex = new Regex(@"nameBlock.*h4.*<a.*>(.+)</a.*addressBlock", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex FirstPersonCityRegex = new Regex(@"addressBlock"">.*\d\d\d\s\d\d.*&nbsp;([\w\s]*).*moreInfo", RegexOptions.Singleline | RegexOptions.Compiled);

        public OneEighteenOneHundredScraper(string content)
        {
            this.content = content;
        }

        public string GetFirstPersonFullName()
        {
            return TryRegex(FirstPersonFullNameRegex);
        }

        public string GetFirstPersonCity()
        {
            return TryRegex(FirstPersonCityRegex);
        }

        private string TryRegex(Regex regex)
        {
            var match = regex.Match(this.content);
            if (!match.Success)
                return null;

            return match.Groups.Count != 2 ? null : match.Groups[1].Value.Trim();
        
        }
    }

    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException(string phoneNumber)
            : base(String.Format("The number {0} does not look like a phone number!", phoneNumber))
        {
        }

    }

    public class Person
    {
        public string FullName { get; set; }
        public string City { get; set; }
    }
}
