// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using BasicWebSite;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class TagHelpersTests
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices("TagHelpersWebSite");
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;

        // Some tests require comparing the actual response body against an expected response baseline
        // so they require a reference to the assembly on which the resources are located, in order to
        // make the tests less verbose, we get a reference to the assembly with the resources and we
        // use it on all the rest of the tests.
        private readonly Assembly _resourcesAssembly = typeof(TagHelpersTests).GetTypeInfo().Assembly;

        [Theory]
        [InlineData("Index")]
        [InlineData("About")]
        [InlineData("Help")]
        public async Task CanRenderViewsWithTagHelpers(string action)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");

            // The K runtime compiles every file under compiler/resources as a resource at runtime with the same name
            // as the file name, in order to update a baseline you just need to change the file in that folder.
            var expectedContent = await _resourcesAssembly.ReadResourceAsStringAsync(
                "compiler/resources/TagHelpersWebSite.Home." + action + ".html");

            // Act

            // The host is not important as everything runs in memory and tests are isolated from each other.
            var response = await client.GetAsync("http://localhost/Home/" + action);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(expectedContent, responseContent);
        }

        [Theory]
        [InlineData("NestedViewStartTagHelper")]
        [InlineData("ViewWithLayoutAndNestedTagHelper")]
        public async Task TagHelpersAreInheritedFromViewStartPages(string action)
        {
            // Arrange
            var expected = string.Join(Environment.NewLine,
                                       "<root>root-content</root>",
                                       "",
                                       "",
                                       "<nested>nested-content</nested>");
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            // Act
            var result = await client.GetStringAsync("http://localhost/Home/" + action);

            // Assert
            Assert.Equal(expected, result.Trim());
        }

        [Theory]
        [InlineData("Create")]
        public async Task ViewsWithModelMetadataAttributes_CanRender(string action)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");
            var expectedContent = await _resourcesAssembly.ReadResourceAsStringAsync(
                "compiler/resources/TagHelpersWebSite.Employee." + action + ".html");

            // Act
            var response = await client.GetAsync("http://localhost/Employee/" + action);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(expectedContent, responseContent);
        }

        [Theory]
        [InlineData("Create", "Index")]
        public async Task ViewsWithModelMetadataAttributes_CanHandleValidPost(string postAction, string redirectAction)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");
            var expectedContent = await _resourcesAssembly.ReadResourceAsStringAsync(
                "compiler/resources/TagHelpersWebSite.Employee." + redirectAction + ".AfterCreate.html");
            var validPostValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("FullName", "Boo"),
                new KeyValuePair<string,string>("Gender", "M"),
                new KeyValuePair<string,string>("Age", "22"),
                new KeyValuePair<string,string>("EmployeeId", "0"),
                new KeyValuePair<string,string>("JoinDate", "2014-12-01"),
                new KeyValuePair<string,string>("Email", "a@b.com"),
            };
            var postContent = new FormUrlEncodedContent(validPostValues);

            // Act
            var postResponse = await client.PostAsync("http://localhost/Employee/" + postAction, postContent);
            var response = await client.GetAsync("http://localhost/Employee/" + redirectAction);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, postResponse.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(expectedContent, responseContent);
        }

        [Theory]
        [InlineData("Create")]
        public async Task ViewsWithModelMetadataAttributes_CanHandleInvalidPost(string action)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");
            var expectedContent = await _resourcesAssembly.ReadResourceAsStringAsync(
                "compiler/resources/TagHelpersWebSite.Employee." + action + ".Invalid.html");
            var validPostValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("FullName", "Boo"),
                new KeyValuePair<string,string>("Gender", "M"),
                new KeyValuePair<string,string>("Age", "1000"),
                new KeyValuePair<string,string>("EmployeeId", "0"),
                new KeyValuePair<string,string>("Email", "a@b.com"),
                new KeyValuePair<string,string>("Salary", "z"),
            };
            var postContent = new FormUrlEncodedContent(validPostValues);

            // Act
            var response = await client.PostAsync("http://localhost/Employee/" + action, postContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(expectedContent, responseContent);
        }
    }
}