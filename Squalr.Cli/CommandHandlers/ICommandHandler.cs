namespace Squalr.Cli
{
    using System;

    interface ICommandHandler
    {
        void TryHandleCommand(String command);
    }
    //// End interface
}
//// End namespace
