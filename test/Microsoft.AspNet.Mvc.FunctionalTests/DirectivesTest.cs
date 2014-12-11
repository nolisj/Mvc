// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class DirectivesTest
    {
        [Fact]
        public async Task ViewsInheritsUsingsAndInjectDirectivesFromViewStarts()
        {
            var expected = @"Hello Person1";
            var site = TestWebSite.Create(nameof(RazorWebSite));
            var client = site.CreateClient();

            // Act
            var body = await client.GetStringAsync("http://localhost/Directives/ViewInheritsInjectAndUsingsFromViewStarts");

            // Assert
            Assert.Equal(expected, body.Trim());
        }

        [Fact]
        public async Task ViewInheritsBasePageFromViewStarts()
        {
            var expected = @"WriteLiteral says:layout:Write says:Write says:Hello Person2";
            var site = TestWebSite.Create(nameof(RazorWebSite));
            var client = site.CreateClient();

            // Act
            var body = await client.GetStringAsync("http://localhost/Directives/ViewInheritsBasePageFromViewStarts");

            // Assert
            Assert.Equal(expected, body.Trim());
        }
    }
}