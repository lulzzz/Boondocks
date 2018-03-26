using Autofac;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Auth.Modules
{
    // Convention based registration.
    public class RepositoryModule : PluginModule
    {
        public override void ScanPlugin(TypeRegistration registration)
        {
            registration.PluginTypes.Where(t => t.Name.EndsWith("Repository", System.StringComparison.Ordinal))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}