﻿using MakersPortal.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MakersPortal.Tests.Integration.Services
{
    public class KeyManagerIntegrationTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _testsFixture;

        public KeyManagerIntegrationTests(TestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact]
        public void EnsureKeyManagerIsRegistered_Success()
        {
            var keyManager = _testsFixture.Server.Services.GetService<IKeyManager>();
            Assert.NotNull(keyManager);
        }
    }
}