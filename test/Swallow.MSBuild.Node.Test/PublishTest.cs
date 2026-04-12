using Swallow.MSBuild.Node.Test.Framework;

namespace Swallow.MSBuild.Node.Test;

public sealed class PublishTest() : MSBuildTestBase("publish")
{
    protected override string GetSourceProjectPath(string solutionRoot) => Path.Combine(solutionRoot, "test", "TestHost");
    protected override IEnumerable<string> GetTargetFileNames() => ["TestHost.csproj", "Client/index.js", "build.js", "package.json", "package-lock.json", "Program.cs", "NuGet.config"];

    [Test]
    public async Task DirectlyPublishing_HasAssetInOutput()
    {
        var dotnet = new Dotnet();

        var outputDirectory = Path.Combine(CurrentDirectory, "publish");
        dotnet.Publish(outputDirectory, ProjectPath);

        var expectedFile = Path.Combine(outputDirectory, "wwwroot", "index.min.js");
        await Assert.That(File.Exists(expectedFile)).IsTrue();
    }

    [Test]
    public async Task ExplicitBuildBeforePack_HasAssetInOutput()
    {
        var dotnet = new Dotnet();

        dotnet.Restore(ProjectPath);
        dotnet.Build(ProjectPath, Dotnet.BuildFlags.NoRestore);

        var outputDirectory = Path.Combine(CurrentDirectory, "publish");
        dotnet.Publish(outputDirectory, ProjectPath, Dotnet.BuildFlags.NoRestore | Dotnet.BuildFlags.NoBuild);

        var expectedFile = Path.Combine(outputDirectory, "wwwroot", "index.min.js");
        await Assert.That(File.Exists(expectedFile)).IsTrue();
    }
}
