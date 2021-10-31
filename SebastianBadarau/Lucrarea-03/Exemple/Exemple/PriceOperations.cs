using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Carucior;
using static LanguageExt.Prelude;

namespace Exemple.Domain
{
    public static class PriceOperations
    {
        public static Task<ICarucior> ValidateCarucior(Func<ProductCode, TryAsync<bool>> checkProductExists, UnvalidatedProduct productList) =>
             productList.ProductList
                  .Select(ValidatedProduct(checkProductExists))
                  .Aggregate(CreateEmptyValidatedProductList().ToAsync(), ReduceValidCarucior)
                  .MatchAsync(
                        Right: validatedProducts => new ValidatedCarucior(validatedProducts),
                        LeftAsync: errorMessage => Task.FromResult((ICarucior)new InvalidatedCarucior(productList.ProductList, errorMessage))
                  );

        private static Func<UnvalidatedProductQuantity, EitherAsync<string, ValidatedProduct>> ValidatedProduct(Func<ProductCode, TryAsync<bool>> checkProductExists) =>
            unvalidatedProduct => ValidatedProduct(checkProductExists, unvalidatedProduct);

        private static EitherAsync<string, ValidatedProduct> ValidatedProduct(Func<ProductCode, TryAsync<bool>> checkStudentExists, UnvalidatedProductQuantity unvalidatedProduct) =>
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

        public static ICarucior CalculatePrices(ICarucior products) => products.Match(
              whenUnvalidatedProduct: unvalidaTedProduct => unvalidaTedProduct,
              whenInvalidatedCarucior: invalidProduct => invalidProduct,
              whenCalculatePrice: calculatedPrice => calculatedPrice,
              whenPaidCarucior: paidCarucior => paidCarucior,
              whenValidatedCarucior: CalculateFinalPrices
        );

        private static ICarucior CalculateFinalPrices(ValidatedCarucior validProduct) =>
                  new CalculatePrice(validProduct.ProductList
                                                 .Select(CalculateProductPrices)
                                                 .ToList()
                                                 .AsReadOnly());
        private static CalculatedPrice CalculateProductPrices(ValidatedProduct validProduct) =>
                                                new CalculatedPrice(validProduct.ProductCode,
                                                                    validProduct.Quantity,
                                                                    validProduct.address,
                                                                    Convert.ToInt32(validProduct.Quantity) * 10);

        public static ICarucior PaidCarucior(ICarucior products) => products.Match(
            whenUnvalidatedProduct: unvalidatedProduct => unvalidatedProduct,
            whenInvalidatedCarucior: invalidProduct => invalidProduct,
            whenValidatedCarucior: validatedCarucior => validatedCarucior,
            whenPaidCarucior: paidCarucior => paidCarucior,
            whenCalculatePrice: GenerateExport
            );

        private static ICarucior GenerateExport(CalculatePrice calculatedPrice) =>
            new PaidCarucior(calculatedPrice.ProductList, DateTime.Now, calculatedPrice.ProductList.Aggregate(new StringBuilder(), CreateCsvLine).ToString());

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedPrice Cos) =>
            export.AppendLine($"{Cos.ProductCode.Value}, {Cos.Quantity}, {Cos.price}, {Cos.price}");

    }
}

