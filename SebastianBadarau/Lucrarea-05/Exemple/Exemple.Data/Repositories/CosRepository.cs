
using LanguageExt;
using Exemple.Data.Models;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using Exemple.Domain.Repositories;
using Exemple.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Exemple.Data.Repositories
{
    public class CosRepository : ICosRepository
    {
        private readonly CosContext dbContext;
        public CosRepository(CosContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public TryAsync<List<OrderView>> TryGetExistingCos() => async () =>
        {
            var products = await dbContext.Cos
                           .AsNoTracking()
                           .ToListAsync();

            var qry = products.GroupBy(s => new { s.OrderID, s.Address})
                              .Select( g => new { 
                                  OrderID = g.Key.OrderID, 
                                  Price   = g.Sum(xt => xt.Price), 
                                  Address = g.Key.Address });

            List<OrderView> list = new List<OrderView>();

            foreach (var o in qry)
            {
                list.Add(new OrderView(o.OrderID, o.Price, o.Address));
            }
            return list;
            /*
            var ret = products.Select(product => new OrderProducts(
                id: product.id,
                OrderID: product.OrderID.ToString(),
                ProductID: product.ProductID.ToString(),
                Quantity: product.Quantity,
                Price: product.Price,
                Address: product.Address.ToString()
            ))
            .ToList();
            */
        };

        public TryAsync<Unit> TrySaveCos(Carucior.PublishedCarucior paidCarucior) => async () =>
        {
            var OrderID = new Random().Next(1000);
            var toAdd = paidCarucior.ProductList.Select(c => CosDto.ToCosDTO(OrderID, c)); 
            foreach (var c in toAdd)
            {
                dbContext.AddRange(c);
            }
            await dbContext.SaveChangesAsync();
            return unit;
        };
    }
}
