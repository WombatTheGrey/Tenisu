<#
	.SYNOPSIS
	Script used to run tests and export the results
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $solutionName,
	[Parameter(Mandatory)][String] $artifactsDirectory
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$root = Get-RepoRoot
$solutionPath = Join-Path $root $solutionName
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
	dotnet test $solutionPath --no-build --configuration Release `
	--artifacts-path $artifactsPath `
	--logger trx --results-directory $testResultsDirectory
}

Write-Host ''
Write-Host "Tests passed. Reports in: $testResultsDirectory" -ForegroundColor Green