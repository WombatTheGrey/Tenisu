<#
	.SYNOPSIS
	Contains general settings and a helper method. Not intended for
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RepoRoot{
	# Split-Path Parent is the same as Cd ..
	# It goes one folder up
	return Split-Path -Parent $PSScriptRoot
}

function Invoke-Native{
	param(
		[Parameter(Mandatory)][string] $Title,
		[Parameter(Mandatory)][scriptblock] $Command
	)

	Write-Host ''
	Write-Host "==> $Title" -ForegroundColor DarkGreen

	# "| Out-Host" sends the program's output straight to the console, to avoid polluting this function's return value.
	& $Command | Out-Host

	if ($LASTEXITCODE -ne 0){
		throw "Command Failed : '$Title' with ExitCode = '$LASTEXITCODE'"
	}
}