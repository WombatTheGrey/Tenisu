<#
	.SYNOPSIS 
	Zips and deploys the packages to Azure App Service
#>
[CmdletBinding()]
param(
	[Parameter(Mandatory)][String] $publishPath,
	[Parameter(Mandatory)][String] $resourceGroup,
	[Parameter(Mandatory)][String] $appName
)

$commonScriptPath = Join-Path $PsScriptRoot "common.ps1"
Write-Host "Dot-sourcing $commonScriptPath"
. $commonScriptPath

$publishAbsolutePath = ConvertTo-AbsolutePath $publishPath
if(!(Test-Path $publishAbsolutePath)){
		throw "Could not find the publish folder at $publishAbsolutePath"
}

$publishDirectoryName = Split-Path $publishPath -Leaf
$publishDirectoryParent = Split-Path $publishPath -Parent
$zipPath = Join-Path $publishDirectoryParent "$publishDirectoryName.zip"




#Check if we are correctly logged in to Azure CLI
Invoke-Native -Title 'Check Azure CLI login' -Command {
	az account show --output none
}
