using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swagger.Net.WebApi.Models;

namespace Swagger.Net.WebApi.Controllers
{
    public class BlogPostsController : ApiController
    {
        /// <summary>
        /// BlogPostsController.Get Summary
        /// </summary>
        /// <remarks>Here are some operation remarks</remarks>
        /// <returns>
        /// <see cref="BlogPost"/>a collection of blog posts
        /// </returns>
        public IEnumerable<BlogPost> Get()
        {
            return new List<BlogPost> { new BlogPost(), new BlogPost() };
        }

        /// <summary>
        /// BlogPostsController.Get(id) Summary
        /// </summary>
        /// <returns>
        /// <see cref="BlogPost"/>
        /// </returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Post summary
        /// </summary>
        /// <remarks>This shows up as notes</remarks>
        /// <param name="blogPost"><see cref="BlogPost"/>        
        /// <example>  
        /// This sample shows how to call the post method.
        /// <code>  
        /// { 
        ///     Title: 'some title',
        ///     Author: 'Philip Roth' 
        /// } 
        /// </code> 
        /// </example>
        /// </param>
        /// <returns><see cref="HttpResponseMessage"/>Status code 200 for ok</returns>
        public HttpResponseMessage Post(BlogPost blogPost)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// BlogPostsController.Put
        /// </summary>
        /// <param name="id"><see cref="Int32"/></param>
        /// <param name="value"><see cref="BlogPost"/>The post to put to database</param>
        public void Put(int id, BlogPost value)
        {
        }

        // DELETE api/blogposts/5
        public void Delete(int id)
        {
        }
    }
}
