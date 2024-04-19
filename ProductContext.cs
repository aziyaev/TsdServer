using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TsdServer
{
    public class ProductContext : DbContext
    {
        public Microsoft.EntityFrameworkCore.DbSet<Product> Products { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=azcsmnt;Database=tsddb;User Id=tsdDBTest;Password=qwertyuiop;TrustServerCertificate=True;");
        }
    }
}
