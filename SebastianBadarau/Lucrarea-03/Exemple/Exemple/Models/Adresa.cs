using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;


namespace Exemple.Domain.Models
{
    public record Adresa
    {
        private static readonly Regex ValidPattern = new("/d{1,}(s{1}w{1,})(s{1}?w{1,})+g");

        public string Value { get; }

        private Adresa(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidateAddressException("");
            }
        }

        private static bool IsValid(string stringValue) => !String.IsNullOrEmpty(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static bool TryParse(string stringValue, out Adresa address)
        {
            bool isValid = false;
            address = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                address = new(stringValue);
            }

            return isValid;
        }

        public static Option<Adresa> TryParseAdresa(string address)
        {
            if (IsValid(address))
            {
                return Some<Adresa>(new(address));
            }
            else
            {
                return None;
            }
        }
    }
}
