namespace Squalr.Cli.CommandHandlers
{
    using System;
    using System.Linq;

    public class Command
    {
        public Command(String command)
        {
            String[] args = command.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            this.Name = (args?.Length ?? 0) <= 0 ? String.Empty : args[0];
            this.Args = (args?.Length ?? 0) <= 0 ? new String[] { } : args.Skip(1).ToArray();
            this.Handled = false;
        }

        public String Name { get; private set; }

        public String[] Args { get; private set; }

        public Boolean Handled { get; set; }
    }
    //// End class
}
//// End namespace
