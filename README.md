# Swallow.MSBuild.Node

An MSBuild adapter to run `npm run build` in `dotnet build` and include the
resulting files as _static assets_.

## Installation

Add a package reference to `Swallow.MSBuild.Node`, but be sure to include the
assets only for `build`ing:

```xml
<PackageReference Include="Swallow.MSBuild.Node" Version="0.1.1">
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

### Properties

| Name                    | Description                                                                                                                  | Default                                                     |
|-------------------------|------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------|
| `BuildToolPath`         | Path to the `npm` installation to use                                                                                        | `npm` (resolved via `$PATH`)                                |
| `BuildScriptName`       | Which npm script to execute on `dotnet build`                                                                                | `build`                                                     |
| `CleanScriptName`       | Which npm script to execute on `dotnet clean`                                                                                | `clean`                                                     |
| `ExcludeWwwRoot`        | Whether to exclude items found in `wwwroot/` from the incremental build                                                      | `true`                                                      |
| `FrontendBuildExcludes` | Which files to exclude for the incremental build                                                                             | See below                                                   |
| `FrontendRootPath`      | The "root" of the frontend build - the `package.json` file is assumed to be in this directory; must include a trailing slash | `$(MSBuildProjectDirectory)/`                               |
| `FrontendOutputPath`    | Where to put the output of `npm run build`; passed as environment variable `$OUT_DIR`                                        | `$(MSBuildProjectDirectory)/$(IntermediateOutputPath)dist/` |

#### Items excluded by default

There are a handful of patterns configured for `FrontendBuildExcludes` by default:

- `$(FrontendRootPath)node_modules/`
- `wwwroot/` (unless `ExcludeWwwRoot` is set to `false`)
- Razor component-specific assets (`$(FrontendRootPath)**/*.razor.js` and `$(FrontendRootPath)**/*.razor.css`)
- Razor page-specific assets (`$(FrontendRootPath)**/*.cshtml.js` and `$(FrontendRootPath)**/*.cshtml.css`)

### Items

| Name             | Description                                                                          |
|------------------|--------------------------------------------------------------------------------------|
| `FrontendBuild`  | Files to be considered for the incremental build; source code (like `Compile`)       |
| `FrontendConfig` | Configuration files impacting an incremental build that are not strictly source code |

`FrontendBuild` includes - by default - all relevant files found in `$(FrontendRootPath)`, whereas `FrontendConfig`
includes the `package.json` and `package-lock.json`. Both item groups will be considered for the incremental build;
if any of these files is _newer_ than the generated output files in `$(FrontendOutputPath)`, a `npm run build` is
triggered.

You can add your own items to any of these item groups if you need to handle additional files not covered by the
default includes. Note that certain files will be ignored even when added to the item groups, most notably those found
in these folders:

- `node_modules/`
- `wwwroot/` (unless `ExcludeWwwRoot` is set to `false`)
- `bin/`
- `obj/`

The default items consider all files with the following extensions as relevant:

- `.js` and `.jsx`
- `.ts` and `.tsx`
- `.css`
- `.scss` and `.sass`
- `.less`
- `.styl`
