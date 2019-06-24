namespace Squalr.Engine.Architecture.Assemblers
{
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The Nasm assembler for x86/64.
    /// </summary>
    internal class NasmAssembler : IAssembler
    {
        /// <summary>
        /// Assemble the specified assembly code.
        /// </summary>
        /// <param name="assembly">The assembly code.</param>
        /// <param name="isProcess32Bit">Whether or not the assembly is in the context of a 32 bit program.</param>
        /// <returns>An array of bytes containing the assembly code.</returns>
        public AssemblerResult Assemble(String assembly, Boolean isProcess32Bit)
        {
            // Assemble and return the code
            return this.Assemble(assembly, isProcess32Bit, 0);
        }

        /// <summary>
        /// The path to the nasm binary. This is searched for recursively and cached. This is done since NuGet can move the relative location of the file.
        /// </summary>
        private Lazy<String> nasmPath = new Lazy<String>(() =>
            {
                String currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);

                // When deployed via NuGet, we lose folder structure and must recurse a couple directories higher
                if (directoryInfo.Parent?.Name == "lib")
                {
                    currentDirectory = directoryInfo.Parent?.Parent?.FullName;
                }

                return Directory.EnumerateFiles(currentDirectory, "nasm.exe", SearchOption.AllDirectories).FirstOrDefault();
            },
            LazyThreadSafetyMode.ExecutionAndPublication
        );

        /// <summary>
        /// Assemble the specified assembly code at a base address.
        /// </summary>
        /// <param name="assembly">The assembly code.</param>
        /// <param name="isProcess32Bit">Whether or not the assembly is in the context of a 32 bit program.</param>
        /// <param name="baseAddress">The address where the code is rebased.</param>
        /// <returns>An array of bytes containing the assembly code.</returns>
        public AssemblerResult Assemble(String assembly, Boolean isProcess32Bit, UInt64 baseAddress)
        {
            AssemblerResult result = new AssemblerResult();
            String preamble = "org 0x" + baseAddress.ToString("X") + Environment.NewLine;

            if (isProcess32Bit)
            {
                preamble += "[BITS 32]" + Environment.NewLine;
            }
            else
            {
                preamble += "[BITS 64]" + Environment.NewLine;
            }

            assembly = preamble + assembly;

            try
            {
                String assemblyFilePath = Path.Combine(Path.GetTempPath(), "SqualrAssembly" + Guid.NewGuid() + ".asm");
                String outputFilePath = Path.Combine(Path.GetTempPath(), "SqualrAssembly" + Guid.NewGuid() + ".bin");

                File.WriteAllText(assemblyFilePath, assembly);
                String exePath = this.nasmPath.Value;
                StringBuilder buildOutput = new StringBuilder();
                ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
                startInfo.Arguments = "-f bin -o " + NasmAssembler.Escape(outputFilePath) + " " + NasmAssembler.Escape(assemblyFilePath);
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                Process process = Process.Start(startInfo);
                result.Message = process.StandardOutput.ReadToEnd();
                result.InnerMessage = process.StandardError.ReadToEnd();

                if (result.Message.IsNullOrEmpty() && !result.InnerMessage.IsNullOrEmpty())
                {
                    result.Message = "NASM Compile error";
                }

                process.WaitForExit();

                if (File.Exists(outputFilePath))
                {
                    result.Bytes = File.ReadAllBytes(outputFilePath);
                }
            }
            catch (Exception ex)
            {
                result.Message = "Error compiling with NASM";
                result.InnerMessage = ex.ToString();
            }

            return result;
        }

        private static String Escape(String str)
        {
            return String.Format("\"{0}\"", str);
        }
    }
    //// End class
}
//// End namespace