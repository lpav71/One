using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using One.Models;

namespace One.Data
{
    public class OneContext : DbContext
    {
        public OneContext (DbContextOptions<OneContext> options)
            : base(options)
        {
        }

        public DbSet<One.Models.Product> Product { get; set; }
    }
}
