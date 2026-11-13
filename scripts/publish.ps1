<#
	.SYNOPSIS
	Publishes the Web API to a folder ready for deployment
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $artifactsDirectory,
	[Parameter(Mandatory)][String] $outputDirectory
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$root = Get-RepoRoot
$artifactsPath = Join-Path $root $artifactsDirectory
if(! (Test-Path $artifactsPath)){
	throw "Could not find the artifacts at $artifactsPath"
}

$outputPath = Join-Path $artifactsPath $outputDirectory
if(Test-Path $outputPath){
	Write-Host "Cleaning $outputPath"
	Remove-Item $outputPath -Recurse -Force
}

Invoke-Native -Title "Publishing" -Command {
	dotnet publish --no-build --artifacts-path $artifactsPath `
	--configuration Release --output $outputPath
}

Write-Host ''
Write-Host "Application published to $outputPath" -ForegroundColor Green