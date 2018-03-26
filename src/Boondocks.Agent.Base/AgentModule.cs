namespace Boondocks.Agent.Base
{
    using System;
    using AgentLogging;
    using ApplicationLogging;
    using Autofac;
    using Docker.DotNet;
    using Interfaces;
    using Model;
    using Serilog;
    using Update;

    public static partial class ContainerFactory
    {
        internal class AgentModule : Module
        {
            /// <summary>
            /// The maximum amount of time that an agent log event will be held before being transmitted to the server.
            /// </summary>
            private const double BatchPeriodSeconds = 30;

            /// <summary>
            /// The maximum number of agent log events to send in a single batch.
            /// </summary>
            private const int BatchSizeLimit = 100;

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

                var sink = new AgentLogSink(BatchSizeLimit, TimeSpan.FromSeconds(BatchPeriodSeconds));

                builder.RegisterInstance(sink);

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.Sink(sink)
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