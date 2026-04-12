using System.Diagnostics;
using System.Text;

namespace Swallow.MSBuild.Node.Test.Framework;

public sealed class Dotnet
{
    public enum Configuration { Debug, Release }

    [Flags]
    public enum BuildFlags
    {
        None          = 0b0000_0000,
        NoRestore     = 0b0000_0010,
        NoBuild       = 0b0000_0100,
        NoIncremental = 0b0000_1000,
    }

    private static readonly string executablePath = FindDotnetExecutable();
    private string? workingDirectory = null;

    public void MoveTo(string directory)
    {
        workingDirectory = directory;
    }

    public void AddPackage(string packageName, string? project = null)
    {
        Execute($"add {project} package {packageName}", workingDirectory);
    }

    public void Restore(string? project = null)
    {
        Execute($"restore {project}", workingDirectory);
    }

    public void Build(string? project = null, BuildFlags flags = BuildFlags.None, Configuration configuration = Configuration.Debug)
    {
        Execute($"build {RenderFlags(flags)} --configuration {configuration} {project}", workingDirectory);
    }

    public void Publish(string outputDirectory, string? project = null, BuildFlags flags = BuildFlags.None, Configuration configuration = Configuration.Debug)
    {
        Execute($"publish {RenderFlags(flags)} --configuration {configuration} --output {outputDirectory} {project}", workingDirectory);
    }

    public void Pack(string outputDirectory, string? project = null, BuildFlags flags = BuildFlags.None, Configuration configuration = Configuration.Debug)
    {
        Execute($"pack {RenderFlags(flags)} --configuration {configuration} --output {outputDirectory} {project}", workingDirectory);
    }

    private static string RenderFlags(BuildFlags flags)
    {
        var builder = new StringBuilder();
        if (flags.HasFlag(BuildFlags.NoRestore))
        {
            builder.Append("--no-restore ");
        }

        if (flags.HasFlag(BuildFlags.NoBuild))
        {
            builder.Append("--no-build ");
        }

        if (flags.HasFlag(BuildFlags.NoIncremental))
        {
            builder.Append("--no-incremental ");
        }

        return builder.ToString();
    }

    private static void Execute(string commandline, string? workingDirectory)
    {
        var startInfo = new ProcessStartInfo(executablePath, commandline)
        {
            RedirectStandardOutput = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };

        var process = Process.Start(startInfo);
        if (process is null)
        {
            throw new InvalidOperationException($"'{executablePath} {commandline}' failed to start.");
        }

        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        if (process.ExitCode is not 0)
        {
            throw new InvalidOperationException($"'{executablePath} {commandline}' failed with exit code {process.ExitCode}.\n\n{output}");
        }
    }

    private static string FindDotnetExecutable()
    {
        if (Environment.GetEnvironmentVariable("DOTNET_ROOT") is { } dotnetRoot)
        {
            return Path.Combine(dotnetRoot, "dotnet");
        }

        if (Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") is { } dotnetHostPath)
        {
            return dotnetHostPath;
        }

        throw new InvalidOperationException("Could not find dotnet executable; ensure that either DOTNET_ROOT or DOTNET_HOST_PATH is set.");
    }
}
