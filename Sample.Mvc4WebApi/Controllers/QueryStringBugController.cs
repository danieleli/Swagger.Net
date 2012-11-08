
using System.Collections.Generic;
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
        /// QueryStringBugController.Get(Count) Summary
        /// </summary>
        /// <remarks>Here are some remarks about the Get by count</remarks>
        public IEnumerable<int> Get(int count)
        {
            return new int[] { count, count };
        }

        public void Post(int count)
        {
            
        }
    }
}
