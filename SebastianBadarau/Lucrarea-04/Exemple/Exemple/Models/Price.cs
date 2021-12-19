using LanguageExt;
using System.Text.RegularExpressions;

namespace Exemple.Domain.Models
{
    public record Price
    {
        private static readonly Regex ValidPattern = new("^\\d+$");

        public string Value { get; }

        public Price(string value)
        {
            if (ValidPattern.IsMatch(value))
            {
                if (value.Length <= 5)
                {
                    Value = value;
                }
            }
            else
            {
                throw new InvalidPriceException("Wrong Price");
            }
        }
        private static bool IsValid(decimal numericPrice) => numericPrice > 0 && numericPrice <= 1000;
        public override string ToString()
        {
            return Value;
        }
       

      
    }
}