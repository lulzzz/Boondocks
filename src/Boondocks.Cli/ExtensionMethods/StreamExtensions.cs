namespace Boondocks.Cli.ExtensionMethods
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using Newtonsoft.Json.Linq;

    internal static class StreamExtensions
    {
        public static async Task<BuildResult> ProcessBuildStreamAsync(this Stream responseStream)
        {
            //This response will get built up.
            var result = new BuildResult();

            //Create a list of line handlers
            var handlers = new Func<JObject, BuildResult, bool>[]
            {
                HandleAux,
                HandleStream,
                HandleError
            };

            using (var streamReader = new StreamReader(responseStream))
            {
                var line = await streamReader.ReadLineAsync();

                while (!string.IsNullOrEmpty(line))
                {
                    try
                    {
                        //Parse the line
                        var parsedLine = JObject.Parse(line);

                        //Attempt to handle this line
                        var handler = handlers.FirstOrDefault(h => h(parsedLine, result));

                        //Check to see if it was handled.
                        if (handler == null)
                        {
                            Console.WriteLine(line);

                            //Add this as a raw message
                            result.Messages.Add(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Unable to parse {line}: {ex.Message}");
                    }

                    try
                    {
                        //Read the next line
                        line = await streamReader.ReadLineAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            return result;
        }

        private static bool HandleStream(JObject parsedLine, BuildResult result)
        {
            var streamProperty = parsedLine.Property("stream");

            if (streamProperty != null)
            {
                var formatted = $"{streamProperty.Value}";

                Console.Write(formatted);
                result.Messages.Add(formatted);

                return true;
            }

            return false;
        }

        private static bool HandleAux(JObject parsedLine, BuildResult result)
        {
            var auxProperty = parsedLine.Property("aux");

            if (auxProperty != null)
            {
                var idProperty = (auxProperty.Value as JObject)?.Property("ID");

                if (idProperty != null)
                {
                    var formatted = $"{idProperty.Value}";

                    result.Ids.Add(formatted);

                    Console.WriteLine("==============================================================");
                    Console.WriteLine(formatted);
                    Console.WriteLine("==============================================================");

                    result.Messages.Add(formatted);
                }

                return true;
            }

            return false;
        }

        private static bool HandleError(JObject parsedLine, BuildResult result)
        {
            var property = parsedLine.Property("error");

            if (property != null)
            {
                var formatted = $"{property}";

                Console.Write(formatted);
                result.Errors.Add(formatted);

                return true;
            }

            return false;
        }
    }
}