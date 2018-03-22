namespace Boondocks.Bootstrap
{
    using Autofac;
    public class BootstrapModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BootstrapHost>();

            base.Load(builder);
        }
    }
}