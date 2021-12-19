using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using static Exemple.Domain.Models.Carucior;
using Exemple.Domain.Repositories;
using Exemple.Domain;
using Example.Api.Models;
using Exemple.Domain.Models;

namespace Example.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CosController : Controller
    {
        private ILogger<CosController> logger;

        public CosController(ILogger<CosController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrades([FromServices] ICosRepository cosRepository) =>
            await cosRepository.TryGetExistingCos().Match(
               Succ: GetAllCosHandleSuccess,
               Fail: GetAllGradesHandleError
            );

        private ObjectResult GetAllGradesHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllCosHandleSuccess(List<Exemple.Domain.Models.OrderView> cos) =>
        Ok(cos.Select(cos => new
        {
            cos.OrderID,cos.Price, cos.Address
        }));

        [HttpPost]
        public async Task<IActionResult> PublishGrades([FromServices] PublishProductWorkflow publishProductWorkflow, [FromBody] InputProducts[] products)
        {
            var unvalidatedGrades = products.Select(MapInputProductsToUnvalidatedGrade)
                                          .ToList()
                                          .AsReadOnly();
            PublishQuantityCommand command = new(unvalidatedGrades);
            var result = await publishProductWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenPaidCaruciorFaildEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenPaidCaruciorScucceededEvent: successEvent => Ok()
            );
        }

        private static UnvalidatedProductQuantity MapInputProductsToUnvalidatedGrade(InputProducts products) => new UnvalidatedProductQuantity(
            cod: products.ProductID,
            quantity: products.Quantity.ToString(),
            address: products.Address);
    }
}

