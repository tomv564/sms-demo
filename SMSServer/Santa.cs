using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SwedishPersonLookup;

namespace SMSServer
{

    public static class Santa
    {




        private static readonly string[] Formats =
        {
            "Dear {0} of {2}:\nIf you've been good since you last sat on my knee, you will find a {1} under your tree.\nBest wishes,\nSanta",
            "Dear {0}: If you've been good this year, Santa will deliver your {1} to {2} on Christmas Eve!",
            "Dear {0}, Santa will deliver your {1} on Christmas Eve!",
            "Dear {0}, Santa will deliver your {1} to {2}",
            "{0}: Santa will deliver your {1}"
        };

        private static readonly Regex SwedishNumberRegex = new Regex(@"^\+46[\d\s]*$", RegexOptions.Compiled);
        private static readonly Regex NounPrefixRegex = new Regex(@"^\s*(?:a|an)\s+", RegexOptions.IgnoreCase);

        public static async Task<string> Reply(ShortMessage sms)
        {

            if (!SwedishNumberRegex.IsMatch(sms.From))
                return "Sorry, Santa only works for Swedish numbers (+46)";

            Person person = null;
            try {
                var lookup = new PersonLookupClient();
                person = await lookup.ByPhoneNumber(sms.From);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Sorry, Santa is having some technical difficulties. The elves will check the error logs shortly!";
            }

            // person not known
            if (person == null)
                return "Sorry, you are not in Santa's book. Try from another number?";

            // clean the input a little
            var firstName = person.FullName.Split(' ')[0];
            var where = string.IsNullOrWhiteSpace(person.City) ? "your town" : person.City;
            var gift = NounPrefixRegex.Replace(sms.Message, "");

            // find the first (largest) message that fits an arbitrary limit of a hundred chars.
            var message =
                Formats.Select(format => String.Format(format, firstName, gift, where))
                    .FirstOrDefault(generated => generated.Length < 160);

            return message ?? "You ask for too much!";

        }


    }
}
