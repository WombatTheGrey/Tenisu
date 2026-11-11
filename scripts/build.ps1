<#
	.SYNOPSIS 
	Builds the application and produces the artifacts
#>
[CmdletBinding()]
$commonScriptLocation = "$PSScriptRoot\common.ps1"
Write-Host "Dot-Sourcing $commonScriptLocation"
. $commonScriptLocation

Get-RepoRoot | Set-Location

Invoke-Native -Title 'Check version' -Command { dotnet --version}

Invoke-Native -Title 'Restore Packages' -Command { dotnet Restore}

Invoke-Native -Title 'Build' -Command { dotnet build --artifacts-path .\artifacts --no-restore --configuration Release --verbosity normal}

Write-Host ''
Write-Host 'Build succeeded'  -ForegroundColor Green