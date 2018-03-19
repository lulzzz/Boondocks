namespace Boondocks.Agent.RaspberryPi3
{
    using Autofac;
    using Base.Interfaces;

    public class RaspberryPiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RootFileSystemUpdateService>()
                .As<IRootFileSystemUpdateService>()
                .SingleInstance();

            base.Load(builder);
        }
    }
}