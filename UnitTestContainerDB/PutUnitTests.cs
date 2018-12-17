using ContainerDB.Controllers;
using ContainerDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UnitTestContainerDB
{
    [TestClass]
    public class PutUnitTests
    {
        public static readonly DbContextOptions<ContainerDBContext> options
        = new DbContextOptionsBuilder<ContainerDBContext>()
        .UseInMemoryDatabase(databaseName: "testDatabase")
        .Options;
        public static IConfiguration configuration = null;
        public static readonly IList<string> profileNames = new List<string> { "Aragorn", "Goblin Slayer" };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new ContainerDBContext(options))
            {
                ProfileItem profileItem1 = new ProfileItem()
                {
                    Name = profileNames[0]
                };

                ProfileItem profileItem2 = new ProfileItem()
                {
                    Name = profileNames[1]
                };

                context.ProfileItem.Add(profileItem1);
                context.ProfileItem.Add(profileItem2);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new ContainerDBContext(options))
            {
                context.ProfileItem.RemoveRange(context.ProfileItem);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestPutProfileItemNoContentStatusCode()
        {
            using (var context = new ContainerDBContext(options))
            {
                // Given
                string name = "Shrek";
                ProfileItem profileItem1 = context.ProfileItem.Where(x => x.Name == profileNames[0]).Single();
                profileItem1.Name = name;

                // When
                ProfileController profileController = new ProfileController(context, configuration);
                IActionResult result = await profileController.PutProfileItem(profileItem1.Id, profileItem1) as IActionResult;

                // Then
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }

        [TestMethod]
        public async Task TestPutProfileItemUpdate()
        {
            using (var context = new ContainerDBContext(options))
            {
                // Given
                string name = "Name";
                ProfileItem profileItem1 = context.ProfileItem.Where(x => x.Name == profileNames[0]).Single();
                profileItem1.Name = name;

                // When
                ProfileController profileController = new ProfileController(context, configuration);
                IActionResult result = await profileController.PutProfileItem(profileItem1.Id, profileItem1) as IActionResult;

                // Then
                profileItem1 = context.ProfileItem.Where(x => x.Name == name).Single();
            }
        }
    }
}