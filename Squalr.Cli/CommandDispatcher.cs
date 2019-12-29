namespace Squalr.Cli
{
    using Squalr.Cli.CommandHandlers;
    using Squalr.Cli.CommandHandlers.Process;
    using Squalr.Cli.CommandHandlers.Project;
    using Squalr.Cli.CommandHandlers.Results;
    using Squalr.Cli.CommandHandlers.Scan;
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommandDispatcher
    {
        private Session session;

        public CommandDispatcher()
        {
            this.CommandHandlers = new List<ICommandHandler>()
            {
                new ProcessCommandHandler(),
                new ProjectCommandHandler(),
                new ScanCommandHandler(),
                new ResultsCommandHandler(),
            };
        }

        private Session Session
        {
            get
            {
                return this.session;
            }

            set
            {
                this.session = value;
            }
        }

        List<ICommandHandler> CommandHandlers { get; set; }

        public void Dispatch(String command)
        {
            Command constructedCommand = new Command(command);

            this.TryHandleHelp(constructedCommand);

            foreach (ICommandHandler handler in this.CommandHandlers)
            {
                if (constructedCommand.Handled)
                {
                    break;
                }

                if (handler.GetCommandAndAliases().Any(alias => constructedCommand.Name.Equals(alias, StringComparison.OrdinalIgnoreCase)))
                {
                    handler.TryHandle(ref this.session, constructedCommand);
                }
            }

            if (!constructedCommand.Handled)
            {
                if (String.IsNullOrWhiteSpace(constructedCommand.Name))
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Unrecognized command '" + constructedCommand.Name + "'");
                }
            }
        }

        private void TryHandleHelp(Command command)
        {
            if (!command.Name.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            foreach (ICommandHandler handler in this.CommandHandlers)
            {
                Console.WriteLine("Commands for TODO:");

                foreach (String alias in handler.GetCommandAndAliases())
                {
                    Console.Write(alias + " | ");
                }

                Console.WriteLine();
            }

            command.Handled = true;
        }
    }
    //// End class
}
//// End namespace
