using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Swagger.Net;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Sample.Mvc4WebApi.App_Start.SwaggerNet), "PreStart")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Sample.Mvc4WebApi.App_Start.SwaggerNet), "PostStart")]
namespace Sample.Mvc4WebApi.App_Start
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
                var baseType = HttpContext.Current.ApplicationInstance.GetType().BaseType;
                if (baseType != null)
                {
                    var assemblyname = Assembly.GetAssembly(baseType).GetName().Name;
                    var path = HttpContext.Current.Server.MapPath("~/bin/" + assemblyname + ".xml");
                    var docProvider = new XmlCommentDocumentationProvider(path);
                    config.Services.Replace(typeof(IDocumentationProvider), docProvider);
                }

                config.Filters.Add(new SwaggerActionFilterAttribute());
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Please enable \"XML documentation file\" in project properties with default (bin\\Sample.Mvc4WebApi.XML) value or edit value in App_Start\\SwaggerNet.cs");
            }
        }
    }
}