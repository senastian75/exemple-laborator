using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Carucior;
using static LanguageExt.Prelude;
using System.Collections;
using System.Text;

namespace Exemple.Domain
{
    public static class PriceOperations
    {
        public static Task<ICarucior> ValidateCarucior(Func<ProductCode, Option<ProductCode>> checkIDExists, UnvalidatedProduct productList) =>
             productList.ProductList
                  .Select(ValidatedProduct(checkIDExists))
                  .Aggregate(CreateEmptyValidatedProductList().ToAsync(), ReduceValidCarucior)
                  .MatchAsync(
                        Right: validatedProducts => new ValidatedCarucior(validatedProducts),
                        LeftAsync: errorMessage => Task.FromResult((ICarucior)new InvalidatedCarucior(productList.ProductList, errorMessage))
                  );

        private static Func<UnvalidatedProductQuantity, EitherAsync<string, ValidatedProduct>> ValidatedProduct(Func<ProductCode, Option<ProductCode>> checkIDExists) =>
            unvalidatedProduct => ValidatedProduct(checkIDExists, unvalidatedProduct);

        private static EitherAsync<string, ValidatedProduct> ValidatedProduct(Func<ProductCode, Option<ProductCode>> checkIDExists, UnvalidatedProductQuantity unvalidatedProduct) =>
            from productCode in ProductCode.TryParseProductCode(unvalidatedProduct.cod)
                                   .ToEitherAsync(() => $"Invalid product code ({unvalidatedProduct.cod})")
            from productQuantity in Quantity.TryParseQuantity(unvalidatedProduct.quantity)
                                   .ToEitherAsync(() => $"Invalid product quantity ({unvalidatedProduct.cod}, {unvalidatedProduct.quantity})")
            from address in Adresa.TryParseAdresa(unvalidatedProduct.address)
                                   .ToEitherAsync(() => $"Invalid student registration number ({unvalidatedProduct.address})")
            select new ValidatedProduct(productCode, productQuantity, address);

        private static Either<string, List<ValidatedProduct>> CreateEmptyValidatedProductList() =>
            Right(new List<ValidatedProduct>());

        private static EitherAsync<string, List<ValidatedProduct>> ReduceValidCarucior(EitherAsync<string, List<ValidatedProduct>> acc, EitherAsync<string, ValidatedProduct> next) =>
            from list in acc
            from nextProduct in next
            select list.AppendValidProduct(nextProduct);

        private static List<ValidatedProduct> AppendValidProduct(this List<ValidatedProduct> list, ValidatedProduct validProduct)
        {
            list.Add(validProduct);
            return list;
        }

        public static ICarucior CalculatePrices(ICarucior products, IEnumerable<Product> productStocks) =>
            products.Match(
                whenUnvalidatedProduct: unvalidatedproduct => unvalidatedproduct,
                whenInvalidatedCarucior: invalidProduct => invalidProduct,
                whenCalculatePrice: calculatedPrice => calculatedPrice,
                whenFailedCarucior: failedProduct => failedProduct,
                whenPublishedCarucior: publishedProduct => publishedProduct,
                whenValidatedCarucior: validatedCarucior => CalculateFinalPrices(validatedCarucior.ProductList, productStocks)
                );


        private static ICarucior CalculateFinalPrices(IEnumerable<ValidatedProduct> validProducts, IEnumerable<Product> productStocks)
        {
            var querry = (from valid in validProducts
                          from stock in productStocks
                          where stock.ProductCode.Contains(valid.ProductCode.ToString())
                          select new CalculatedPrice(
                              valid.ProductCode,
                              valid.Quantity,
                              valid.address,
                              stock.price * Convert.ToInt32(valid.Quantity)));

            var ret = querry.GroupBy(x => x.productCode);

            return new CalculatePrice(ret.SelectMany(q => q));
        }


        public static ICarucior PaidCarucior(ICarucior products) => products.Match(
            whenUnvalidatedProduct: unvalidatedproduct => unvalidatedproduct,
            whenInvalidatedCarucior: invalidProduct => invalidProduct,
            whenFailedCarucior: failedProduct => failedProduct,
            whenValidatedCarucior: validatedCarucior=> validatedCarucior,
            whenPublishedCarucior: publishedProduct => publishedProduct,
            whenCalculatePrice:  GenerateExport);

        private static ICarucior GenerateExport(CalculatePrice cos) =>
           new PublishedCarucior(cos.ProductList.ToList().AsReadOnly(), DateTime.Now,
                                   cos.ProductList.Aggregate(new StringBuilder(), CreateCsvLine).ToString());


        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedPrice prod) =>
           export.AppendLine($"{prod.productCode}, {prod.quantity}, {prod.price}, {prod.address}");
    }
}

