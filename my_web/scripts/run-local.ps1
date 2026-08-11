[CmdletBinding()]
param(
    [string]$ConfigPath,
    [int]$Port
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $projectRoot 'src\MyWeb.Portal\MyWeb.Portal.csproj'

$env:ASPNETCORE_ENVIRONMENT = 'Development'
if ($ConfigPath) {
    $resolvedConfig = Resolve-Path -LiteralPath $ConfigPath
    $env:MYWEB_CONFIG_FILE = $resolvedConfig.Path
}

if ($Port) {
    $env:MYWEB_Server__Port = $Port.ToString()
}

dotnet run --project $projectPath
exit $LASTEXITCODE
