// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class CompositeViewEngineTests
    {
        [Fact]
        public async Task CompositeViewEngine_FindsPartialViewsAcrossAllEngines()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(CompositeViewEngineWebSite));
            var client = site.CreateClient();

            // Act
            var body = await client.GetStringAsync("http://localhost/");

            // Assert
            Assert.Equal("Hello world", body.Trim());
        }

        [Fact]
        public async Task CompositeViewEngine_FindsViewsAcrossAllEngines()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(CompositeViewEngineWebSite));
            var client = site.CreateClient();

            // Act
            var body = await client.GetStringAsync("http://localhost/Home/TestView");

            // Assert
            Assert.Equal("Content from test view", body.Trim());
        }
    }
}