using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using System.Xml.XPath;
using Swagger.Net;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Swagger.Net.WebApi.App_Start.SwaggerNet), "PreStart")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Swagger.Net.WebApi.App_Start.SwaggerNet), "PostStart")]
namespace Swagger.Net.WebApi.App_Start
{
    public static class SwaggerNet
    {
        public static void PreStart()
        {
            RouteTable.Routes.MapHttpRoute(
                name: "SwaggerApi",
                routeTemplate: "api/docs/{controller}",
                defaults: new { swagger = true }
            );
        }

        public static void PostStart()
        {
            var config = GlobalConfiguration.Configuration;
            
            try
            {
                var assemblyname = Assembly.GetAssembly(HttpContext.Current.ApplicationInstance.GetType().BaseType).GetName().Name;
                var path = HttpContext.Current.Server.MapPath("~/bin/" + assemblyname + ".xml");
                config.Services.Replace(typeof(IDocumentationProvider), new XmlCommentDocumentationProvider(path));
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Please enable \"XML documentation file\" in project properties with default (bin\\Swagger.Net.WebApi.XML) value or edit value in App_Start\\SwaggerNet.cs");
            }

            config.Filters.Add(new SwaggerActionFilter());

        }
    }
}