using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ReSTore.Web.Controllers;
using ReSTore.Web.Controllers.OrderControllers;

namespace ReSTore.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
                name: "CategoriesApi",
                routeTemplate: "api/areas/{areaId}/categories/{id}",
                defaults: new { controller = "categories", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ProductsApi",
                routeTemplate: "api/areas/{areaId}/categories/{categoryId}/products/{id}",
                defaults: new { controller = "products", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "OrderCommandsApi",
                routeTemplate: "api/order/{controller}/{commandId}",
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
                defaults: new { controller="root" }
            );
        }
    }
}
