using System.IO.Compression;
using Swallow.MSBuild.Node.Test.Framework;

namespace Swallow.MSBuild.Node.Test;

public sealed class PackTest() : MSBuildTestBase("pack")
{
    protected override string GetSourceProjectPath(string solutionRoot) => Path.Combine(solutionRoot, "test", "TestPackage");
    protected override IEnumerable<string> GetTargetFileNames() => ["TestPackage.csproj", "Client/index.js", "build.js", "package.json", "package-lock.json", "NuGet.config"];

    [Test]
    public async Task DirectlyPacking_HasAssetInPackage()
    {
        var dotnet = new Dotnet();

        var outputDirectory = Path.Combine(CurrentDirectory, "publish");
        dotnet.Pack(outputDirectory, ProjectPath);

        await ZipFile.ExtractToDirectoryAsync(Path.Combine(outputDirectory, "TestPackage.1.0.0.nupkg"), outputDirectory);

        var expectedFile = Path.Combine(outputDirectory, "staticwebassets", "index.min.js");
        await Assert.That(File.Exists(expectedFile)).IsTrue();
    }

    [Test]
    public async Task ExplicitBuildBeforePack_HasAssetInPackage()
    {
        var dotnet = new Dotnet();

        dotnet.Restore(ProjectPath);
        dotnet.Build(ProjectPath, Dotnet.BuildFlags.NoRestore);

        var outputDirectory = Path.Combine(CurrentDirectory, "publish");
        dotnet.Pack(outputDirectory, ProjectPath, Dotnet.BuildFlags.NoRestore | Dotnet.BuildFlags.NoBuild);

        await ZipFile.ExtractToDirectoryAsync(Path.Combine(outputDirectory, "TestPackage.1.0.0.nupkg"), outputDirectory);

        var expectedFile = Path.Combine(outputDirectory, "staticwebassets", "index.min.js");
        await Assert.That(File.Exists(expectedFile)).IsTrue();
    }
}
