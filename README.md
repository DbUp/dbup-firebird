[![GitHub Workflow Status (branch)](https://img.shields.io/github/actions/workflow/status/DbUp/dbup-firebird/main.yml?branch=main)](https://github.com/DbUp/dbup-firebird/actions/workflows/main.yml?query=branch%3Amain)
[![NuGet](https://img.shields.io/nuget/dt/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird)
[![NuGet](https://img.shields.io/nuget/v/dbup-firebird.svg)](https://www.nuget.org/packages/dbup-firebird)
[![Prerelease](https://img.shields.io/nuget/vpre/dbup-firebird?color=orange&label=prerelease)](https://www.nuget.org/packages/dbup-firebird)

# DbUp Firebird support
DbUp is a .NET library that helps you to deploy changes to relational databases. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.
This Provider is for deploying to Firebird databases.

## Getting Help
To learn more about DbUp check out the [documentation](https://dbup.readthedocs.io/en/latest/)

Please only log issue related to Firebird support in this repo. For cross cutting issues, please use our [main issue list](https://github.com/DbUp/DbUp/issues).

# Contributing

See the [readme in our main repo](https://github.com/DbUp/DbUp/blob/master/README.md) for how to get started and contribute.

# Quirks concerning the Firebird implementation

The Journal Table for Firebird is called `"schemaversions"` i.e. with quotes. This can be confusing since other providers do not use quotes.

It will not be fixed because of backwards compatibility. You can check the content of the table by using

```sql
select * from "schemaversions"
```

In a new project you can also choose another name for the JournalTable instead like this:

```csharp
    public static UpgradeEngineBuilder JournalToFirebirdTable(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c => c.Journal = new FirebirdTableJournal(() => c.ConnectionManager, () => c.Log, "ABetterTableName"));
        return builder;

```