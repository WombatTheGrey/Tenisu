<#
	.SYNOPSIS
	Publishes the application to an output directoy
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $artifactsDirectory,
	[Parameter(Mandatory)][String] $projectToPublishPath,
	[String] $publishDirectoryName = 'webapi'
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$artifactsPath = ConvertTo-AbsolutePath $artifactsDirectory
if(! (Test-Path $artifactsPath)){
	throw "Could not find the artifacts at $artifactsPath"
}

$projectPath = ConvertTo-AbsolutePath $projectToPublishPath

$publishDirectory = Join-Path $artifactsPath $publishDirectoryName
if(Test-Path $publishDirectory){
	Write-Host "Cleaning $publishDirectory"
	Remove-Item $publishDirectory -Recurse -Force
}

Invoke-Native -Title "Publishing" -Command {
	dotnet publish $projectPath --no-build `
	--artifacts-path $artifactsPath `
	--configuration Release `
	--output $publishDirectory
}

Write-Host ''
Write-Host "Application published to $publishDirectory" -ForegroundColor Green