using Autofac;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Device.Adapters
{
    // Convention based registration.
    public class AdapterModule : PluginModule
    {
        public override void ScanPlugin(TypeRegistration registration)
        {
            registration.PluginTypes.Where(t => t.Name.EndsWith("Adapter", System.StringComparison.Ordinal))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

    }
}