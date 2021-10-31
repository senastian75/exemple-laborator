using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using static Exemple.Domain.Models.Carucior;
using static Exemple.Domain.PriceOperations;
using Exemple.Domain;

namespace MainProgram
{
    class Program
    {
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            var listOfProducts = ReadListOfProducts().ToArray();
            PublishQuantityCommand command = new(listOfProducts);
            PublishProductWorkflow workflow = new PublishProductWorkflow();
            var result = workflow.Execute(command, (productCode) => true);

            result.Match(
                    whenPaidCaruciorFaildEvent: @event =>
                    {
                        Console.WriteLine($"Publish failed: {@event.Reason}");
                        return @event;
                    },
                    whenPaidCaruciorScucceededEvent: @event =>
                    {
                        Console.WriteLine($"Publish succeeded.");
                        return @event;
                    }
                );

            Console.WriteLine("Try again later!");
        }

        private static List<UnvalidatedProductQuantity> ReadListOfProducts()
        {
            List<UnvalidatedProductQuantity> listOfProducts = new();
            string flag = "N";
            do
            {
                var productCode = ReadValue("Introduce Product Code: ");
                while (string.IsNullOrEmpty(productCode))
                {
                    productCode = ReadValue("Product Code must not be blank: ");
                }

                var quantity = ReadValue("Quantity: ");
                while (string.IsNullOrEmpty(quantity))
                {
                    quantity = ReadValue("Quantity must not be blank: ");
                }

                var address = ReadValue("Address: ");
                while (string.IsNullOrEmpty(address))
                {
                    address = ReadValue("Address must not be blank: ");
                }

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
