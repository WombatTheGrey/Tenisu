<#
	.SYNOPSIS
	Script used to run tests and export the results
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $artifactsDirectory
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$root = Get-RepoRoot
$artifactsPath = Join-Path $root $artifactsDirectory
if(! (Test-Path $artifactsPath)){
	throw "Could not find the artifacts at $artifactsPath"
}

$testResultsDirectory = Join-Path $artifactsPath 'TestResults'
if(Test-Path $testResultsDirectory){
	Write-Host "Cleaning $testResultsDirectory content"
	Remove-Item $testResultsDirectory -Recurse -Force
}

Invoke-Native -Title 'Tests' -Command {
	dotnet test "$artifactsDirectory\bin\*.Tests\release\*.Tests.dll" `
	--logger trx --results-directory $testResultsDirectory `
	-- RunConfiguration.MaxCpuCount=0
	}

Write-Host ''
Write-Host "Tests passed. Reports in: $testResultsDirectory" -ForegroundColor Green