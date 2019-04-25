namespace Squalr.Engine.Memory.Windows.PEB
{
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using static Squalr.Engine.Memory.Windows.Native.Enumerations;
    using static Squalr.Engine.Memory.Windows.Native.NativeMethods;
    using static Squalr.Engine.Memory.Windows.Native.Structures;

    /// <summary>
    /// Class representing the Process Environment Block of a remote process.
    /// </summary>
    internal class ManagedPeb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPeb"/> class.
        /// </summary>
        /// <param name="address">The location of the peb.</param>
        internal ManagedPeb(IntPtr handle)
        {
            this.Handle = handle;

            this.Address = this.FindPebs(this.Handle).First().ToUInt64();
        }

        /// <summary>
        /// Gets or sets a reference to the target process.
        /// </summary>
        public Process TargetProcess { get; set; }

        private IntPtr Handle { get; set; }

        private UInt64 Address { get; set; }

        /// <summary>
        /// Finds the Process Environment Block address of a specified process.
        /// </summary>
        /// <param name="processHandle">A handle of the process.</param>
        /// <returns>A <see cref="IntPtr"/> pointer of the PEB.</returns>
        private IEnumerable<IntPtr> FindPebs(IntPtr processHandle)
        {
            List<IntPtr> pebs = new List<IntPtr>();

            ProcessBasicInformation processBasicInformation = new ProcessBasicInformation();

            Int32 sizeInfoReturned;
            Int32 queryStatus = NtQueryInformationProcess(processHandle, ProcessInformationClass.ProcessBasicInformation, ref processBasicInformation, processBasicInformation.Size, out sizeInfoReturned);
            pebs.Add(processBasicInformation.PebBaseAddress);

            // if (Process.Default.IsSelf64Bit() && Processes.Default.IsOpenedProcess32Bit())
            {
                // When a 32 bit process runs on a 64 bit OS (also known as a WoW64 process), two PEB blocks are loaded.
                // Apparently the only solution is to navigate the TEB to find the PEB. So TODO: Port this code to C#:
                /*
                 #define TEB32OFFSET 0x2000

                void interceptNtDll32(HANDLE hProcess)
                {
                THREAD_BASIC_INFORMATION tbi;
                NTSTATUS ntrv;
                TEB32 teb32;
                void *teb32addr;
                PEB_LDR_DATA32 ldrData;
                PEB32 peb32;
                LIST_ENTRY32 *pMark = NULL;
                LDR_DATA_TABLE_ENTRY32 ldrDataTblEntry;
                size_t bytes_read;
                HANDLE hThread = getThreadHandle(hProcess);

                /* Used to be able to get 32 bit PEB from PEB64 with 0x1000 offset but
                    Windows 8 changed that so we do it indirectly from the TEB 
                if (!hThread)
                    return;

                // Get thread basic information to get 64 bit TEB 
                ntrv = NtQueryInformationThread(hThread, ThreadBasicInformation, &tbi, sizeof(tbi), NULL);
                if (ntrv != 0)
                {
                    goto out;
                }

                // Use magic to find 32 bit TEB
                teb32addr = (char*)tbi.TebBaseAddress + TEB32OFFSET; // Magic...
                ntrv = NtReadVirtualMemory(hProcess, teb32addr, &teb32, sizeof(teb32), NULL);
                if (ntrv != 0 || teb32.NtTib.Self != (DWORD)teb32addr)
                {  // Verify magic...
                    goto out;
                }

                // TEB32 has address for 32 bit PEB.
                ntrv = NtReadVirtualMemory(hProcess, (void*)teb32.ProcessEnvironmentBlock, &peb32, sizeof(peb32), NULL);
                if (ntrv != 0)
                {
                    goto out;
                }
                 */
            }

            return pebs;
        }

        /*
        public Byte InheritedAddressSpace
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<Byte>(this.Address.Add(PebStructure32.InheritedAddressSpace), out success);
            }
        }

        public Boolean BeingDebugged
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<Boolean>(this.Address.Add(PebStructure32.BeingDebugged), out success);
            }
        }

        public IntPtr ProcessHeap
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.ProcessHeap), out success).ToIntPtr();
            }
        }

        public IntPtr ReadOnlySharedMemoryBase
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.ReadOnlySharedMemoryBase), out success).ToIntPtr();
            }
        }

        public IntPtr HeapSegmentReserve
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.HeapSegmentReserve), out success).ToIntPtr();
            }
        }

        public IntPtr HeapSegmentCommit
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.HeapSegmentCommit), out success).ToIntPtr();
            }
        }

        public IntPtr HeapDeCommitTotalFreeThreshold
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.HeapDeCommitTotalFreeThreshold), out success).ToIntPtr();
            }
        }

        public IntPtr HeapDeCommitFreeBlockThreshold
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.HeapDeCommitFreeBlockThreshold), out success).ToIntPtr();
            }
        }

        public IntPtr ProcessHeaps
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.ProcessHeaps), out success).ToIntPtr();
            }
        }

        public IntPtr MinimumStackCommit
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.MinimumStackCommit), out success).ToIntPtr();
            }
        }

        public UInt32 NumberOfHeaps
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<UInt32>(this.Address.Add(PebStructure32.NumberOfHeaps), out success);
            }
        }

        public Int32 MaximumNumberOfHeaps
        {
            get
            {
                Boolean success;
                return MemoryReaderFactory.Windows.Read<Int32>(this.Address.Add(PebStructure32.MaximumNumberOfHeaps), out success);
            }
        }
        */
    }
    //// End class
}
//// End namespace