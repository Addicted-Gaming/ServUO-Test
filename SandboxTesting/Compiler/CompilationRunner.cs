using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SandboxTesting.Compiler
{
    public static class CompilationRunner
    {
        public static CompilationResult Run(bool debug, bool cache)
        {
            var logBuilder = new StringBuilder();
            var writer = new StringWriter(logBuilder);

            var originalOut = Console.Out;
            var originalErr = Console.Error;

            Console.SetOut(writer);
            Console.SetError(writer);

            var stopwatch = Stopwatch.StartNew();
            bool success;

            try
            {
                success = Server.ScriptCompiler.Compile(debug, cache);
            }
            catch (Exception e)
            {
                writer.WriteLine(e);
                success = false;
            }
            finally
            {
                stopwatch.Stop();
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
                writer.Dispose();
            }

            var log = logBuilder.ToString();
            var errors = success
                ? Array.Empty<string>()
                : log.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return new CompilationResult
            {
                Success = success,
                Errors = errors,
                Duration = stopwatch.Elapsed,
                Log = log
            };
        }
    }

    public class CompilationResult
    {
        public bool Success { get; set; }

        public string[] Errors { get; set; }

        public TimeSpan Duration { get; set; }

        public string Log { get; set; }
    }
}
