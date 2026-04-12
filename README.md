# Swallow.MSBuild.Node

An MSBuild adapter to run `npm run build` in `dotnet build` and include the
resulting files as _static assets_.

## Installation

Add a package reference to `Swallow.MSBuild.Node`, but be sure to include the
assets only for `build`ing:

```xml
<PackageReference Include="Swallow.MSBuild.Node" Version="0.1.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>build</IncludeAssets>
</PackageReference>
```

After that, once you've got a `package.json` in the root of your project, MSBuild
will...

- Run `npm install` on a `dotnet restore` (if needed)
- Run `npm run build` on a `dotnet build` (if needed)
- Run `npm run clean` on a `dotnet clean`

If the `build` or `clean` scrpt is not defined, it will just not run anything.
All scripts will be executed with `$OUT_DIR` set to the target directory; if you
write to any file, be sure to pass that environment variable to any tooling.
Otherwise, the resulting output will not be included as static asset.

## Configuring

`Swallow.MSBuild.Node` can be configured in a number of ways. Shown is the default
value for every property:

```xml
<PropertyGroup>
  <!-- All extensions of files that should be considered for the build -->
  <FrontendCompilePatterns>*.js;*.jsx;*.ts;*.tsx</FrontendCompilePatterns>

  <!-- Whether to ignore files in wwwroot/ for the up-to-date check. -->
  <ExcludeWwwRoot>true</ExcludeWwwRoot>
  <FrontendCompileExcludes Condition="$(ExcludeWwwRoot) == 'true'">wwwroot/**/*</FrontendCompileExcludes>

  <!--
    Set the "root" of the frontend build to this directory. Be sure to include a trailing slash!
    The 'package.json', 'package-lock.json' and 'node_modules' are expected to be in this directory.
  -->
  <FrontendRootPath>$(MSBuildProjectDirectory)/</FrontendRootPath>

  <!-- Path to a specific 'npm' installation to use -->
  <BuildToolPath>npm</BuildToolPath>

  <!-- The npm script to execute for 'dotnet build' -->
  <BuildScriptName>build</BuildScriptName>

  <!-- The npm script to execute for 'dotnet clean' -->
  <CleanScriptName>clean</CleanScriptName>
</PropertyGroup>
```
