namespace Squalr.Cli
{
    using Squalr.Engine;
    using System;
    using System.Collections.Generic;

    public class CommandDispatcher
    {
        private Session session;

        public CommandDispatcher()
        {
            this.CommandHandlers = new List<ICommandHandler>()
            {
                new ProcessCommandHandler(),
                new ScanCommandHandler(),
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

            foreach (ICommandHandler handler in this.CommandHandlers)
            {
                if (constructedCommand.Handled)
                {
                    break;
                }

                handler.TryHandle(ref this.session, constructedCommand);
            }

            if (!constructedCommand.Handled && !String.IsNullOrWhiteSpace(constructedCommand.Name))
            {
                Console.WriteLine("Unrecognized command '" + constructedCommand.Name + "'");
            }
        }
    }
    //// End class
}
//// End namespace
