<#
.SYNOPSIS
Starts the Ledger WebApi project and all necessary dependencies.

.DESCRIPTION
Starts the Ledger WebApi project and all necessary dependencies.

.PARAMETER NoNewWindow
When specified, starts the server using the current console window and waits for the process to end.

.EXAMPLE
PS> Start-LedgerServer

Starts the Ledger server as a new window.

.EXAMPLE
PS> Start-LedgerServer -NoNewWindow

Starts the Ledger server using the existing console window.
#>
param (
    [Parameter()]
    [switch] $NoNewWindow
)

Start-Process `
    docker-compose `
    @(
        "up" 
        "--build"
        "--force-recreate"
    ) `
    -NoNewWindow:$NoNewWindow `
    -Wait:$NoNewWindow
