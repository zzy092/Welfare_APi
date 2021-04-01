using System.Web.Http;
using WebActivatorEx;
using Welfare;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Welfare
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            var xmlFile = string.Format("{0}/bin/WelfareApi.XML", System.AppDomain.CurrentDomain.BaseDirectory);
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "WelfareApi");
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        if (System.IO.File.Exists(xmlFile)) { c.IncludeXmlComments(xmlFile); }
                        //c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("My Swagger UI");
                        c.InjectJavaScript(thisAssembly, "WelfareApi.Scripts.swagger.js");
                    });
        }
    }
    
}
