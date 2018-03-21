using Autofac;
using Boondocks.Auth.App.Modules;
using Boondocks.Auth.Domain.Services;

namespace Boondocks.Auth.Tests.Mocks
{
    /// <summary>
    /// Mock provider module that returns a known provider.
    /// </summary>
    public class MockProviderModule : IAuthProviderModule
    {
        private readonly IAuthProvider _provider;

        public MockProviderModule()
        {

        }

        public MockProviderModule(IAuthProvider provider)
        {
            _provider = provider;
        }

        public IAuthProvider GetServiceAuthProvider(ILifetimeScope currentScope, string serviceName)
        {
            return _provider;
        }
    }
}