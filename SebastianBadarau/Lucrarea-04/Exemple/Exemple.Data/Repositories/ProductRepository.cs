using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Data.Repositories
{
    public class ProductRepository : IProductsRepository
    {
        private readonly CosContext cosContext;

        public ProductRepository(CosContext cosContext)
        {
            this.cosContext = cosContext;
        }

        public TryAsync<IEnumerable<Product>> TryGetProductIDstock(IEnumerable<string> IDsToCheck) => async () =>
        {
            var products = await cosContext.Product
                                              .Where(product => IDsToCheck.Contains(product.ProductID))
                                              .AsNoTracking()
                                              .ToListAsync();
            var ret = products.Select(product => new Product(product.ProductID,product.Stock,product.Price))
                           .ToList();
            return ret;
        };
    }
}

