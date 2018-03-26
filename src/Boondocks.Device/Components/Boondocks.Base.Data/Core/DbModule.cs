using Autofac;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Base.Data.Core
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