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
        public interface ICarucior { };
        

        public record UnvalidatedProduct : ICarucior
        {
            public UnvalidatedProduct(IReadOnlyCollection<UnvalidatedProductQuantity> productList)
            {
                ProductList = productList;
            }
            public IReadOnlyCollection<UnvalidatedProductQuantity> ProductList { get; }
        }
        public record InvalidatedCarucior : ICarucior
        {
            internal InvalidatedCarucior(IReadOnlyCollection<UnvalidatedProductQuantity> productlist, string reason)
            {
                ProductList = productlist;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidatedProductQuantity> ProductList { get; }
            public string Reason { get; }
        }
        public record ValidatedCarucior : ICarucior
        {
            internal ValidatedCarucior(IReadOnlyCollection<ValidatedProduct> productlist)
            {
                ProductList = productlist;
            }
            public IReadOnlyCollection<ValidatedProduct> ProductList { get; }
        }
        public record CalculatePrice : ICarucior
        {
            internal CalculatePrice(IReadOnlyCollection<CalculatedPrice> productlist)
            {
                ProductList = productlist;
            }
            public IReadOnlyCollection<CalculatedPrice> ProductList { get; }
        }
        public record PaidCarucior : ICarucior
        {
            internal PaidCarucior(IReadOnlyCollection<CalculatedPrice> productList, DateTime publishedDate, string csv)
            {
                ProductList = productList;
                PublishedDate = publishedDate;
                Csv = csv;
            }
            public IReadOnlyCollection<CalculatedPrice> ProductList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
