using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swagger.Net.WebApi.Models
{
    /// <summary>
    /// Blog Post Class Summary
    /// </summary>
    /// <remarks>
    /// Blog Post Class Remarks
    /// </remarks>
    public class BlogPost
    {
        /// <summary>
        /// BlogPost.Title Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.Title Remarks
        /// </remarks>
        public string Title { get; set; }

        /// <summary>
        /// BlogPost.Id Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.Id Remarks
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// BlogPost.PublishDate Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.PublishDate Remarks
        /// </remarks>
        public DateTime PublishDate { get; set; }


        /// <summary>
        /// BlogPost.Author Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.Author Remarks
        /// </remarks>
        public string Author { get; set; }
    }
}