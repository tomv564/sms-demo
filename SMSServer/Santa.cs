﻿using System;
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
            "Dear {0}: If you've been good this year, Santa will deliver your {1} to {2} on Christmas Eve!",
            "Dear {0}, Santa will deliver your {1} on Christmas Eve!",
            "Dear {0}, Santa will deliver your {1} to {2}",
            "{0}: Santa will deliver your {1}"
        };

        private static readonly Regex SwedishNumberRegex = new Regex(@"^\+46[\d\s]*$", RegexOptions.Compiled);

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
                return "Sorry, Santa is having some technical difficulties!";
            }

            // person not known
            if (person == null)
                return "Sorry, you are not in Santa's book. Try from another number?";

            var firstName = person.FullName.Split(' ')[0];
            var where = string.IsNullOrWhiteSpace(person.City) ? "you" : person.City;
            
            // find the first (largest) message that fits an arbitrary limit of a hundred chars.
            var message =
                Formats.Select(format => String.Format(format, firstName, sms.Message, where))
                    .FirstOrDefault(generated => generated.Length < 100);

            return message ?? "You ask for too much!";

        }


    }
}