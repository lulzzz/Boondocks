﻿using System;
using System.Linq;
using CommandLine;

namespace BoondocksCli
{
    class Program
    {
        static int Main(string[] args)
        {
            //All commands are based off of this type.
            Type baseType = typeof(CommandBase);

            //Get the command types via reflection.
            var commandTypes = baseType.Assembly.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

            //Create the execution context.
            var executionContext = new ExecutionContext();

            //Do it now
            return Parser.Default.ParseArguments(args, commandTypes)
                .MapResult(
                    (CommandBase opts) => opts.ExecuteAsync(executionContext).GetAwaiter().GetResult(),
                    errs => 1);
        }
    }
}
