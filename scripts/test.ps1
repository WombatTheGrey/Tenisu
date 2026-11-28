<#
	.SYNOPSIS
	Script used to run tests and export the results
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $solutionName,
	[Parameter(Mandatory)][String] $artifactsDirectory,
	[String] $testResultsDirectory = 'TestResults'
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$solutionPath = ConvertTo-AbsolutePath $solutionName
if(!(Test-Path $solutionPath)){
		throw "Could not find the solution at $solutionPath"
}

$artifactsPath = ConvertTo-AbsolutePath $artifactsDirectory
if(! (Test-Path $artifactsPath)){
	throw "Could not find the artifacts at $artifactsPath"
}

$testResultsPath = Join-Path $artifactsPath $testResultsDirectory
if(Test-Path $testResultsPath){
	Write-Host "Cleaning $testResultsPath content"
	Remove-Item $testResultsPath -Recurse -Force
}

Invoke-Native -Title 'Tests' -Command {
	dotnet test $solutionPath --no-build --configuration Release `
	--artifacts-path $artifactsPath `
	--logger trx --results-directory $testResultsPath
}

Write-Host ''
Write-Host "Tests passed. Reports in: $testResultsPath" -ForegroundColor Green