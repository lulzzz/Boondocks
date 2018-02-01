namespace Boondocks.Agent
{
    using BizArk.ConsoleApp;

    internal class Program
    {
        private static void Main(string[] args)
        {
            BaCon.Start<AgentApp>();
        }
    }
}