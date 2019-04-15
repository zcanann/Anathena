namespace Squalr.Cli
{
    using Squalr.Engine.Common;
    using System;

    public interface ICommandHandler
    {
        void TryHandle(ref Session session, Command command);
    }
    //// End class
}
//// End namespace
