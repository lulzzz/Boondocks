using BizArk.ConsoleApp;

namespace Boondocks.Agent
{
    class Program
    {
        private static void Main(string[] args)
        {
            BaCon.Start<AgentApp>();
        }
    }
}
