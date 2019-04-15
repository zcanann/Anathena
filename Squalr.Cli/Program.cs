namespace Squalr.Cli
{
    using System;

    class Program
    {
        static void Main(String[] args)
        {
            Console.SetOut(new PrefixedWriter());
            CommandDispatcher commandDispatcher = new CommandDispatcher();

            while (true)
            {
                Console.Write("");
                String input = Console.ReadLine();

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("close", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                commandDispatcher.Dispatch(input);
            }
        }
    }
    //// End class
}
//// End namespace
