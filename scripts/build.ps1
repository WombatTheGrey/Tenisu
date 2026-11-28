<#
	.SYNOPSIS
	Builds the a solution and produces the artifacts
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $solutionName,
	[Parameter(Mandatory)][String] $artifactsDirectory,
	[String] $VersionSuffix = 'untagged'
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$solutionPath = ConvertTo-AbsolutePath $solutionName
if(!(Test-Path $solutionPath)){
		throw "Could not find the solution at $solutionPath"
}

$artifactsPath = ConvertTo-AbsolutePath $artifactsDirectory
if(Test-Path $artifactsPath){
	Write-Host "Cleaning $artifactsPath content"
	Remove-Item $artifactsPath -Recurse -Force
}

Invoke-Native -Title 'Check version' -Command { dotnet --version }
Invoke-Native -Title 'Restore Packages' -Command { dotnet restore $solutionPath }

Invoke-Native -Title 'Build' -Command {
	dotnet build $solutionPath --no-restore `
	--artifacts-path $artifactsPath `
	--configuration Release `
	"-p:VersionSuffix=$VersionSuffix"
}

Write-Host ''
Write-Host 'Build succeeded'  -ForegroundColor Green