
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
