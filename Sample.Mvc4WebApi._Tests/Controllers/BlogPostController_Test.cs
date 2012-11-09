using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sample.Mvc4WebApi;
using Sample.Mvc4WebApi.Controllers;
using Sample.Mvc4WebApi.Models;
using Swagger.Net.WebApi.Controllers;


namespace Sample.Mvc4WebApi._Tests.Controllers
{
    [TestClass]
    public class BlogPostController_Test
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            var controller = new BlogPostsController();

            // Act
            var result = controller.Get();

            // Assert

            Debug.WriteLine(JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            var controller = new BlogPostsController();

            // Act
            var result = 1;/// controller.Get(new[]{1,2});

            // Assert
            Debug.WriteLine(JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BlogPost));
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            var controller = new BlogPostsController();
            var post = new BlogPost()
                           {
                               Id = 3,
                               PublishDate = DateTime.Now,
                               Title = "title3",
                               Author = new Person() {First = "Bill", Last = "Berger", Id = 12}
                           };
            // Act
            var result  = controller.Post(post);

            // Assert
            Debug.WriteLine(JsonConvert.SerializeObject(result));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpResponseMessage));
            Assert.AreEqual( HttpStatusCode.OK, result.StatusCode, "Status Code");
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            var controller = new BlogPostsController();
            var post = new BlogPost()
            {
                Id = 3,
                PublishDate = DateTime.Now,
                Title = "title3",
                Author = new Person() { First = "Bill", Last = "Berger", Id = 12 }
            };

            // Act
            controller.Put(3, post);

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            var controller = new BlogPostsController();

            // Act
            controller.Delete(3);

            // Assert
        }
    }
}
