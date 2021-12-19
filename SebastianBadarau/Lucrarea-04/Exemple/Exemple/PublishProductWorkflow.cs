using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Carucior;
using static Exemple.Domain.Models.PaidCaruciorEvent;
using static Exemple.Domain.PriceOperations;
using static LanguageExt.Prelude;

namespace Exemple.Domain
{
    public class PublishProductWorkflow
    {
        private readonly ICosRepository cosRepository;
        private readonly IProductsRepository productsRepository;
        private readonly ILogger<PublishProductWorkflow> logger;

        public PublishProductWorkflow(ICosRepository cosRepository, IProductsRepository productsRepository, ILogger<PublishProductWorkflow> logger)
        {
            this.cosRepository = cosRepository;
            this.productsRepository = productsRepository;
            this.logger = logger;
        }

        public async Task<IPaidCarucioredEvent> ExecuteAsync(PublishQuantityCommand command)
        {
            UnvalidatedProduct unvalidatedProduct = new UnvalidatedProduct(command.InputQuantity);
           

            var result = from ProductIDstocks in productsRepository.TryGetProductIDstock(unvalidatedProduct.ProductList.Select(product => product.cod))
                                          .ToEither(ex => new FailedCarucior(unvalidatedProduct.ProductList, ex) as ICarucior)
                         
                         let checkProdusIDExists = (Func<ProductCode, Option<ProductCode>>)(ProductID => CheckIDExists(ProductIDstocks.Select(product => product.ProductCode), ProductID))

                         from publishedCos in ExecuteWorkflowAsync(unvalidatedProduct, ProductIDstocks, checkProdusIDExists).ToAsync()

                         from _ in cosRepository.TrySaveCos(publishedCos)
                                                           .ToEither(ex => new FailedCarucior(unvalidatedProduct.ProductList, ex) as ICarucior)

                         select publishedCos;

            return await result.Match(
                    Left: cos => GenerateFailedEvent(cos) as IPaidCarucioredEvent,
                    Right: paidProduct => new PaidCaruciorScucceededEvent(paidProduct.Csv, paidProduct.PublishedDate)
                );
        }
        private async Task<Either<ICarucior, PublishedCarucior>> ExecuteWorkflowAsync(UnvalidatedProduct unvalidatedProduct,
                                                                                      IEnumerable<Product> productStocks,
                                                                                      Func<ProductCode, Option<ProductCode>> checkProdusIDExists)
        {
            ICarucior products = await ValidateCarucior(checkProdusIDExists, unvalidatedProduct);
            products = CalculatePrices(products, productStocks);
            products = PaidCarucior(products);

            return products.Match<Either<ICarucior, PublishedCarucior>>(
                whenUnvalidatedProduct:     unvalidatedProduct  => Left(unvalidatedProduct as ICarucior),
                whenInvalidatedCarucior:    invalidatedProduct  => Left(unvalidatedProduct as ICarucior),
                whenValidatedCarucior:      validatedCarucior   => Left(validatedCarucior as ICarucior),
                whenFailedCarucior:         failedCarucior      => Left(failedCarucior as ICarucior),
                whenCalculatePrice:         calculatedProduct   => Left(unvalidatedProduct as ICarucior),
                whenPublishedCarucior:      publishedCarucior   => Right(publishedCarucior)
                );
        }
        private Option<ProductCode> CheckIDExists(IEnumerable<string> ProductIDs, ProductCode ProductID)
        {
            if (ProductIDs.Any(s => s == ProductID.ToString()))
            {
                return Some(ProductID);
            }
            else
            {
                return None;
            }
        }

        private PaidCaruciorFaildEvent GenerateFailedEvent(ICarucior cos)
        {
            return cos.Match<PaidCaruciorFaildEvent>(
                whenUnvalidatedProduct:     unvalidatedProduct  => new($"Invalid state{nameof(UnvalidatedProduct) }"),
                whenInvalidatedCarucior:    invalidatedProduct  => new($"Invalid state{nameof(InvalidatedCarucior) }"),
                whenFailedCarucior:         failedCarucior      =>
                {
                    logger.LogError(failedCarucior.Exception, failedCarucior.Exception.Message);
                    return new(failedCarucior.Exception.Message);
                },
                whenValidatedCarucior:      validatedProduct    => new($"Invalid state{nameof(ValidatedCarucior)}"),
                whenCalculatePrice:         calculatedProduct   => new($"Invalid state{nameof(CalculatePrice)}"),
                whenPublishedCarucior:      publishedCarucior   => new($"Invalid state{nameof(PublishedCarucior)}")
                );
        }


    }

}

