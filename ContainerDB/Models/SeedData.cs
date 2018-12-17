using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerDB.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ContainerDBContext(
                serviceProvider.GetRequiredService<DbContextOptions<ContainerDBContext>>()))
            {
                //DB has been seeded already
                if (context.LocationsItem.Count() > 0)
                {
                    return;   
                }

                context.LocationsItem.AddRange(
                    new LocationsItem
                    {
                        ContainerID = "Patrick the Star",
                        Tauranga = "Tauranga",
                        Lyttleton = "Lyttleton",
                        Timaru = "Timaru",
                        Otago = "Otago",
                        Kiwirail = "Kiwirail",
                        PortConnect = "PortConnect"
                    }


                );
                context.SaveChanges();

                if (context.ContainerItem.Count() > 0)
                {
                    return;
                }

                context.ContainerItem.AddRange(
                    new ContainerItem
                    {
                        ContainerID = "Patrick the Star",
                        shipco = "Maersk",
                        size = "20 feet",
                        grade = 9,
                        location = "Penrose",
                        full = false,
                        status = "Hired",
                        time = "18/12/2018",
                        type = "insulated"
                    }


                );
                context.SaveChanges();
            }
        }
    }
}
