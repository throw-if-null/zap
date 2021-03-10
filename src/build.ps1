$ErrorActionPreference = 'Stop'

dotnet tool install --global dotnet-sonarscanner --version 5.1.0

Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet tool install --global Cake.Tool --version 1.1.0

dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet cake @args
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }