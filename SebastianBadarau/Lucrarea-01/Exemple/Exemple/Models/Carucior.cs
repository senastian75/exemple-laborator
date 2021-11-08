using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class Carucior
    {
        public interface ICarucior { }

        public record CosGol(IReadOnlyCollection<UnvalidatedProductQuantity> productList) : ICarucior;
        public record InvalidatedCarucior(IReadOnlyCollection<UnvalidatedProductQuantity> productlist, string reason) : ICarucior;

        public record ValidatedCarucior(IReadOnlyCollection<ValidatedProduct> productlist) : ICarucior;

        public record PaidCarucior(IReadOnlyCollection<ValidatedProduct> productList, DateTime publishedDate) : ICarucior;
       
    }
}
