using System;
using System.IO;
using SandboxTesting.Compiler;
using Xunit;

namespace Server.Tests;

public class CompilationRunnerTests
{
    [Fact]
    public void Compilation_succeeds()
    {
        var previousDir = Directory.GetCurrentDirectory();
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

        try
        {
            Directory.SetCurrentDirectory(repoRoot);
            var result = CompilationRunner.Run(debug: false, cache: false);

            Assert.True(result.Success, string.Join(Environment.NewLine, result.Errors));
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDir);
        }
    }
}
