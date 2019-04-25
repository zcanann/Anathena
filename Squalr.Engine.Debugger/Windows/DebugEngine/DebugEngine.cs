namespace Squalr.Engine.Debuggers.Windows.DebugEngine
{
    using Microsoft.Diagnostics.Runtime.Interop;
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DebugEngine : IDebugger
    {
        private CancellationTokenSource readCancellationToken;

        private CancellationTokenSource writeCancellationToken;

        private CancellationTokenSource accessCancellationToken;

        public DebugEngine(Process targetProcess)
        {
            this.TargetProcess = targetProcess;
            this.DebugRequestCallback = null;
            this.EventCallBacks = new EventCallBacks();
            this.OutputCallBacks = new OutputCallBacks();
            this.Scheduler = new ConcurrentExclusiveSchedulerPair();
            this.InterruptScheduler = new ConcurrentExclusiveSchedulerPair();
            this.BreakPoints = new ConcurrentDictionary<CancellationTokenSource, IDebugBreakpoint2>();
            this.readCancellationToken = null;
            this.writeCancellationToken = null;
            this.accessCancellationToken = null;
        }

        /// <summary>
        /// Gets or sets the debug request callback. This callback will be called before the debugger is attached,
        /// and will only be attached if the result of the callback is true.
        /// </summary>
        public DebugRequestCallback DebugRequestCallback { get; set; }

        public Boolean IsAttached { get; set; }

        private IDebugClient BaseClient { get; set; }

        private IDebugClient6 Client
        {
            get
            {
                return this.BaseClient as IDebugClient6;
            }
        }

        private IDebugControl6 Control
        {
            get
            {
                return this.BaseClient as IDebugControl6;
            }
        }

        public IDebugRegisters2 Registers
        {
            get
            {
                return this.BaseClient as IDebugRegisters2;
            }
        }

        public IDebugAdvanced3 Advanced
        {
            get
            {
                return this.BaseClient as IDebugAdvanced3;
            }
        }

        private Process TargetProcess { get; set; }

        private EventCallBacks EventCallBacks { get; set; }

        private OutputCallBacks OutputCallBacks { get; set; }

        private ConcurrentExclusiveSchedulerPair Scheduler { get; set; }

        private ConcurrentExclusiveSchedulerPair InterruptScheduler { get; set; }

        private Boolean Interrupt { get; set; }

        private ConcurrentDictionary<CancellationTokenSource, IDebugBreakpoint2> BreakPoints { get; set; }

        public CancellationTokenSource FindWhatReads(UInt64 address, BreakpointSize size, MemoryAccessCallback callback)
        {
            this.Attach();

            this.readCancellationToken?.Cancel();
            this.EventCallBacks.ReadCallback = callback;
            IDebugBreakpoint2 breakpoint = this.SetHardwareBreakpoint(address, DEBUG_BREAKPOINT_ACCESS_TYPE.READ, size.ToUInt32());

            if (breakpoint == null)
            {
                return null;
            }

            this.readCancellationToken = this.CreateNewCancellationToken(this.OnAccessTraceCancel);
            this.BreakPoints.TryAdd(this.readCancellationToken, breakpoint);

            return this.readCancellationToken;
        }

        public CancellationTokenSource FindWhatWrites(UInt64 address, BreakpointSize size, MemoryAccessCallback callback)
        {
            this.Attach();

            this.writeCancellationToken?.Cancel();
            this.EventCallBacks.WriteCallback = callback;
            IDebugBreakpoint2 breakpoint = this.SetHardwareBreakpoint(address, DEBUG_BREAKPOINT_ACCESS_TYPE.WRITE, size.ToUInt32());

            if (breakpoint == null)
            {
                return null;
            }

            this.writeCancellationToken = this.CreateNewCancellationToken(this.OnAccessTraceCancel);
            this.BreakPoints.TryAdd(this.writeCancellationToken, breakpoint);

            return this.writeCancellationToken;
        }

        public CancellationTokenSource FindWhatAccesses(UInt64 address, BreakpointSize size, MemoryAccessCallback callback)
        {
            this.Attach();

            this.accessCancellationToken?.Cancel();
            this.EventCallBacks.AccessCallback = callback;
            IDebugBreakpoint2 breakpoint = this.SetHardwareBreakpoint(address, DEBUG_BREAKPOINT_ACCESS_TYPE.READ | DEBUG_BREAKPOINT_ACCESS_TYPE.WRITE, size.ToUInt32());

            if (breakpoint == null)
            {
                return null;
            }

            this.accessCancellationToken = this.CreateNewCancellationToken(this.OnAccessTraceCancel);
            this.BreakPoints.TryAdd(this.accessCancellationToken, breakpoint);

            return this.accessCancellationToken;
        }

        public void Attach()
        {
            // Exit if already attached, or debug request fails
            if (this.IsAttached || (this.DebugRequestCallback != null && this.DebugRequestCallback()))
            {
                return;
            }

            // Perform the attach
            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.BaseClient = DebugEngine.CreateIDebugClient();
                    this.Client.AddProcessOptions(DEBUG_PROCESS.DETACH_ON_EXIT);

                    this.EventCallBacks.BaseClient = this.BaseClient;

                    this.Client.SetOutputCallbacksWide(this.OutputCallBacks);
                    this.Client.SetEventCallbacksWide(this.EventCallBacks);

                    this.Client.AttachProcess(0, unchecked((UInt32)this.TargetProcess.Id), DEBUG_ATTACH.DEFAULT);
                    this.Control.WaitForEvent(DEBUG_WAIT.DEFAULT, 0);

                    List<DEBUG_EXCEPTION_FILTER_PARAMETERS> exceptionFilters = new List<DEBUG_EXCEPTION_FILTER_PARAMETERS>();

                    foreach (EXCEPTION exception in Enum.GetValues(typeof(EXCEPTION)))
                    {
                        /*
                        exceptionFilters.Add(new DEBUG_EXCEPTION_FILTER_PARAMETERS()
                        {
                            ExceptionCode = (UInt32)exception,
                            ExecutionOption = DEBUG_FILTER_EXEC_OPTION.BREAK,
                            ContinueOption = DEBUG_FILTER_CONTINUE_OPTION.GO_NOT_HANDLED,
                        });
                        */

                        // this.Control.ExecuteWide(DEBUG_OUTCTL.THIS_CLIENT, "sxe " + ((UInt32)exception).ToString("X"), DEBUG_EXECUTE.ECHO);
                        // this.Control.ExecuteWide(DEBUG_OUTCTL.THIS_CLIENT, "sxe -h " + ((UInt32)exception).ToString("X"), DEBUG_EXECUTE.ECHO);
                    }

                    // this.Control.SetExceptionFilterParameters((UInt32)exceptionFilters.Count, exceptionFilters.ToArray());
                    // this.Control.ExecuteWide(DEBUG_OUTCTL.THIS_CLIENT, "sx", DEBUG_EXECUTE.ECHO);

                    this.IsAttached = true;
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, "Error attaching debugger", ex);
                }
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.Scheduler.ExclusiveScheduler).Wait();

            // Listen for events such as breakpoint hits
            this.ListenForEvents();
        }

        private void ListenForEvents()
        {
            Task.Run(() =>
            {
                while (this.IsAttached)
                {
                    // Do not listen for events when interrupted
                    while (this.Interrupt)
                    {
                    }

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            DEBUG_STATUS status;

                            this.CheckHandleResult(this.Control.GetExecutionStatus(out status));

                            if (status == DEBUG_STATUS.NO_DEBUGGEE)
                            {
                                return;
                            }

                            this.Control.WaitForEvent(DEBUG_WAIT.DEFAULT, UInt32.MaxValue);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogLevel.Error, "Error listening for debugger events", ex);
                        }
                    }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.Scheduler.ExclusiveScheduler).Wait();
                }
            });
        }

        private IDebugBreakpoint2 SetSoftwareBreakpoint(UInt64 address, DEBUG_BREAKPOINT_ACCESS_TYPE access, UInt32 size)
        {
            return this.SetBreakpoint(address, DEBUG_BREAKPOINT_TYPE.CODE, access, size);
        }

        private IDebugBreakpoint2 SetHardwareBreakpoint(UInt64 address, DEBUG_BREAKPOINT_ACCESS_TYPE access, UInt32 size)
        {
            return this.SetBreakpoint(address, DEBUG_BREAKPOINT_TYPE.DATA, access, size);
        }

        private IDebugBreakpoint2 SetBreakpoint(UInt64 address, DEBUG_BREAKPOINT_TYPE breakpointType, DEBUG_BREAKPOINT_ACCESS_TYPE access, UInt32 size)
        {
            const UInt32 AnyId = UInt32.MaxValue;

            IDebugBreakpoint2 breakpoint = null;

            this.BeginInterrupt();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.CheckHandleResult(this.Control.AddBreakpoint2(breakpointType, AnyId, out breakpoint));

                    breakpoint.SetOffset(address);
                    breakpoint.SetFlags(DEBUG_BREAKPOINT_FLAG.ENABLED);
                    breakpoint.SetDataParameters(size, access);

                    this.Control.SetExecutionStatus(DEBUG_STATUS.GO);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, "Error setting breakpoint", ex);
                }
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.Scheduler.ExclusiveScheduler).Wait();

            this.EndInterrupt();

            return breakpoint;
        }

        private void RemoveBreakpoint(CancellationTokenSource source)
        {
            this.BeginInterrupt();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    IDebugBreakpoint2 breakpoint;

                    if (this.BreakPoints.TryRemove(source, out breakpoint))
                    {
                        breakpoint.SetFlags(DEBUG_BREAKPOINT_FLAG.NONE);

                        Logger.Log(LogLevel.Debug, "Breakpoint removed");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, "Error removing breakpoint", ex);
                }
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.Scheduler.ExclusiveScheduler).Wait();

            this.EndInterrupt();
        }

        private void BeginInterrupt()
        {
            this.Interrupt = true;

            try
            {
                Task.Factory.StartNew(() =>
                {
                    this.CheckHandleResult(this.Control.SetInterrupt(DEBUG_INTERRUPT.ACTIVE));
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.InterruptScheduler.ExclusiveScheduler).Wait();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Debug, "Error interrupting events", ex);
            }
        }

        private void EndInterrupt()
        {
            this.Interrupt = false;
        }

        private CancellationTokenSource CreateNewCancellationToken(Action cancelCallback)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(this.OnWriteTraceCancel);

            return cancellationTokenSource;
        }

        private void OnWriteTraceCancel()
        {
            this.EventCallBacks.WriteCallback = null;
            this.RemoveBreakpoint(this.writeCancellationToken);
            this.writeCancellationToken = null;
        }

        private void OnReadTraceCancel()
        {
            this.EventCallBacks.ReadCallback = null;
            this.RemoveBreakpoint(this.readCancellationToken);
            this.readCancellationToken = null;
        }

        private void OnAccessTraceCancel()
        {
            this.EventCallBacks.AccessCallback = null;
            this.RemoveBreakpoint(this.accessCancellationToken);
            this.accessCancellationToken = null;
        }

        private void CheckHandleResult(Int32 hresult)
        {
            this.CheckHandleResult((ERROR_CODE)hresult);
        }

        private void CheckHandleResult(ERROR_CODE hresult)
        {
            if (hresult < 0)
            {
                throw new Exception("Invalid HRESULT: " + hresult.ToString());
            }
        }

        private static IDebugClient CreateIDebugClient()
        {
            Guid guid = typeof(IDebugClient).GUID;
            DebugEngine.DebugCreate(ref guid, out Object obj);
            IDebugClient client = (IDebugClient)obj;

            return client;
        }

        /// <summary>
        /// The DebugCreate function creates a new client object and returns an interface pointer to it.
        /// </summary>
        /// <param name="InterfaceId">Specifies the interface identifier (IID) of the desired debugger engine client interface. This is the type of the interface that will be returned in Interface.</param>
        /// <param name="Interface">Receives an interface pointer for the new client. The type of this interface is specified by InterfaceId.</param>
        [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
        [DllImport("dbgeng.dll")]
        internal static extern UInt32 DebugCreate(ref Guid InterfaceId, [MarshalAs(UnmanagedType.IUnknown)] out Object Interface);
    }
    //// End class
}
//// End namespace
