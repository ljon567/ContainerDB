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
                        Auckland = "Auckland"
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
                        ISO = "2510",
                        grade = 9,
                        location = "Penrose",
                        status = "Full",
                        time = "18/12/2018",
                        booking = "ABC123",
                        vessel = "Davy Jones Locker",
                        loadPort = "Bikini Bottom",
                        weight = "9000",
                        category = "Export",
                        seal = "NZNZ0101",
                        commodity = "starfish"
                    }


                );
                context.SaveChanges();

                if (context.UserItem.Count() > 0)
                {
                    return;
                }

                context.UserItem.AddRange(
                    new UserItem
                    {
                        username = "Patrick the Star",
                        password = "password"
                    }


                );
                context.SaveChanges();
            }
        }
    }
}
