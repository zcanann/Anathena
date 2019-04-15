namespace Squalr.Cli
{
    using System;

    class Program
    {
        static void Main(String[] args)
        {
            while (true)
            {
                String input = Console.ReadLine();

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("close", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                CommandDispatcher.Dispatch(input);
            }
        }
    }
    //// End class
}
//// End namespace
