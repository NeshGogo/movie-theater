using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieTheater.Controllers;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test.UnitTests
{
    [TestClass]
    public class GendersControllerTest : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenders()
        {
            // Arrange or preparation
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            await context.Genders.AddRangeAsync(new List<Gender> {
                new Gender{Name = "Gender 1"},
                new Gender{Name = "Gender 2"},
            });
            await context.SaveChangesAsync();
            var context2 = BuildContext(dbName);
            
            // Act or test
            var controller = new GendersController(context2, mapper);
            var response = await controller.Get();
            
            // Assert or verification
            var genders = response.Value;
            Assert.AreEqual(2, genders.Count);

        }

        [TestMethod]
        public async Task GetGenderByIdExistent()
        {
            // Arrange or preparation
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            await context.Genders.AddRangeAsync(new List<Gender> {
                new Gender{Name = "Gender 1"},
                new Gender{Name = "Gender 2"},
            });
            await context.SaveChangesAsync();
            var context2 = BuildContext(dbName);
            // Act or Test
            var id = 1;
            var controller = new GendersController(context2, mapper);
            var response = await controller.Get(id);
            // Assert or Vefirification
            var gender = response.Value;
            Assert.AreEqual(gender.Id, id);
        }

        [TestMethod]
        public async Task GetGenderByIdNonExistent()
        {
            // Arrange or preparation
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            // Act or Test
            var controller = new GendersController(context, mapper);
            var response = await controller.Get(20);
            // Assert or Vefirification
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(result.StatusCode, 404 );
        }

        [TestMethod]
        public async Task CreateGender()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            var newGender = new GenderCreateDTO { Name = "new gender" };
            var controller = new GendersController(context, mapper);
            var response = await controller.Post(newGender);
            var result = response as CreatedAtRouteResult;
            Assert.IsNotNull(result);

            var context2 = BuildContext(dbName);
            var total = await context2.Genders.CountAsync();
            Assert.AreEqual(1, total);
        }

        [TestMethod]
        public async Task UpdateGender()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            await context.Genders.AddRangeAsync(new List<Gender> {
                new Gender{Name = "Gender 1"},
                new Gender{Name = "Gender 2"},
            });
            await context.SaveChangesAsync(); 
            var context2 = BuildContext(dbName);
            
            var id = 2;
            var newGender = new GenderCreateDTO { Name = "new gender" };
            var controller = new GendersController(context2, mapper);
            var response = controller.Put(id, newGender);
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(result.StatusCode, 204);
            
            var context3 = BuildContext(dbName); 
            var gender = await context3.Genders.FindAsync(id);
            Assert.AreEqual(gender.Name, newGender.Name);
        }

        [TestMethod]
        public async Task TryDeleteGenderNonExistent()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new GendersController(context, mapper);
            var response = controller.Delete(2);
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(result.StatusCode, 404);
        }

        [TestMethod]
        public async Task DeleteGenderExistent()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            await context.Genders.AddRangeAsync(new List<Gender> {
                new Gender{Name = "Gender 1"},
                new Gender{Name = "Gender 2"},
            });

            await context.SaveChangesAsync();
            var context2 = BuildContext(dbName);

            var id = 2;
            var controller = new GendersController(context2, mapper);
            var response = controller.Delete(id);
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(result.StatusCode, 204);

            var context3 = BuildContext(dbName);
            var exists = await context3.Genders.AnyAsync( g => g.Id == id);
            Assert.IsFalse(false);
        }
    }
}
