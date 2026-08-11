[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidatePattern('^[^@\s]+@[^@\s]+\.[^@\s]+$')]
    [string]$Email,

    [string]$ConfigPath
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $projectRoot 'src\MyWeb.Portal\MyWeb.Portal.csproj'

$env:ASPNETCORE_ENVIRONMENT = 'Development'
if ($ConfigPath) {
    $resolvedConfig = Resolve-Path -LiteralPath $ConfigPath
    $env:MYWEB_CONFIG_FILE = $resolvedConfig.Path
}

dotnet run --project $projectPath -- --bootstrap-admin=true --email=$Email
exit $LASTEXITCODE
