
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Swagger.Net.WebApi.Controllers
{
    /// <summary>
    /// QueryStringBugController class summary
    /// </summary>
    /// <remarks>
    /// QueryStringBugController class remarks
    /// </remarks>
    public class QueryStringBugController : ApiController
    {

        /// <summary>
        /// QueryStringBugController Summary
        /// </summary>
        /// <remarks>Here are some remarks about the Get by count</remarks>
        public IEnumerable<int> Get(int count)
        {
            return new int[] { count, count };
        }

        public void Post(int count)
        {
            
        }

        [HttpGet]
        public HttpResponseMessage Docs()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
