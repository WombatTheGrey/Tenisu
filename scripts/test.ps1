<#
	.SYNOPSIS
	Script used to run tests and export the results
#>
[CmdletBinding()]
param()

$commonScriptPath = "$PsScriptRoot\common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$root = Get-RepoRoot
Write-Host "Setting location at $root"
Set-Location $root

$testResultsDirectory = Join-Path $root "artifacts\TestResults"

if(Test-Path $testResultsDirectory){
	Write-Host "Cleaning $testResultsDirectory content"
	Remove-Item $testResultsDirectory -Recurse -Force
}

Invoke-Native -Title 'Tests' -Command {
	dotnet test '.\artifacts\bin\*.Tests\release\*.Tests.dll' `
	--logger trx --results-directory $testResultsDirectory `
	-- RunConfiguration.MaxCpuCount=0
	}

Write-Host ''
Write-Host "Tests passed. Reports in: $testResultsDirectory" -ForegroundColor Green