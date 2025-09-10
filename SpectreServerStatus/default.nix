# default.nix

let
  nixpkgs = fetchTarball "https://github.com/NixOS/nixpkgs/tarball/nixos-25.05";
  pkgs = import nixpkgs {};
in
{
  SpectreServerStatus = pkgs.callPackage ./SpectreServerStatus.nix { };
 }

