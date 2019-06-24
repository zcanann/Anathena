namespace Squalr.Engine.Scripting.Memory
{
    using Squalr.Engine.Architecture;
    using Squalr.Engine.Architecture.Assemblers;
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Memory;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Provides access to memory manipulations in an external process for scripts.
    /// </summary>
    internal class MemoryCore : IMemoryCore
    {
        /// <summary>
        /// The size of a jump instruction. TODO: this may not always be the case, and sure as hell isn't true for all architectures.
        /// </summary>
        private const Int32 JumpSize = 5;

        /// <summary>
        /// The largest possible instruction size. TODO: Abstract this out to the architecture factory. This can vary by architecture.
        /// </summary>
        private const Int32 Largestx86InstructionSize = 15;

        /// <summary>
        /// Gets or sets the keywords associated with all running scripts.
        /// </summary>
        private static Lazy<ConcurrentDictionary<String, Object>> globalKeywords = new Lazy<ConcurrentDictionary<String, Object>>(
            () => { return new ConcurrentDictionary<String, Object>(); },
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCore" /> class.
        /// </summary>
        public MemoryCore()
        {
            this.RemoteAllocations = new List<UInt64>();
            this.Keywords = new ConcurrentDictionary<String, Object>();
            this.CodeCaves = new List<CodeCave>();
        }

        /// <summary>
        /// Gets or sets the keywords associated with the calling script.
        /// </summary>
        private ConcurrentDictionary<String, Object> Keywords { get; set; }

        /// <summary>
        /// Gets or sets the collection of allocations created in the external process;
        /// </summary>
        private List<UInt64> RemoteAllocations { get; set; }

        /// <summary>
        /// Gets or sets the collection of code caves active in the external process.
        /// </summary>
        private List<CodeCave> CodeCaves { get; set; }

        /// <summary>
        /// Returns the address of the specified module name. Returns 0 on failure.
        /// </summary>
        /// <param name="moduleName">The name of the module to calculate the address of.</param>
        /// <returns>The read address.</returns>
        public UInt64 GetModuleAddress(String moduleName)
        {
            this.PrintDebugTag();

            moduleName = moduleName?.RemoveSuffixes(true, ".exe", ".dll");

            UInt64 address = 0;
            foreach (NormalizedModule module in MemoryQueryer.Instance.GetModules())
            {
                String targetModuleName = module?.Name?.RemoveSuffixes(true, ".exe", ".dll");
                if (targetModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    address = module.BaseAddress;
                    break;
                }
            }

            return address;
        }

        /// <summary>
        /// Determines the size of the first instruction at the given address.
        /// </summary>
        /// <param name="assembly">The assembly code to measure.</param>
        /// <param name="address">The base address of the assembly code.</param>
        /// <returns>The size, in number of bytes, of the assembly code.</returns>
        public Int32 GetAssemblySize(String assembly, UInt64 address)
        {
            this.PrintDebugTag();

            assembly = this.ResolveKeywords(assembly);

            Byte[] bytes = this.GetAssemblyBytes(assembly, address);

            return bytes == null ? 0 : bytes.Length;
        }

        /// <summary>
        /// Converts the instruction with a frame of reference at a specific address to raw bytes.
        /// </summary>
        /// <param name="assembly">The assembly code to disassemble.</param>
        /// <param name="address">The base address of the assembly code.</param>
        /// <returns>The disassembled bytes of the assembly code.</returns>
        public Byte[] GetAssemblyBytes(String assembly, UInt64 address)
        {
            this.PrintDebugTag();

            assembly = this.ResolveKeywords(assembly);
            AssemblerResult result = Assembler.Instance.Assemble(assembly, Processes.Instance.IsOpenedProcess32Bit(), address);

            Logger.Log(LogLevel.Info, result.Message, result.InnerMessage);

            return result.Bytes;
        }

        /// <summary>
        /// Returns the bytes of multiple instructions, as long as they are greater than the specified minimum.
        /// ie with a minimum of 16 bytes, if we get three 5 byte instructions, we will need to read the entire
        /// next instruction.
        /// </summary>
        /// <param name="address">The base address to begin reading instructions.</param>
        /// <param name="injectedCodeSize">The size of the code being injected.</param>
        /// <returns>The bytes read from memory</returns>
        public Byte[] CollectOriginalBytes(UInt64 address, Int32 injectedCodeSize)
        {
            this.PrintDebugTag();

            // Read original bytes at code cave jump
            Boolean readSuccess;

            Byte[] originalBytes = MemoryReader.Instance.ReadBytes(address, injectedCodeSize + MemoryCore.Largestx86InstructionSize, out readSuccess);

            if (!readSuccess || originalBytes == null || originalBytes.Length <= 0)
            {
                return null;
            }

            // Grab instructions at code entry point
            IEnumerable<Instruction> instructions = Disassembler.Instance.Disassemble(originalBytes, Processes.Instance.IsOpenedProcess32Bit(), address);

            // Determine size of instructions we need to overwrite
            Int32 replacedInstructionSize = 0;
            foreach (Instruction instruction in instructions)
            {
                replacedInstructionSize += instruction.Size;
                if (replacedInstructionSize >= injectedCodeSize)
                {
                    break;
                }
            }

            if (replacedInstructionSize < injectedCodeSize)
            {
                return null;
            }

            // Truncate to only the bytes we will need to save
            originalBytes = originalBytes.LargestSubArray(0, replacedInstructionSize);

            return originalBytes;
        }

        /// <summary>
        /// Allocates memory in the target process, and returns the address of the new memory.
        /// </summary>
        /// <param name="size">The size of the allocation.</param>
        /// <returns>The address of the allocated memory.</returns>
        public UInt64 Allocate(Int32 size)
        {
            this.PrintDebugTag();

            UInt64 address = MemoryAllocator.Instance.AllocateMemory(size);
            this.RemoteAllocations.Add(address);

            return address;
        }

        /// <summary>
        /// Allocates memory in the target process, and returns the address of the new memory.
        /// </summary>
        /// <param name="size">The size of the allocation.</param>
        /// <param name="allocAddress">The rough address of where the allocation should take place.</param>
        /// <returns>The address of the allocated memory.</returns>
        public UInt64 Allocate(Int32 size, UInt64 allocAddress)
        {
            this.PrintDebugTag();

            UInt64 address = MemoryAllocator.Instance.AllocateMemory(size, allocAddress);
            this.RemoteAllocations.Add(address);

            return address;
        }

        /// <summary>
        /// Deallocates memory previously allocated at a specified address.
        /// </summary>
        /// <param name="address">The address to perform the deallocation.</param>
        public void Deallocate(UInt64 address)
        {
            this.PrintDebugTag();

            foreach (UInt64 allocationAddress in this.RemoteAllocations)
            {
                if (allocationAddress == address)
                {
                    MemoryAllocator.Instance.DeallocateMemory(allocationAddress);
                    this.RemoteAllocations.Remove(allocationAddress);
                    break;
                }
            }

            return;
        }

        /// <summary>
        /// Deallocates all allocated memory for the parent script.
        /// </summary>
        public void DeallocateAll()
        {
            this.PrintDebugTag();

            foreach (UInt64 address in this.RemoteAllocations)
            {
                MemoryAllocator.Instance.DeallocateMemory(address);
            }

            this.RemoteAllocations.Clear();
        }

        /// <summary>
        /// Creates a code cave that jumps from a given entry address and executes the given assembly. This will nop-fill.
        /// If the injected assmebly code fits in one instruction, no cave will be created.
        /// </summary>
        /// <param name="address">The injection address.</param>
        /// <param name="assembly">The assembly code to disassemble and inject into the code cave.</param>
        /// <returns>The address of the code cave. Returns zero if no allocation was necessary.</returns>
        public UInt64 CreateCodeCave(UInt64 address, String assembly)
        {
            this.PrintDebugTag();

            assembly = this.ResolveKeywords(assembly);

            // Determine size of our injected code
            Int32 assemblySize = this.GetAssemblySize(assembly, address);

            // Determine the minimum number of bytes that need to be replaced
            Int32 minimumReplacementSize = Math.Min(assemblySize, MemoryCore.JumpSize);

            // Gather the original bytes
            Byte[] originalBytes = this.CollectOriginalBytes(address, minimumReplacementSize);

            // Handle case where allocation is not needed
            if (assemblySize <= originalBytes.Length)
            {
                // Determine number of no-ops to fill dangling bytes
                String noOps = assemblySize - originalBytes.Length > 0 ? "db " + String.Join(" ", Enumerable.Repeat("0x90,", assemblySize - originalBytes.Length)).TrimEnd(',') : String.Empty;

                Byte[] injectionBytes = this.GetAssemblyBytes(assembly + Environment.NewLine + noOps, address);
                this.Write(address, injectionBytes);

                CodeCave codeCave = new CodeCave(address, 0, originalBytes);
                this.CodeCaves.Add(codeCave);

                return address;
            }
            else
            {
                // Determine number of no-ops to fill dangling bytes
                String noOps = originalBytes.Length - minimumReplacementSize > 0 ? "db " + String.Join(" ", Enumerable.Repeat("0x90,", originalBytes.Length - minimumReplacementSize)).TrimEnd(',') : String.Empty;

                // Add code cave jump return automatically
                UInt64 returnAddress = this.GetCaveExitAddress(address);

                // Place jump to return address
                assembly = assembly.Trim()
                    + Environment.NewLine
                    + "jmp " + Conversions.ToHex(returnAddress, formatAsAddress: false, includePrefix: true);
                assemblySize = this.GetAssemblySize(assembly, address);

                // Allocate memory
                UInt64 remoteAllocation;

                if (Processes.Instance.IsOpenedProcess32Bit())
                {
                    remoteAllocation = this.Allocate(assemblySize);
                }
                else
                {
                    remoteAllocation = this.Allocate(assemblySize, address);
                }

                // Write injected code to new page
                Byte[] injectionBytes = this.GetAssemblyBytes(assembly, remoteAllocation);
                this.Write(remoteAllocation, injectionBytes);

                // Write in the jump to the code cave
                String codeCaveJump = ("jmp " + Conversions.ToHex(remoteAllocation, formatAsAddress: false, includePrefix: true) + Environment.NewLine + noOps).Trim();
                Byte[] jumpBytes = this.GetAssemblyBytes(codeCaveJump, address);
                this.Write(address, jumpBytes);

                // Save this code cave for later deallocation
                CodeCave codeCave = new CodeCave(address, remoteAllocation, originalBytes);
                this.CodeCaves.Add(codeCave);

                return remoteAllocation;
            }
        }

        /// <summary>
        /// Injects instructions at the specified location, overwriting following instructions. This will nop-fill.
        /// </summary>
        /// <param name="address">The injection address.</param>
        /// <param name="assembly">The assembly code to disassemble and inject into the code cave.</param>
        /// <returns>The address of the code cave.</returns>
        public UInt64 InjectCode(UInt64 address, String assembly)
        {
            this.PrintDebugTag();

            assembly = this.ResolveKeywords(assembly);

            Int32 assemblySize = this.GetAssemblySize(assembly, address);

            Byte[] originalBytes = this.CollectOriginalBytes(address, assemblySize);

            if (originalBytes == null)
            {
                throw new Exception("Could not gather original bytes");
            }

            // Determine number of no-ops to fill dangling bytes
            String noOps = (originalBytes.Length - assemblySize > 0 ? "db " : String.Empty) + String.Join(" ", Enumerable.Repeat("0x90,", originalBytes.Length - assemblySize)).TrimEnd(',');

            Byte[] injectionBytes = this.GetAssemblyBytes(assembly + "\n" + noOps, address);
            MemoryWriter.Instance.WriteBytes(address, injectionBytes);

            CodeCave codeCave = new CodeCave(address, 0, originalBytes);
            this.CodeCaves.Add(codeCave);

            return address;
        }

        /// <summary>
        /// Determines the address that a code cave would need to return to, if one were to be created at the specified address.
        /// </summary>
        /// <param name="address">The address of the code cave.</param>
        /// <returns>The address to which the code cave will return upon completion.</returns>
        public UInt64 GetCaveExitAddress(UInt64 address)
        {
            this.PrintDebugTag();

            Byte[] originalBytes = this.CollectOriginalBytes(address, MemoryCore.JumpSize);
            Int32 originalByteSize;

            if (originalBytes != null && originalBytes.Length < MemoryCore.JumpSize)
            {
                // Determine the size of the minimum number of instructions we will be overwriting
                originalByteSize = originalBytes.Length;
            }
            else
            {
                // Fall back if something goes wrong
                originalByteSize = MemoryCore.JumpSize;
            }

            address = address.Add(originalByteSize);

            return address;
        }

        /// <summary>
        /// Removes a created code cave at the specified address.
        /// </summary>
        /// <param name="codeCaveAddress">The address of the code cave.</param>
        public void RemoveCodeCave(UInt64 codeCaveAddress)
        {
            this.PrintDebugTag();

            foreach (CodeCave codeCave in this.CodeCaves)
            {
                // If remote allocation address is unset, then it was not allocated. Also, if the addresses do not match, ignore it.
                if (codeCave.RemoteAllocationAddress == 0 || codeCave.Address != codeCaveAddress)
                {
                    continue;
                }

                MemoryWriter.Instance.WriteBytes(codeCave.Address, codeCave.OriginalBytes);

                MemoryAllocator.Instance.DeallocateMemory(codeCave.RemoteAllocationAddress);
            }
        }

        /// <summary>
        /// Removes all created code caves by the parent script.
        /// </summary>
        public void RemoveAllCodeCaves()
        {
            this.PrintDebugTag();

            foreach (CodeCave codeCave in this.CodeCaves)
            {
                MemoryWriter.Instance.WriteBytes(codeCave.Address, codeCave.OriginalBytes);

                // If remote allocation address is unset, then it was not allocated.
                if (codeCave.RemoteAllocationAddress == 0)
                {
                    continue;
                }

                MemoryAllocator.Instance.DeallocateMemory(codeCave.RemoteAllocationAddress);
            }

            this.CodeCaves.Clear();
        }

        /// <summary>
        /// Binds a keyword to a given value for use in the script.
        /// </summary>
        /// <param name="keyword">The local keyword to bind.</param>
        /// <param name="value">The value to which the keyword is bound.</param>
        public void SetKeyword(String keyword, Object value)
        {
            this.PrintDebugTag(keyword?.ToLower(), value?.ToString() as String);

            this.Keywords[keyword?.ToLower()] = value;
        }

        /// <summary>
        /// Binds a keyword to a given value for use in all scripts.
        /// </summary>
        /// <param name="globalKeyword">The global keyword to bind.</param>
        /// <param name="value">The address to which the keyword is bound.</param>
        public void SetGlobalKeyword(String globalKeyword, Object value)
        {
            this.PrintDebugTag(globalKeyword?.ToLower(), value.ToString() as String);

            MemoryCore.globalKeywords.Value[globalKeyword?.ToLower()] = value;
        }

        /// <summary>
        /// Gets the value of a keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>The value of the keyword. If not found, returns 0.</returns>
        public Object GetKeyword(String keyword)
        {
            Object result;
            this.Keywords.TryGetValue(keyword?.ToLower(), out result);

            return result;
        }

        /// <summary>
        /// Gets the value of a global keyword.
        /// </summary>
        /// <param name="globalKeyword">The global keyword.</param>
        /// <returns>The value of the global keyword. If not found, returns 0.</returns>
        public Object GetGlobalKeyword(String globalKeyword)
        {
            Object result;
            MemoryCore.globalKeywords.Value.TryGetValue(globalKeyword?.ToLower(), out result);

            return result;
        }

        /// <summary>
        /// Clears the specified keyword created by the parent script.
        /// </summary>
        /// <param name="keyword">The local keyword to clear.</param>
        public void ClearKeyword(String keyword)
        {
            this.PrintDebugTag(keyword);

            Object result;
            if (this.Keywords.ContainsKey(keyword))
            {
                this.Keywords.TryRemove(keyword, out result);
            }
        }

        /// <summary>
        /// Clears the specified global keyword created by any script.
        /// </summary>
        /// <param name="globalKeyword">The global keyword to clear.</param>
        public void ClearGlobalKeyword(String globalKeyword)
        {
            this.PrintDebugTag(globalKeyword);

            if (MemoryCore.globalKeywords.Value.ContainsKey(globalKeyword))
            {
                Object valueRemoved;
                MemoryCore.globalKeywords.Value.TryRemove(globalKeyword, out valueRemoved);
            }
        }

        /// <summary>
        /// Clears all keywords created by the parent script.
        /// </summary>
        public void ClearAllKeywords()
        {
            this.PrintDebugTag();

            this.Keywords.Clear();
        }

        /// <summary>
        /// Clears all global keywords created by any script.
        /// </summary>
        public void ClearAllGlobalKeywords()
        {
            this.PrintDebugTag();

            MemoryCore.globalKeywords.Value.Clear();
        }

        /// <summary>
        /// Searches for the first address that matches the array of bytes.
        /// </summary>
        /// <param name="bytes">The array of bytes to search for.</param>
        /// <returns>The address of the first first array of byte match.</returns>
        public UInt64 SearchAob(Byte[] bytes)
        {
            this.PrintDebugTag();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for the first address that matches the given array of byte pattern.
        /// </summary>
        /// <param name="pattern">The pattern string for which to search.</param>
        /// <returns>The address of the first first pattern match.</returns>
        public UInt64 SearchAob(String pattern)
        {
            this.PrintDebugTag(pattern);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for all addresses that match the given array of byte pattern.
        /// </summary>
        /// <param name="pattern">The array of bytes to search for.</param>
        /// <returns>The addresses of all matches.</returns>
        public UInt64[] SearchAllAob(String pattern)
        {
            this.PrintDebugTag(pattern);

            throw new NotImplementedException();
        }

        public UInt64 EvaluatePointer(UInt64 address, IEnumerable<Int32> offsets)
        {
            this.PrintDebugTag();

            UInt64 finalAddress = MemoryReader.Instance.EvaluatePointer(address, offsets);
            return finalAddress;
        }

        /// <summary>
        /// Reads the value at the given address.
        /// </summary>
        /// <typeparam name="T">The data type to read.</typeparam>
        /// <param name="address">The address of the read.</param>
        /// <returns>The value read from memory.</returns>
        public T Read<T>(UInt64 address)
        {
            this.PrintDebugTag(address.ToString("x"));

            Boolean readSuccess;
            return MemoryReader.Instance.Read<T>(address, out readSuccess);
        }

        /// <summary>
        /// Reads the array of bytes of the specified count at the given address.
        /// </summary>
        /// <param name="address">The address of the read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The bytes read at the address.</returns>
        public Byte[] Read(UInt64 address, Int32 count)
        {
            this.PrintDebugTag(address.ToString("x"), count.ToString());

            Boolean readSuccess;
            return MemoryReader.Instance.ReadBytes(address, count, out readSuccess);
        }

        /// <summary>
        /// Writes the value at the specified address.
        /// </summary>
        /// <typeparam name="T">The data type to write.</typeparam>
        /// <param name="address">The address of the write.</param>
        /// <param name="value">The value of the write.</param>
        public void Write<T>(UInt64 address, T value)
        {
            this.PrintDebugTag(address.ToString("x"), value.ToString());

            MemoryWriter.Instance.Write<T>(address, value);
        }

        /// <summary>
        /// Writes the Byte array to the specified address
        /// </summary>
        /// <param name="address">The address of the write.</param>
        /// <param name="values">The values of the write.</param>
        public void Write(UInt64 address, Byte[] values)
        {
            this.PrintDebugTag(address.ToString("x"));

            MemoryWriter.Instance.WriteBytes(address, values);
        }

        /// <summary>
        /// Replaces user provided keywords with their associated value.
        /// </summary>
        /// <param name="assembly">The assembly script.</param>
        /// <returns>The assembly script with all keywords replaced with their values.</returns>
        private String ResolveKeywords(String assembly)
        {
            if (assembly == null)
            {
                return String.Empty;
            }

            // Clear out any whitespace that may cause issues in the assembly script
            assembly = assembly.Replace("\t", String.Empty);

            // Resolve keywords
            foreach (KeyValuePair<String, Object> keyword in this.Keywords)
            {
                assembly = assembly.Replace("<" + keyword.Key + ">", Conversions.ToHex(keyword.Value, formatAsAddress: false, includePrefix: true) as String, StringComparison.OrdinalIgnoreCase);
            }

            foreach (KeyValuePair<String, Object> globalKeyword in MemoryCore.globalKeywords.Value.ToArray())
            {
                assembly = assembly.Replace("<" + globalKeyword.Key + ">", Conversions.ToHex(globalKeyword.Value, formatAsAddress: false, includePrefix: true) as String, StringComparison.OrdinalIgnoreCase);
            }

            return assembly;
        }

        /// <summary>
        /// Defines instructions replaced by a jump to a newly allocated region of memory, which will execute and return control.
        /// </summary>
        private struct CodeCave
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CodeCave" /> struct.
            /// </summary>
            /// <param name="address">The entry address of the code cave.</param>
            /// <param name="codeCaveAddress">The address of the code cave allocation.</param>
            /// <param name="originalBytes">The original bytes being overwritten.</param>
            public CodeCave(UInt64 address, UInt64 codeCaveAddress, Byte[] originalBytes)
            {
                this.RemoteAllocationAddress = codeCaveAddress;
                this.OriginalBytes = originalBytes;
                this.Address = address;
            }

            /// <summary>
            /// Gets or sets the original instruction bytes at the cave entry.
            /// </summary>
            public Byte[] OriginalBytes { get; set; }

            /// <summary>
            /// Gets or sets the address of the allocated code cave.
            /// </summary>
            public UInt64 RemoteAllocationAddress { get; set; }

            /// <summary>
            /// Gets or sets the entry address of the code cave.
            /// </summary>
            public UInt64 Address { get; set; }
        }
    }
    //// End interface
}
//// End namespace