using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using filth.filters;

namespace filth
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Services.RemoveAll(
            typeof(System.Web.Http.Validation.ModelValidatorProvider),
            v => v is System.Web.Http.Validation.Providers.InvalidModelValidatorProvider);

            config.Filters.Add(new ModelValidationFilter());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional }
            );
        }
    }
}
