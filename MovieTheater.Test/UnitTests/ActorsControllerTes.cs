using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieTheater.Controllers;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using MovieTheater.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test.UnitTests
{
    [TestClass]
    public class ActorsControllerTes : BaseTests
    {
        [TestMethod]
        public async Task GetActorsPaginated()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();
            await context.Actors.AddRangeAsync(new List<Actor> {
                new Actor{Name = "Actor 1"},
                new Actor{Name = "Actor 2"},
                new Actor{Name = "Actor 3"},
            });

            await context.SaveChangesAsync();
            var context2 = BuildContext(dbName);

            var controller = new ActorsController(context2, mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var page1 = await controller.Get(new PaginationDTO { Page = 1, RecordPerPage = 2 });
            var resultPage1 = page1.Value;
            Assert.AreEqual(2, resultPage1.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var page2 = await controller.Get(new PaginationDTO { Page = 2, RecordPerPage = 2 });
            var resultPage2 = page2.Value;
            Assert.AreEqual(1, resultPage2.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var page3 = await controller.Get(new PaginationDTO { Page = 3, RecordPerPage = 2 });
            var resultPage3 = page3.Value;
            Assert.AreEqual(0, resultPage3.Count);
        }

        [TestMethod]
        public async Task CreateActorWithoutPhoto()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var mock = new Mock<IFileStorage>();
            mock.Setup(m => m.SaveFileAsync(null, null, null, null))
                .Returns(Task.FromResult("url"));

            var controller = new ActorsController(context, mapper, mock.Object);
            var newActorCreateDTO = new ActorCreateDTO { Name = "new actor", DateOfBirth = DateTime.Now };

            var response = await controller.Post(newActorCreateDTO);
            var result = response as CreatedAtRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);

            var context2 = BuildContext(dbName);
            var results = await context2.Actors.ToListAsync();
            Assert.AreEqual(results.Count, 1);
            Assert.IsNull(results[0].Photo);

            // Making sure that the mock never was invoked
            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CreateActorWithPhoto()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var content = Encoding.UTF8.GetBytes("this a test");
            var file = new FormFile(new MemoryStream(content), 0, content.Length, "data", "picture.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";
            var newActorCreateDTO = new ActorCreateDTO 
            { 
                Name = "new actor", 
                DateOfBirth = DateTime.Now,
                PhotoFile = file
            };

            var mock = new Mock<IFileStorage>();
            mock.Setup(m => m.SaveFileAsync(content, ".jpg", "Actors", file.ContentType))
                .Returns(Task.FromResult("url"));

            var controller = new ActorsController(context, mapper, mock.Object);
            var response = await controller.Post(newActorCreateDTO);
            var result = response as CreatedAtRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);

            var context2 = BuildContext(dbName);
            var actors = await context2.Actors.ToListAsync();
            Assert.AreEqual(1, actors.Count);
            Assert.AreEqual("url", actors[0].Photo);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task PatchReturn404IfNotExists()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new ActorsController(context, mapper, null);
            var patchDto = new JsonPatchDocument<ActorPatchDTO>();
            var response = await controller.Patch(1, patchDto);
            var result = response as StatusCodeResult;
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task PatchUpdateOneField()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var dateOfBirth = DateTime.Now;
            var actor = new Actor { Name = "actor 1", DateOfBirth = dateOfBirth };
            await context.Actors.AddAsync(actor);
            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);
            var controller = new ActorsController(context2, mapper, null);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(ov => ov.Validate(It.IsAny<ActionContext>(),
               It.IsAny<ValidationStateDictionary>(),
               It.IsAny<string>(),
               It.IsAny<object>()));
            controller.ObjectValidator = objectValidator.Object;
            var patchDto = new JsonPatchDocument<ActorPatchDTO>();
            var id = 1;
            patchDto.Operations.Add(new Operation<ActorPatchDTO>("replace", "/Name", null, "Actor"));
            var response = await controller.Patch(id, patchDto);
            var result = response as StatusCodeResult;
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            var context3 = BuildContext(dbName);
            var actorDb = await context3.Actors.FindAsync(id);
            Assert.AreNotEqual(actor.Name, actorDb.Name);
            Assert.AreEqual("Actor", actorDb.Name);
            Assert.AreEqual(actorDb.DateOfBirth, dateOfBirth);
        }
    }
}
