namespace Squalr.Engine.Debuggers.Windows.DebugEngine
{
    using Microsoft.Diagnostics.Runtime.Interop;
    using Squalr.Engine.Memory;
    using Squalr.Engine.Processes;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class EventCallBacks : IDebugEventCallbacksWide
    {
        public EventCallBacks()
        {
            this.BaseClient = null;
            this.WriteCallback = null;
            this.ReadCallback = null;
            this.AccessCallback = null;
        }

        public IDebugClient BaseClient { get; set; }

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

        public MemoryAccessCallback WriteCallback { get; set; }

        public MemoryAccessCallback ReadCallback { get; set; }

        public MemoryAccessCallback AccessCallback { get; set; }

        private String[] Registers32 =
        {
            "eax",
            "ebx",
            "ecx",
            "edx",
            "edi",
            "esi",
            "ebp",
            "esp",
            "eip",
        };

        private String[] Registers64 =
        {
            "rax",
            "rbx",
            "rcx",
            "rdx",
            "rdi",
            "rsi",
            "rbp",
            "rsp",
            "rip",
            "r8",
            "r9",
            "r10",
            "r11",
            "r12",
            "r13",
            "r14",
            "r15",
        };

        public Int32 GetInterestMask([Out] out DEBUG_EVENT mask)
        {
            mask = DEBUG_EVENT.BREAKPOINT
                | DEBUG_EVENT.CHANGE_DEBUGGEE_STATE
                | DEBUG_EVENT.CHANGE_ENGINE_STATE
                | DEBUG_EVENT.CHANGE_SYMBOL_STATE
                | DEBUG_EVENT.CREATE_PROCESS
                | DEBUG_EVENT.CREATE_THREAD
                | DEBUG_EVENT.EXCEPTION
                | DEBUG_EVENT.EXIT_PROCESS
                | DEBUG_EVENT.EXIT_THREAD
                | DEBUG_EVENT.LOAD_MODULE
                | DEBUG_EVENT.SESSION_STATUS
                | DEBUG_EVENT.SYSTEM_ERROR
                | DEBUG_EVENT.UNLOAD_MODULE
            ;

            return 0;
        }

        public Int32 Breakpoint([In, MarshalAs(UnmanagedType.Interface)] IDebugBreakpoint2 bp)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Breakpoint Hit");
            this.Control.SetExecutionStatus(DEBUG_STATUS.GO_HANDLED);

            CodeTraceInfo codeTraceInfo = new CodeTraceInfo();

            String[] registers;
            throw new NotImplementedException();
            /*
            Boolean isProcess32Bit = false;// ProcessQuery.Instance.IsProcessWindowed();

            if (isProcess32Bit)
            {
                registers = this.Registers32;
            }
            else
            {
                registers = this.Registers64;
            }

            // Prepare register indicies for DbgEng register value call call
            UInt32[] registerIndicies = new UInt32[registers.Length];

            for (Int32 index = 0; index < registers.Length; index++)
            {
                this.Registers.GetIndexByName(registers[index], out registerIndicies[index]);
            }

            // Get register values
            DEBUG_VALUE[] values = new DEBUG_VALUE[registers.Length];
            this.Registers.GetValues((UInt32)registers.Length, registerIndicies, 0, values);

            // Copy to code trace info
            for (Int32 index = 0; index < registers.Length; index++)
            {
                codeTraceInfo.IntRegisters.Add(registers[index], values[index].I64);
            }

            // Get the current instruction address
            UInt64 address;
            this.Registers.GetInstructionOffset(out address);

            // TEMP: Correct the traced address
            // TODO: Remove this once we figure out how to trigger breakpoint callbacks BEFORE EIP is updated
            address = this.CorrectAddress(address);

            // Disassemble instruction
            Byte[] bytes = null; // MemoryReader.Instance.ReadBytes(address, 15, out _);
            throw new NotImplementedException();
            codeTraceInfo.Instruction = Engine.Architecture.Disassembler.Default.Disassemble(bytes, isProcess32Bit, address).FirstOrDefault();

            // Invoke callbacks
            this.ReadCallback?.Invoke(codeTraceInfo);
            this.WriteCallback?.Invoke(codeTraceInfo);
            this.AccessCallback?.Invoke(codeTraceInfo);

            // Output.Output.Log(Output.LogLevel.Debug, "Breakpoint Hit: " + codeTraceInfo.Address);
            return (Int32)DEBUG_STATUS.BREAK;
            */
        }

        public Int32 Exception([In] ref EXCEPTION_RECORD64 Exception, [In] uint FirstChance)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Exception Hit");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 CreateThread([In] ulong Handle, [In] ulong DataOffset, [In] ulong StartOffset)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Thread Created");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 ExitThread([In] uint ExitCode)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Exit Thread");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 CreateProcess([In] ulong ImageFileHandle, [In] ulong Handle, [In] ulong BaseOffset, [In] uint ModuleSize, [In, MarshalAs(UnmanagedType.LPWStr)] string ModuleName, [In, MarshalAs(UnmanagedType.LPWStr)] string ImageName, [In] uint CheckSum, [In] uint TimeDateStamp, [In] ulong InitialThreadHandle, [In] ulong ThreadDataOffset, [In] ulong StartOffset)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Debugger attached");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 ExitProcess([In] uint ExitCode)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Process exited");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 LoadModule([In] ulong ImageFileHandle, [In] ulong BaseOffset, [In] uint ModuleSize, [In, MarshalAs(UnmanagedType.LPWStr)] string ModuleName, [In, MarshalAs(UnmanagedType.LPWStr)] string ImageName, [In] uint CheckSum, [In] uint TimeDateStamp)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Module loaded");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 UnloadModule([In, MarshalAs(UnmanagedType.LPWStr)] string ImageBaseName, [In] ulong BaseOffset)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Module unloaded");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 SystemError([In] uint Error, [In] uint Level)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "System error");

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 SessionStatus([In] DEBUG_SESSION Status)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Session status: " + Status.ToString());

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 ChangeDebuggeeState([In] DEBUG_CDS Flags, [In] ulong Argument)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Change debuggee State: " + Flags.ToString());

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 ChangeEngineState([In] DEBUG_CES Flags, [In] ulong Argument)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Change engine State: " + Flags.ToString());

            return (Int32)DEBUG_STATUS.BREAK;
        }

        public Int32 ChangeSymbolState([In] DEBUG_CSS Flags, [In] ulong Argument)
        {
            // Output.Output.Log(Output.LogLevel.Debug, "Change symbol State: " + Flags.ToString());

            return (Int32)DEBUG_STATUS.BREAK;
        }

        private UInt64 CorrectAddress(UInt64 address)
        {
            const UInt64 MaxInstructionSize = 15;

            UInt32 disassemblySize = 0;
            UInt64 endAddress = 0;
            UInt64 effectiveAddress = address - MaxInstructionSize - 1;

            do
            {
                effectiveAddress++;

                StringBuilder buffer = new StringBuilder(256);
                this.Control.Disassemble(effectiveAddress, DEBUG_DISASM.EFFECTIVE_ADDRESS, buffer, 256, out disassemblySize, out endAddress);

            } while (endAddress < address);


            return effectiveAddress;
        }
    }
    //// End class
}
//// End namespace
