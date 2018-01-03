﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace Boondocks.Supervisor
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create the container
            using (var container = ApplicationContainerFactory.Create())
            {
                //Get the supervisor host
                var host = container.Resolve<SupervisorHost>();

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                //We shall cancel on the keypress
                Console.CancelKeyPress += (sender, eventArgs) => cancellationTokenSource.Cancel();

                try
                {
                    Console.WriteLine("Starting...");
                    host.RunAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
                }
                catch (TaskCanceledException)
                {
                }
            }   
        }
    }
}