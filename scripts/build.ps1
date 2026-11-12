<#
	.SYNOPSIS
	Builds the application and produces the artifacts
#>
[CmdletBinding()]
param()

$commonScriptPath = "$PsScriptRoot\common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$root = Get-RepoRoot
Write-Host "Setting location at $root"
Set-Location $root

Invoke-Native -Title 'Check version' -Command { dotnet --version}

Invoke-Native -Title 'Restore Packages' -Command { dotnet restore}

Invoke-Native -Title 'Build' -Command { dotnet build --artifacts-path .\artifacts --no-restore --configuration Release --verbosity normal}

Write-Host ''
Write-Host 'Build succeeded'  -ForegroundColor Green