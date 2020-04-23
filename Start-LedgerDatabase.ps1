<#
.SYNOPSIS
Starts a SQL Server instance and initializes or performs any necessary migrations to the Ledger database.

.DESCRIPTION
Starts a SQL Server instance and initializes or performs any necessary migrations to the Ledger database.

.PARAMETER NoNewWindow
When specified, starts the server using the current console window and waits for the process to end.

.EXAMPLE
PS> Start-LedgerDatabase

Starts the SQL Server instance as a new window.

.EXAMPLE
PS> Start-LedgerDatabase -NoNewWindow

Starts the SQL Server instance in the current window.
#>
param (
    [Parameter()]
    [switch] $NoNewWindow
)

Start-Process `
    docker-compose `
    @(
        "up" 
        "db"
    ) `
    -NoNewWindow:$NoNewWindow `
    -Wait:$NoNewWindow
