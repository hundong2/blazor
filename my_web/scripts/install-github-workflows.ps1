[CmdletBinding(SupportsShouldProcess)]
param(
    [switch]$Apply
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$repositoryRoot = (& git -C $projectRoot rev-parse --show-toplevel).Trim()
if (-not $repositoryRoot) {
    throw 'Git repository root was not found.'
}

$source = Join-Path $projectRoot 'deploy\github\workflows'
$target = Join-Path $repositoryRoot '.github\workflows'
$templates = Get-ChildItem -LiteralPath $source -Filter '*.yml'

Write-Output "Workflow source: $source"
Write-Output "Workflow target: $target"
foreach ($template in $templates) {
    Write-Output "  $($template.Name)"
}

if (-not $Apply) {
    Write-Output 'Preview only. Re-run with -Apply to install these workflows at repository root.'
    return
}

if ($PSCmdlet.ShouldProcess($target, 'Install My Web GitHub Actions workflows')) {
    New-Item -ItemType Directory -Path $target -Force | Out-Null
    foreach ($template in $templates) {
        Copy-Item -LiteralPath $template.FullName -Destination (Join-Path $target $template.Name) -Force
    }
    Write-Output 'GitHub Actions workflows installed. Review the diff before commit.'
}
