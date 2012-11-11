using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Swagger.Net;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Sample.WebApi.SwaggerNet), "PostStart")]
namespace Sample.WebApi
{
    public static class SwaggerNet
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DocsApi",
                routeTemplate: "api/docs/{id}",
                defaults: new { controller = "Docs", id = RouteParameter.Optional }
            );
        }

        public static void PostStart()
        {
            var config = GlobalConfiguration.Configuration;

            var baseType = HttpContext.Current.ApplicationInstance.GetType().BaseType;
            if (baseType != null)
            {
                var assemblyname = Assembly.GetAssembly(baseType).GetName().Name;
                var path = HttpContext.Current.Server.MapPath("~/bin/" + assemblyname + ".xml");
                ConfigureDocumentationProvider(path, config.Services);
            }
            else
            {
                throw new ApplicationException("ApplicationInstance.GetType().BaseType not found.");
            }

        }

        public static void ConfigureDocumentationProvider(string absoluteDocPath, ServicesContainer services)
        {

            try
            {
                var docProvider = new XmlCommentDocumentationProvider(absoluteDocPath);
                services.Replace(typeof(IDocumentationProvider), docProvider);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Please enable \"XML documentation file\" in project properties with default (bin\\Sample.WebApi.XML) value or edit value in App_Start\\SwaggerNet.cs");
            }


        }
    }
}