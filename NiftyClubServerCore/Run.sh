#!/bin/sh

set -eu

script_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
export DOTNET_ROLL_FORWARD="${DOTNET_ROLL_FORWARD:-Major}"

exec dotnet "$script_dir/Lib/DarkRift.Server.Console.dll"
