using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public static class TestWebSite
    {
        public static ITestWebSite Create(string webSiteName)
        {
            if (IsInMemoryServer)
            {
                var services = TestHelper.CreateServices(webSiteName);

                var assembly = Assembly.Load(new AssemblyName(webSiteName));

                var startupType = assembly.GetExportedTypes().Where(t => string.Equals("StartUp", t.Name, StringComparison.OrdinalIgnoreCase)).Single();
                var method = startupType.GetRuntimeMethod("Configure", new Type[] { typeof(IApplicationBuilder), });

                var startup = Activator.CreateInstance(startupType);
                var configure = (Action<IApplicationBuilder>)method.CreateDelegate(typeof(Action<IApplicationBuilder>), startup);

                var server = TestHost.TestServer.Create(services, configure);

                return new InMemoryWebSite()
                {
                    BaseUrl = "http://localhost",
                    Host = "localhost",
                    Inner = server,
                };
            }
            else
            {
                var text = File.ReadAllText(Environment.GetEnvironmentVariable("MVC_TEST_SERVER"));
                var sites = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

                var url = sites[webSiteName];

                var parsed = new Uri(url, UriKind.Absolute);

                return new HttpWebSite()
                {
                    BaseUrl = url,
                    Host = parsed.GetComponents(UriComponents.HostAndPort, UriFormat.Unescaped),
                };
            }
        }

        public static bool IsInMemoryServer
        {
            get
            {
                return Environment.GetEnvironmentVariable("MVC_TEST_SERVER") == null;
            }
        }

        private class InMemoryWebSite : ITestWebSite
        {
            public string BaseUrl { get; set; }

            public string Host { get; set; }

            public TestHost.TestServer Inner { get; set; }

            public HttpClient CreateClient()
            {
                return Inner.CreateClient();
            }
        }

        private class HttpWebSite : ITestWebSite
        {
            public string BaseUrl { get; set; }

            public string Host { get; set; }

            public HttpClient CreateClient()
            {
                var handler = new BaseUrlHandler(BaseUrl);

                // Don't follow redirects, we want to examine the HTTP output.
                // This is consistent with how the in-memory client behaves.
                handler.AllowAutoRedirect = false;
                return new HttpClient(handler);
            }
        }

        private class BaseUrlHandler : HttpClientHandler
        {
            public BaseUrlHandler(string baseUrl)
            {
                BaseUrl = baseUrl;
            }

            public string BaseUrl { get; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri.AbsoluteUri.StartsWith("http://localhost"))
                {
                    request.RequestUri = new Uri(BaseUrl + request.RequestUri.AbsoluteUri.Substring("http://localhost".Length));
                }

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}