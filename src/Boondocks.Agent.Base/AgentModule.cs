namespace Boondocks.Agent.Base
{
    using System;
    using Autofac;
    using Docker.DotNet;
    using Interfaces;
    using Logs;
    using Model;
    using Serilog;
    using Update;

    public static partial class ContainerFactory
    {
        public class AgentModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                //Update services
                builder.RegisterType<ApplicationUpdateService>().SingleInstance();
                builder.RegisterType<AgentUpdateService>().SingleInstance();

                //IDockerClient
                builder.Register(context =>
                {
                    IPathFactory pathFactory = context.Resolve<IPathFactory>();

                    string endpoint = pathFactory.DockerEndpoint;

                    var dockerClientConfiguration = new DockerClientConfiguration(new Uri(endpoint));

                    return dockerClientConfiguration.CreateClient();

                }).As<IDockerClient>().SingleInstance();

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .CreateLogger();

                builder.RegisterInstance(Log.Logger);

                //Types
                builder.RegisterType<DeviceStateProvider>().SingleInstance();
                builder.RegisterType<UptimeProvider>().As<IUptimeProvider>().SingleInstance();
                builder.RegisterType<AgentHost>().As<IAgentHost>().SingleInstance();
                builder.RegisterType<ApplicationDockerContainerFactory>().SingleInstance();
                builder.RegisterType<AgentDockerContainerFactory>().SingleInstance();
                builder.RegisterType<PlatformDetector>().As<IPlatformDetector>().SingleInstance();
                builder.RegisterType<ApplicationLogSucker>().SingleInstance();
                builder.RegisterType<LogBatchCollector>().SingleInstance();

                base.Load(builder);
            }
        }
    }
}