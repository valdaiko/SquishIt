﻿using NUnit.Framework;

namespace SquishIt.Nancy.Tests
{
    [TestFixture]
    public class HoganTests : SquishIt.Tests.HoganTests
    {
        public HoganTests() : base(
            TestConfiguration.Default.DefaultOutputBaseHref(),
            TestConfiguration.Default.DefaultPathTranslator(),
            TestConfiguration.Default.FileSystemResolver,
            TestConfiguration.Default.HttpResolver,
            TestConfiguration.Default.RootEmbeddedResourceResolver,
            TestConfiguration.Default.StandardEmbeddedResourceResolver,
            TestConfiguration.Default.VirtualPathRoot)
        {
        }
    }
}
