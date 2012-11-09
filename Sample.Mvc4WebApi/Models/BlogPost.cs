    using System;
    using System.Collections.Generic;
    using Sample.Mvc4WebApi.Models;

namespace Sample.Mvc4WebApi.Models
{
    /// <summary>
    /// Blog Post Class Summary
    /// </summary>
    /// <remarks>
    /// Blog Post Class Remarks
    /// </remarks>
    public class BlogPost
    {
        public BlogPost()
        {
            SomeNumbers=new List<int>();
            Data= new List<byte>().ToArray();
        }
        /// <summary>
        /// BlogPost.Title Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.Title Remarks
        /// </remarks>
        public string Title { get; set; }
        public Byte[] Data { get; set; }
        /// <summary>
        /// BlogPost.Id Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.Id Remarks
        /// </remarks>
        public int Id { get; set; }
        public List<int> SomeNumbers { get; set; }
        /// <summary>
        /// BlogPost.PublishDate Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.PublishDate Remarks
        /// </remarks>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// BlogPost.Author (Person) Summary
        /// </summary>
        /// <remarks>
        /// BlogPost.Author (Person) Remarks
        /// </remarks>
        public Person Author { get; set; }

 
    }
}