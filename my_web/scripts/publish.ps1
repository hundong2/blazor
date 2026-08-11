[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+([-.][0-9A-Za-z.-]+)?$')]
    [string]$Version = '0.1.0-dev',

    [string]$OutputRoot
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$outputBase = if ($OutputRoot) { $OutputRoot } else { Join-Path $projectRoot '.artifacts\releases' }
$releasePath = Join-Path $outputBase $Version
$projectPath = Join-Path $projectRoot 'src\MyWeb.Portal\MyWeb.Portal.csproj'

if (Test-Path -LiteralPath $releasePath) {
    throw "Release path already exists: $releasePath"
}

New-Item -ItemType Directory -Path $releasePath -Force | Out-Null
dotnet publish $projectPath -c Release -r win-x64 --self-contained false -p:RestoreLockedMode=true -o $releasePath
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

Copy-Item -LiteralPath (Join-Path $projectRoot 'config\appsettings.Production.example.json') -Destination $releasePath
Copy-Item -LiteralPath (Join-Path $projectRoot 'env') -Destination (Join-Path $releasePath 'env') -Recurse
Copy-Item -LiteralPath (Join-Path $projectRoot 'scripts') -Destination (Join-Path $releasePath 'scripts') -Recurse

$manifest = [ordered]@{
    version = $Version
    createdAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    runtime = 'win-x64'
    frameworkDependent = $true
}
$manifest | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $releasePath 'release-manifest.json') -Encoding UTF8

Write-Output "Published My Web $Version to $releasePath"
