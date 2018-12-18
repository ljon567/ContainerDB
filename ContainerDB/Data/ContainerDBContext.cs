using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContainerDB.Models
{
    public class ContainerDBContext : DbContext
    {
        public ContainerDBContext (DbContextOptions<ContainerDBContext> options)
            : base(options)
        {
        }

        public DbSet<ContainerDB.Models.LocationsItem> LocationsItem { get; set; }
        public DbSet<ContainerDB.Models.ContainerItem> ContainerItem { get; set; }
        public DbSet<ContainerDB.Models.UserItem> UserItem { get; set; }
    }
}
