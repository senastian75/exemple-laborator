using Exemple.Domain.Models;
using static Exemple.Domain.Models.PaidCaruciorEvent;
using static Exemple.Domain.PriceOperations;
using System;
using static Exemple.Domain.Models.Carucior;

namespace Exemple.Domain
{
    public class PublishProductWorkflow
    {
        public IPaidCarucioredEvent Execute(PublishQuantityCommand command, Func<ProductCode, bool> checkProductExists)
        {
            UnvalidatedProduct unvalidatedProducts = new UnvalidatedProduct(command.InputQuantity);
            ICarucior products = ValidateProduct(checkProductExists, unvalidatedProducts);
            products = CalculatePrice(products);
            products = PaidCarucior(products);

            return products.Match(
                    whenUnvalidatedProduct: unvalidatedProduct => new PaidCaruciorFaildEvent("Unexpected unvalidated state") as IPaidCarucioredEvent,
                    whenInvalidatedCarucior: invalidatedProduct => new PaidCaruciorFaildEvent(invalidatedProduct.Reason),
                    whenValidatedCarucior: validatedProduct => new PaidCaruciorFaildEvent("Unexpected validated state"),
                    whenCalculatePrice: calculatedProduct => new PaidCaruciorFaildEvent("Unexpected calculated state"),
                    whenPaidCarucior: paidProduct => new PaidCaruciorScucceededEvent(paidProduct.PublishedDate)
                );
        }
    }
}
