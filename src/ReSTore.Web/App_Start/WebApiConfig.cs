using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace ReSTore.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

/*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
*/

            config.Routes.MapHttpRoute(
                name: "CategoriesApi",
                routeTemplate: "api/areas/{areaId}/categories/{id}",
                defaults: new {controller = "categories", id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute(
                name: "ProductsApi",
                routeTemplate: "api/areas/{areaId}/categories/{categoryId}/products/{id}",
                defaults: new { controller = "products", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "OrderCommandsApi",
                routeTemplate: "api/commands/order/{controller}/{commandId}",
                defaults: new { commandId = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "RootApi",
                routeTemplate: "api",
                defaults: new { controller = "root" }
            );

        }
    }
}
