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
    public record ProductCode
    {
        public const string Pattern = "^\\d+$";
        private static readonly Regex ValidPattern = new(Pattern);
        public string Value { get; }

        public ProductCode(string value)
        {
            if (ValidPattern.IsMatch(value))
            {
                if(value.Length <= 5)
                {
                    Value = value;
                } 
            }
            else
            {
                throw new InvalidProductCodeException("Wrong Product Code");
            }
        }
        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);
        public override string ToString()
        {
            return Value;
        }
        public static bool TryParse(string stringValue, out ProductCode code)
        {
            bool isValid = false;
            code = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                code = new(stringValue);
            }

            return isValid;
        }

        public static Option<ProductCode> TryParseProductCode(string stringValue)
        {
            if (IsValid(stringValue))
            {
                return Some<ProductCode>(new(stringValue));
            }
            else
            {
                return None;
            }
        }
    }
}
