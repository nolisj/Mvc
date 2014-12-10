// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InMemoryFactAttribute : FactAttribute
    {
        public InMemoryFactAttribute(string reason)
        {
            if (!TestWebSite.IsInMemoryServer)
            {
                Skip = reason;
            }
        }
    }
}