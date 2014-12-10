using System.Net.Http;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public interface ITestWebSite
    {
        string BaseUrl { get; }

        string Host { get; }

        HttpClient CreateClient();
    }
}