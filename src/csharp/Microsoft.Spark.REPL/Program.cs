using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Spark.REPL.Kernel;
using Newtonsoft.Json;

namespace Microsoft.Spark.REPL
{
    internal static class StringExtensions
    {
        internal static string StripMargin(this string s)
        {
            return Regex.Replace(s, @"[ \t]+\|", string.Empty);
        }
    }

    class Program
    {
        private static int s_commandId = 0;

        static void Main()
        {
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "try kernel-server --default-kernel csharp";

            // UseShellExecute defaults to true in .NET Framework,
            // but defaults to false in .NET Core. To support both, set it
            // to false which is required for stream redirection.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            SubmitCode(process.StandardInput, process.StandardOutput, "#r \"nuget: Microsoft.Spark, 0.5.0\"", printValue: false);
            SubmitCode(process.StandardInput, process.StandardOutput, "using Microsoft.Spark.Sql;", printValue: false);
            SubmitCode(process.StandardInput, process.StandardOutput, "using static Microsoft.Spark.Sql.Functions;", printValue: false);
            SubmitCode(process.StandardInput, process.StandardOutput, "var spark = SparkSession.Builder().AppName(\"repl\").GetOrCreate();", printValue: false);

            string middleWare = @"((WorkspaceServer.Kernel.CSharpKernel)Microsoft.DotNet.Interactive.KernelInvocationContext.Current.HandlingKernel)
                | .Pipeline.AddMiddleware(async (command, context, next) =>
                | {
                |   var kernel = context.HandlingKernel as WorkspaceServer.Kernel.CSharpKernel;
                |   var preCellCompilation = kernel.ScriptState.Script.GetCompilation();
                |   var match = System.Text.RegularExpressions.Regex.Match(
                |       preCellCompilation.AssemblyName,
                |       ""^\u211B\\*([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})#([0-9]+-[0-9]+)"");
                |   var path = System.Environment.GetEnvironmentVariable(""HOME"");
                |   var assemblyPath = System.IO.Path.Combine(path, $""{match.Groups[1].Value}-{match.Groups[2].Value}.dll"");
                |   System.IO.Directory.CreateDirectory(path);
                |   if (!System.IO.File.Exists(assemblyPath))
                |   {
                |       Microsoft.CodeAnalysis.FileSystemExtensions.Emit(preCellCompilation, assemblyPath);
                |       SparkSession.GetActiveSession().SparkContext.AddFile(assemblyPath);
                |   }
                |
                |   await next(command, context);
                |});".StripMargin();
            SubmitCode(process.StandardInput, process.StandardOutput, middleWare, printValue: true);

            while (true)
            {
                Console.Write("\ncsharp> ");
                string input = Console.ReadLine();
                if (input == ":quit")
                {
                    var command = new StreamKernelCommand()
                    {
                        Id = s_commandId++,
                        CommandType = "Quit",
                        Command = new Command() { Code = null }
                    };

                    process.StandardInput.WriteLine(command.Serialize());
                    process.StandardInput.Flush();

                    while (process.StandardOutput.ReadLine() != null)
                    {
                    }

                    break;
                }
                else if (string.IsNullOrEmpty(input))
                {

                }
                else
                {
                    SubmitCode(process.StandardInput, process.StandardOutput, input, printValue: true);
                }
            }

            process.Dispose();
        }

        static void SubmitCode(StreamWriter writer, StreamReader reader, string code, bool printValue)
        {
            var command = new StreamKernelCommand()
            {
                Id = s_commandId++,
                CommandType = "SubmitCode",
                Command = new Command() { Code = code }
            };

            writer.WriteLine(command.Serialize());
            writer.Flush();

            string output;
            while ((output = reader.ReadLine()) != null)
            {
                Console.WriteLine($"output: {output}");
                StreamKernelEvent kernelEvent = JsonConvert.DeserializeObject<StreamKernelEvent>(output);

                string value = kernelEvent.GetValue();
                if (printValue && !string.IsNullOrEmpty(value))
                {
                    Console.WriteLine(value);
                }

                if (kernelEvent.IsProcessingComplete())
                {
                    break;
                }
                Console.WriteLine($"About to do a ReadLine");
            }
        }
    }
}
