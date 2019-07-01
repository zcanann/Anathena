namespace Squalr.Cli
{
    using Squalr.Engine.Common.Logging;
    using System;

    public class Program
    {
        static void Main(String[] args)
        {
            Console.SetOut(new PrefixedWriter());
            CommandDispatcher commandDispatcher = new CommandDispatcher();
            LogListener logListener = new LogListener();

            Logger.Subscribe(logListener);

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
