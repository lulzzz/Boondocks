using System;

namespace Boondocks.Cli.ExtensionMethods
{
    public static class ArrayExtensions
    {
        public static void DisplayEntities<TEntity>(this TEntity[] entities, Func<TEntity, string> formatFunc)
        {
            if (entities.Length == 0)
            {
                Console.WriteLine(" None found.");
            }
            else
            {
                foreach (var entity in entities)
                {
                    Console.WriteLine($"  {formatFunc(entity)}");
                }

                Console.WriteLine($"{entities.Length} found.");
            }
        }
    }
}