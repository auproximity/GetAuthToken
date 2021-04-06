#!/bin/bash
set -euo pipefail

rm Hazel-Networking/Hazel/Properties/AssemblyInfo.cs
cp ./Hazel.csproj Hazel-Networking/Hazel/Hazel.csproj
pushd Hazel-Networking/Hazel
dotnet build -c Release
popd

pushd GetAuthToken
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
popd

