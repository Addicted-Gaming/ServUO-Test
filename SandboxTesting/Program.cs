using System;

namespace SandboxTesting
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var buildMode = "Debug";
            var headless = false;
            var cleanup = true;

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--mode":
                    case "-m":
                        if (i + 1 < args.Length)
                        {
                            buildMode = args[++i];
                        }
                        break;
                    case "--headless":
                    case "-h":
                        headless = true;
                        break;
                    case "--cleanup":
                        cleanup = true;
                        break;
                    case "--no-cleanup":
                        cleanup = false;
                        break;
                }
            }

            Console.WriteLine($"Build mode: {buildMode}");
            Console.WriteLine($"Headless: {headless}");
            Console.WriteLine($"Cleanup: {cleanup}");
        }
    }
}
