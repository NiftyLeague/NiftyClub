if ([string]::IsNullOrWhiteSpace($env:DOTNET_ROLL_FORWARD)) {
    $env:DOTNET_ROLL_FORWARD = "Major"
}

& dotnet (Join-Path $PSScriptRoot "Lib/DarkRift.Server.Console.dll")
exit $LASTEXITCODE
