using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Carucior;

namespace Exemple.Domain
{
    public static class PriceOperations
    {
        public static ICarucior ValidateProduct(Func<ProductCode, bool> checkProductExists, UnvalidatedProduct product)
        {
            List<ValidatedProduct> validatedProduct = new();
            bool isValidList = true;
            string invalidReson = string.Empty;
            foreach (var unvalidatedProduct in product.ProductList)
            {
                if (!Quantity.TryParseQuantity(unvalidatedProduct.quantity, out Quantity quantity))
                {
                    invalidReson = $"Invalid product quantity ({unvalidatedProduct.cod}, {unvalidatedProduct.quantity})";
                    isValidList = false;
                    break;
                }
                if (!Adresa.TryParse(unvalidatedProduct.address, out Adresa adress))
                {
                    invalidReson = $"Invalid address ({unvalidatedProduct.cod}, {unvalidatedProduct.address})";
                    isValidList = false;
                    break;
                }
                if (!ProductCode.TryParse(unvalidatedProduct.cod, out ProductCode code))
                {
                    invalidReson = $"Invalid product code ({unvalidatedProduct.cod})";
                    isValidList = false;
                    break;
                }
                ValidatedProduct validProduct = new(code, quantity, adress);
                validatedProduct.Add(validProduct);
            }

            if (isValidList)
            {
                return new ValidatedCarucior(validatedProduct);
            }
            else
            {
                return new InvalidatedCarucior(product.ProductList, invalidReson);
            }

        }

        public static ICarucior CalculatePrice(ICarucior products) => products.Match(
            whenUnvalidatedProduct: unvalidaTedProduct => unvalidaTedProduct,
            whenInvalidatedCarucior: invalidProduct => invalidProduct,
            whenCalculatePrice: calculatedPrice => calculatedPrice,
            whenPaidCarucior: paidCarucior => paidCarucior,
            whenValidatedCarucior: validProduct =>
            {
                var calculatedPrice = validProduct.ProductList.Select(validProduct =>
                                                  new CalculatedPrice(validProduct.ProductCode,
                                                                      validProduct.Quantity,
                                                                      validProduct.address,
                                                                      Convert.ToInt32(validProduct.Quantity) * 10 ));
                return new CalculatePrice(calculatedPrice.ToList().AsReadOnly());
            }
        );

        public static ICarucior PaidCarucior(ICarucior products) => products.Match(
            whenUnvalidatedProduct: unvalidaTedProduct => unvalidaTedProduct,
            whenInvalidatedCarucior: invalidProduct => invalidProduct,
            whenValidatedCarucior: validatedCarucior => validatedCarucior,
            whenPaidCarucior: paidCarucior => paidCarucior,
            whenCalculatePrice: calculatedPrice =>
            {
                StringBuilder csv = new();

                calculatedPrice.ProductList.Aggregate(csv, (export, produs) => export.AppendLine($"{produs.ProductCode.Value}, {produs.Quantity}, {produs.price}, {produs.address} "));

                PaidCarucior paidCarucior = new(calculatedPrice.ProductList, DateTime.Now, csv.ToString());

                Console.WriteLine(csv);

                return paidCarucior;
            });
    }
}
