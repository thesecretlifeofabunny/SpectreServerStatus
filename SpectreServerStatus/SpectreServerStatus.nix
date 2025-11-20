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

  dotnet-sdk = dotnetCorePackages.sdk_10_0;
  dotnet-runtime = dotnetCorePackages.runtime_10_0;

  executables = [ "SpectreServerStatus" ];

  packNupkg = true;
}
