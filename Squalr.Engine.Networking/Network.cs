namespace Squalr.Engine.Networks
{
    using Squalr.Engine.Engine.Hook;
    using System.Diagnostics;

    public class Network : INetwork
    {
        public Network()
        {
        }

        private HookClient HookClient { get; set; }

        private Process TargetProcess { get; set; }

        public void InstallHook()
        {
            if (this.TargetProcess == null)
            {
                return;
            }

            this.HookClient = new HookClient();
            this.HookClient?.Inject(this.TargetProcess.Id);
        }

        public void UninstallHook()
        {
            this.HookClient?.Uninject();
            this.HookClient = null;
        }
    }
    //// End class
}
//// End namespace