using Exemple.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Data
{
    public class CosContext : DbContext
    {
        public CosContext(DbContextOptions<CosContext> options) : base(options)
        {
        }
        public DbSet<ProductDto> Product { get; set; }

        public DbSet<CosDto> Cos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDto>().ToTable("ProductDta").HasKey(s => s.ProductID);
            modelBuilder.Entity<CosDto>().ToTable("CosDta").HasKey(s => s.id);
        }
    }
}
