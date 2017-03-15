using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Service
{
    public class FileService
    {
        private readonly HttpSelfHostServer server;

        public FileService(string port)
        {
            var selfHostConfiguraiton = new HttpSelfHostConfiguration(port);

            selfHostConfiguraiton.Routes.MapHttpRoute(
                name: "DefaultApiRoute",
                routeTemplate: "api/{controller}",
                defaults: null
            );
            //selfHostConfiguraiton.Formatters.Clear();
            //selfHostConfiguraiton.Formatters.Add(new JsonMediaTypeFormatter());
            //selfHostConfiguraiton.Formatters.JsonFormatter.SerializerSettings =
            //new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};

            server = new HttpSelfHostServer(selfHostConfiguraiton);
        }

        public void Start()
        {
            server.OpenAsync();
        }

        public void Stop()
        {
            server.CloseAsync();
            server.Dispose();
        }
    }
}
