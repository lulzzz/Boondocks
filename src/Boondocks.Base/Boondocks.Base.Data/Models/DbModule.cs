using Autofac;
using Boondocks.Base.Data.Core;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Base.Data.Modules
{
    /// <summary>
    /// Plugin module invoked when the application container is bootstrapped.
    /// </summary>
    public class DbModule : PluginModule
    {
        public override void RegisterDefaultComponents(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof (RepositoryContext<>))
                .As(typeof (IRepositoryContext<>))
                .InstancePerLifetimeScope();
        }
    }
}