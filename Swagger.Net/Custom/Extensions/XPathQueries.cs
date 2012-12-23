using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swagger.Net.Custom.Extensions
{
    public static class XPathQueries
    {
        public const string METHOD = "/doc/members/member[@name='M:{0}']";
        public const string TYPE = "/doc/members/member[@name='T:{0}']";
        public const string ENUM = "/doc/members/member[@name='F:{0}']";
        public const string MEMBER = "param[@name='{0}']";
        public const string PROPERTY = "/doc/members/member[contains(@name,'P:{0}')]";
        public const string NO_DOCS_FOUND = "No docs found.";  //"No documentation found."
    }
}
