[assembly: System.CLSCompliant(true)]
[assembly: System.Reflection.AssemblyMetadata("RepositoryUrl", "https://github.com/DbUp/dbup-firebird.git")]
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.Runtime.Versioning.TargetFramework(".NETStandard,Version=v2.0", FrameworkDisplayName=".NET Standard 2.0")]
public static class FirebirdExtensions
{
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(this DbUp.Builder.SupportedDatabases supported, DbUp.Engine.Transactions.IConnectionManager connectionManager) { }
    public static DbUp.Builder.UpgradeEngineBuilder FirebirdDatabase(this DbUp.Builder.SupportedDatabases supported, string connectionString) { }
    public static void FirebirdDatabase(this DbUp.SupportedDatabasesForDropDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger = null) { }
    public static void FirebirdDatabase(this DbUp.SupportedDatabasesForEnsureDatabase supported, string connectionString, DbUp.Engine.Output.IUpgradeLog logger = null) { }
}
namespace DbUp.Firebird
{
    public class FirebirdConnectionManager : DbUp.Engine.Transactions.DatabaseConnectionManager
    {
        public FirebirdConnectionManager(string connectionString) { }
        protected override DbUp.Engine.Transactions.AllowedTransactionMode AllowedTransactionModes { get; }
        public override System.Collections.Generic.IEnumerable<string> SplitScriptIntoCommands(string scriptContents) { }
    }
    public class FirebirdObjectParser : DbUp.Support.SqlObjectParser
    {
        public FirebirdObjectParser() { }
    }
    public class FirebirdPreprocessor : DbUp.Engine.IScriptPreprocessor
    {
        public FirebirdPreprocessor() { }
        public string Process(string contents) { }
    }
    public class FirebirdScriptExecutor : DbUp.Support.ScriptExecutor
    {
        public FirebirdScriptExecutor(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManagerFactory, System.Func<DbUp.Engine.Output.IUpgradeLog> log, string schema, System.Func<bool> variablesEnabled, System.Collections.Generic.IEnumerable<DbUp.Engine.IScriptPreprocessor> scriptPreprocessors, System.Func<DbUp.Engine.IJournal> journal) { }
        protected override bool UseTheSameTransactionForJournalTableAndScripts { get; }
        protected override void ExecuteCommandsWithinExceptionHandler(int index, DbUp.Engine.SqlScript script, System.Action executeCommand) { }
        protected override string GetVerifySchemaSql(string schema) { }
    }
    public class FirebirdTableJournal : DbUp.Support.TableJournal
    {
        public FirebirdTableJournal(System.Func<DbUp.Engine.Transactions.IConnectionManager> connectionManager, System.Func<DbUp.Engine.Output.IUpgradeLog> logger, string tableName) { }
        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName) { }
        protected override string DoesTableExistSql() { }
        protected override string GetInsertJournalEntrySql(string scriptName, string applied) { }
        protected override string GetJournalEntriesSql() { }
        protected override void OnTableCreated(System.Func<System.Data.IDbCommand> dbCommandFactory) { }
    }
}