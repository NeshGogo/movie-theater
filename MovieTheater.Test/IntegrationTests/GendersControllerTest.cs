using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test.IntegrationTests
{
    [TestClass]
    public class GendersControllerTest : BaseTests
    {
        private readonly string url = "/api/genders";

        [TestMethod]
        public async Task GetAllGenderEmptyList()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genders = JsonConvert.DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(0, genders.Count);
        }

        [TestMethod]
        public async Task GetAllGender()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);
            
            var context = BuildContext(dbName);
            await context.Genders.AddAsync(new Gender { Name = "Drama 1" });
            await context.Genders.AddAsync(new Gender { Name = "Drama 1" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genders = JsonConvert.DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(2, genders.Count);
        }

        [TestMethod]
        public async Task DeleteGender()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var context = BuildContext(dbName);
            await context.Genders.AddAsync(new Gender { Name = "Drama 1" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.DeleteAsync($"{url}/1");

            response.EnsureSuccessStatusCode();

            var context2 = BuildContext(dbName);
            var exists = await context2.Genders.AnyAsync();
            Assert.IsFalse(exists);
        }
    }
}
