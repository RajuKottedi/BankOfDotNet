using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankOfDotNetAPI.Models
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
