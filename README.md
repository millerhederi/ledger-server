# Ledger

Ledger is in early development stages, but the goal is to become a web based double-entry accounting system.

# Running

To run Ledger locally, simply execute:

```
.\Start-LedgerServer.ps1
```

# Developing

The quickest way to build and run/debug the application from within Visual Studio is to set `docker-compose` as the Startup Project and run that. This will launch a local SQL Server instance and either initialize it or perform any necessary database migrations. It will also launch the actual application which can be debugged as normal.

If you already have an existing database instance or simply don't want to use `docker-compose` for any other reason, simply update the connection strings in the `Ledger.DatabaseInstaller` and `Ledger.WebApi` projects via the `appsettings.json`. To either initialize the database or perform any database migrations, run the `Ledger.DatabaseInstaller` project first, then you can run the `Ledger.WebApi` project.

## Testing

The `Ledger.Tests` project has end to end integration tests, so a running instance of SQL Server is required. To use an existing instance, update the connection strings in `appsettings.json` and run the `Ledger.DatabaseInstaller` project first to initialize the database or perform any migrations. If you do not already have a runing instance of SQL Server, run:

```
docker-compose up db
```

This will launch a new instance via docker. Again, run `Ledger.DatabaseInstaller` first if you have not done so already. At this point you can run all tests.