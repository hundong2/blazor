[CmdletBinding()]
param(
    [string]$ConfigPath
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$defaultConfig = 'C:\ProgramData\MyWeb\config\appsettings.Production.json'
$fallbackConfig = Join-Path $projectRoot 'config\appsettings.Production.example.json'
$selectedConfig = if ($ConfigPath) { $ConfigPath } elseif (Test-Path -LiteralPath $defaultConfig) { $defaultConfig } else { $fallbackConfig }
$resolvedConfig = Resolve-Path -LiteralPath $selectedConfig
$config = Get-Content -LiteralPath $resolvedConfig -Raw | ConvertFrom-Json
$results = [System.Collections.Generic.List[object]]::new()

function Add-Result {
    param([string]$Check, [string]$Status, [string]$Detail)
    $results.Add([pscustomobject]@{ Check = $Check; Status = $Status; Detail = $Detail })
}

Add-Result 'Config' 'PASS' $resolvedConfig.Path

if ($config.Server.BindAddress -in @('127.0.0.1', '::1')) {
    Add-Result 'Portal bind' 'PASS' "$($config.Server.BindAddress):$($config.Server.Port)"
} else {
    Add-Result 'Portal bind' 'FAIL' 'BindAddress must be loopback.'
}

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnet) {
    $sdkVersion = (& dotnet --version).Trim()
    Add-Result '.NET SDK' 'PASS' $sdkVersion
} else {
    Add-Result '.NET SDK' 'FAIL' 'dotnet was not found in PATH.'
}

$listener = Get-NetTCPConnection -LocalPort $config.Server.Port -State Listen -ErrorAction SilentlyContinue
if ($listener) {
    $addresses = ($listener.LocalAddress | Sort-Object -Unique) -join ', '
    $unsafe = $listener | Where-Object LocalAddress -NotIn @('127.0.0.1', '::1')
    Add-Result 'Portal listener' $(if ($unsafe) { 'FAIL' } else { 'PASS' }) "$addresses (PID $($listener[0].OwningProcess))"
} else {
    Add-Result 'Portal listener' 'INFO' 'Portal is not currently running.'
}

try {
    $iis = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
    Add-Result 'IIS' $(if ($iis.State -eq 'Enabled') { 'PASS' } else { 'WARN' }) "State: $($iis.State)"
    $webSockets = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebSockets
    Add-Result 'IIS WebSocket' $(if ($webSockets.State -eq 'Enabled') { 'PASS' } else { 'WARN' }) "State: $($webSockets.State)"
} catch {
    Add-Result 'IIS' 'INFO' 'Run doctor from an elevated PowerShell to inspect Windows features.'
    Add-Result 'IIS WebSocket' 'INFO' 'Feature state not available without elevation.'
}

try {
    $publicUri = [Uri]$config.MyWeb.PublicUrl
    if ($publicUri.DnsSafeHost.EndsWith('.example.com', [StringComparison]::OrdinalIgnoreCase)) {
        Add-Result 'Public DNS' 'INFO' 'Replace the example.com placeholder before deployment.'
    } else {
        $addresses = [Net.Dns]::GetHostAddresses($publicUri.DnsSafeHost)
        Add-Result 'Public DNS' 'PASS' (($addresses.IPAddressToString | Sort-Object -Unique) -join ', ')
    }
} catch {
    Add-Result 'Public DNS' 'WARN' $_.Exception.Message
}

if (Get-Command docker -ErrorAction SilentlyContinue) {
    try {
        $dockerVersion = (& docker version --format '{{.Server.Version}}' 2>&1)
        Add-Result 'Docker' $(if ($LASTEXITCODE -eq 0) { 'PASS' } else { 'WARN' }) $(if ($LASTEXITCODE -eq 0) { $dockerVersion } else { 'CLI found, daemon unavailable.' })
    } catch {
        Add-Result 'Docker' 'WARN' 'CLI found, daemon unavailable.'
    }
} else {
    Add-Result 'Docker' 'INFO' 'Not installed; optional until container services are enabled.'
}

$renewalTask = Get-ScheduledTask -ErrorAction SilentlyContinue | Where-Object TaskName -Like '*win-acme*' | Select-Object -First 1
Add-Result 'win-acme renewal' $(if ($renewalTask) { 'PASS' } else { 'INFO' }) $(if ($renewalTask) { $renewalTask.TaskName } else { 'Scheduled task not found.' })

$results | Format-Table -AutoSize
if ($results.Status -contains 'FAIL') {
    exit 1
}

exit 0
