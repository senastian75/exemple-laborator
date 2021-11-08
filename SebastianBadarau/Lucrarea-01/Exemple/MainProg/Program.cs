using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using static Exemple.Domain.Models.Carucior;
using Exemple.Domain;

namespace MainProgram
{
    class Program
    {
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            var listOfProducts = ReadListOfProducts().ToArray();
            CosGol Cos = new(listOfProducts);
            ICarucior result = CheckCos(Cos);

            result.Match(
                whenCosGol: cosGol => cosGol,
                whenInvalidatedCarucior: invalidatedCarucior => invalidatedCarucior,
                whenValidatedCarucior: validatedCos => CosPlatit(validatedCos, DateTime.Now),
                whenPaidCarucior: paidCarucior => paidCarucior
                );

            Console.WriteLine(result);
        }

        private static ICarucior CheckCos(CosGol unvalidatedCos) =>
             ((!CheckProducts(unvalidatedCos)) ? new CosGol(new List<UnvalidatedProductQuantity>())
                : (!CheckAddress(unvalidatedCos)) ? new InvalidatedCarucior(new List<UnvalidatedProductQuantity>(), "Cos Invalid")
                    : (!PayProducts(unvalidatedCos) ? new ValidatedCarucior(new List<ValidatedProduct>())
                         :new PaidCarucior(new List<ValidatedProduct>(), DateTime.Now)));

        public static bool PayProducts(CosGol unvalidatedCos)
        {
            string paymentState = ReadValue("Platesti? [Y/N]");
            if (paymentState.Contains("Y")) return true;
            else return false;
        }

        public static bool CheckProducts(CosGol unvalidatedCos)
        {
            foreach (var prod in unvalidatedCos.productList)
            {
                if (string.IsNullOrEmpty(prod.cod))         return false;
                if (string.IsNullOrEmpty(prod.quantity))    return false;
            }
            return true;
        }
        public static bool CheckAddress(CosGol unvalidatedCos)
        {
            foreach (var prod in unvalidatedCos.productList)
            {
                if (string.IsNullOrEmpty(prod.address)) return false;
            }
            return true;
        }
        private static ICarucior CosPlatit(ValidatedCarucior validatedResult, DateTime PublishedDate) =>
            new PaidCarucior(new List<ValidatedProduct>(), DateTime.Now);

        private static List<UnvalidatedProductQuantity> ReadListOfProducts()
        {
            List<UnvalidatedProductQuantity> listOfProducts = new();
            string flag = "N";
            do
            {
                var productCode = ReadValue("Introduce Product Code: ");
                
                var quantity = ReadValue("Quantity: ");
                
                var address = ReadValue("Address: ");

                listOfProducts.Add(new(productCode, quantity, address));
                flag = ReadValue("Do you want to add more items [Y/N]");
            } while (flag != "N");
            return listOfProducts;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
