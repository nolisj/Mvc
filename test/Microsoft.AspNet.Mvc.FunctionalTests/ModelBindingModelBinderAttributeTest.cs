// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using ModelBindingWebSite;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class ModelBindingModelBinderAttributeTest
    {
        [Fact]
        public async Task ModelBinderAttribute_CustomModelPrefix()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ModelBindingWebSite));
            var client = site.CreateClient();

            // [ModelBinder(Name = "customPrefix")] is used to apply a prefix
            var url = 
                "http://localhost/ModelBinderAttribute_Company/GetCompany?customPrefix.Employees[0].Name=somename";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(body);

            var employee = Assert.Single(company.Employees);
            Assert.Equal("somename", employee.Name);
        }

        [Theory]
        [InlineData("GetBinderType_UseModelBinderOnType")]
        [InlineData("GetBinderType_UseModelBinderProviderOnType")]
        public async Task ModelBinderAttribute_WithPrefixOnParameter(string action)
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ModelBindingWebSite));
            var client = site.CreateClient();

            // [ModelBinder(Name = "customPrefix")] is used to apply a prefix
            var url =
                "http://localhost/ModelBinderAttribute_Product/" +
                action +
                "?customPrefix.ProductId=5";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(
                "ModelBindingWebSite.Controllers.ModelBinderAttribute_ProductController+ProductModelBinder",
                body);
        }

        [Theory]
        [InlineData("GetBinderType_UseModelBinder")]
        [InlineData("GetBinderType_UseModelBinderProvider")]
        public async Task ModelBinderAttribute_WithBinderOnParameter(string action)
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ModelBindingWebSite));
            var client = site.CreateClient();

            var url =
                "http://localhost/ModelBinderAttribute_Product/" +
                action +
                "?model.productId=5";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(
                "ModelBindingWebSite.Controllers.ModelBinderAttribute_ProductController+ProductModelBinder", 
                body);
        }

        [Fact]
        public async Task ModelBinderAttribute_WithBinderOnEnum()
        {
            // Arrange
            var site = TestWebSite.Create(nameof(ModelBindingWebSite));
            var client = site.CreateClient();

            var url =
                "http://localhost/ModelBinderAttribute_Product/" +
                "ModelBinderAttribute_UseModelBinderOnEnum" +
                "?status=Shipped";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("StatusShipped", body);
        }

        private class Product
        {
            public int ProductId { get; set; }

            public string BinderType { get; set; }
        }
    }
}