<#
	.SYNOPSIS
	Contains general settings and a helper method
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RepoRoot{
	# Split-Path Parent is the same as Cd ..
	# It goes one folder up
	return Split-Path $PSScriptRoot -Parent
}

function ConvertTo-AbsolutePath{
	param(
		[Parameter(Mandatory)][String] $path
		)
	$root = Get-RepoRoot
	return [System.IO.Path]::GetFullPath($path, $root)	
}

function Invoke-Native{
	param(
		[Parameter(Mandatory)][String] $Title,
		[Parameter(Mandatory)][Scriptblock] $Command
	)

	Write-Host ''
	Write-Host "==> $Title" -ForegroundColor DarkGreen

	# "| Out-Host" sends the program's output straight to the console, to avoid polluting this function's return value.
	& $Command | Out-Host

	if ($LASTEXITCODE -ne 0){
		throw "Command Failed : '$Title' with ExitCode = '$LASTEXITCODE'"
	}
}