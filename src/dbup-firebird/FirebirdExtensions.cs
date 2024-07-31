using System;
using System.IO;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Firebird;
using FirebirdSql.Data.FirebirdClient;

// ReSharper disable once CheckNamespace

/// <summary>
/// Configuration extension methods for Firebird.
/// </summary>
public static class FirebirdExtensions
{
    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">Firebird database connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(this SupportedDatabases supported, string connectionString)
    {
        return FirebirdDatabase(new FirebirdConnectionManager(connectionString));
    }

    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="FirebirdConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
        => FirebirdDatabase(connectionManager);

    /// <summary>
    /// Creates an upgrader for Firebird databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="FirebirdConnectionManager"/> to be used during a database upgrade.</param>
    /// <returns>
    /// A builder for a database upgrader designed for Firebird databases.
    /// </returns>
    public static UpgradeEngineBuilder FirebirdDatabase(IConnectionManager connectionManager)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new FirebirdScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new FirebirdTableJournal(() => c.ConnectionManager, () => c.Log, "schemaversions"));
        builder.WithPreprocessor(new FirebirdPreprocessor());
        return builder;
    }


    //The code below concerning EnsureDatabase and DropDatabase is a modified version from a PR from Github user @hhindriks. Thank you for your contribution.

    //Error codes from Firebird (see https://www.firebirdsql.org/pdfrefdocs/Firebird-2.1-ErrorCodes.pdf)
    const int FbIoError = 335544344;
    const int FbNetworkError = 335544721;
    const int FbLockTimeout = 335544510;

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, IUpgradeLog logger = null)
    {
        logger ??= new ConsoleUpgradeLog();
        var builder = new FbConnectionStringBuilder(connectionString);

        if (builder.ServerType == FbServerType.Embedded)
        {
            //The code for the embedded servertype is currently not tested.
            //Comes from the original PR from @hhindriks
            if (!File.Exists(builder.Database))
            {
                FbConnection.CreateDatabase(builder.ToString());
                logger.WriteInformation("Created database {0}", builder.Database);
            }
            else
            {
                logger.WriteInformation("Database {0} already exists", builder.Database);
            }
        }
        else
        {
            using var conn = new FbConnection(builder.ToString());
            try
            {
                conn.Open();
                conn.Close();
                logger.WriteInformation("Database {0} already exists", builder.Database);
            }
            catch (FbException ex) when (ex.ErrorCode == FbIoError)
            {
                FbConnection.CreateDatabase(builder.ToString());
                logger.WriteInformation("Created database {0}", builder.Database);
            }
            catch (FbException ex) when (ex.ErrorCode == FbNetworkError)
            {
                logger.WriteError("Could not access server. The server: {0} is probably not started.", builder.DataSource);
                throw;
            }
            catch (FbException)
            {
                logger.WriteError("Ensure Database: Unknown firebird error when trying to access the server: {0}.", builder.DataSource);
                throw;
            }
            catch (Exception)
            {
                logger.WriteError("Ensure Database: Unknown error when trying to access the server: {0}.", builder.DataSource);
                throw;
            }
        }
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <returns></returns>
    public static void FirebirdDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, IUpgradeLog logger = null)
    {
        logger ??= new ConsoleUpgradeLog();
        var builder = new FbConnectionStringBuilder(connectionString);

        if (builder.ServerType == FbServerType.Embedded)
        {
            //The code for the embedded servertype is currently not tested.
            //Comes from the original PR from @hhindriks
            if (File.Exists(builder.Database))
            {
                FbConnection.DropDatabase(builder.ToString());
                logger.WriteInformation("Dropped database {0}", builder.Database);
            }
        }
        else
        {
            try
            {
                //There seems to be an error in the FirebirdClient when trying to drop a database that does not exist.
                //It gives a NullRefException instead of the expected FbException.
                FbConnection.DropDatabase(builder.ToString());
                logger.WriteInformation("Dropped database {0}", builder.Database);
            }
            catch (FbException ex) when (ex.ErrorCode == FbIoError)
            {
                logger.WriteWarning("Nothing to Drop. No database found.");
            }
            catch (FbException ex) when (ex.ErrorCode == FbLockTimeout)
            {
                logger.WriteError("Can't drop database. Are there still an active connection?");
                throw;
            }
            catch (FbException)
            {
                logger.WriteError("Drop Database: Unknown firebird error when trying to access the server: {0}.", builder.DataSource);
                throw;
            }
            catch (Exception)
            {
                logger.WriteError("Drop Database: Unknown error when trying to access the server: {0}.", builder.DataSource);
                throw;
            }
        }
    }

}
