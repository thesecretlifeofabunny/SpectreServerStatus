# SpectreServerStatus.nix
{
  buildDotnetModule,
  dotnetCorePackages,
}:

buildDotnetModule {
  pname = "SpectreServerStatus";
  version = "0.1";

  src = ../.;

  projectFile = "src/../SpectreServerStatus.sln";
  nugetDeps = ./deps.json;

  dotnet-sdk = dotnetCorePackages.sdk_8_0;
  dotnet-runtime = dotnetCorePackages.runtime_8_0;

  executables = [ "SpectreServerStatus" ];

  packNupkg = true; # This packs the project as "foo-0.1.nupkg" at `$out/share`.
}
