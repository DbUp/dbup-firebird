using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Transactions;
using FirebirdSql.Data.FirebirdClient;

namespace DbUp.Firebird
{
    /// <summary>
    /// Manages Firebird database connections.
    /// </summary>
    public class FirebirdConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Creates a new Firebird database connection.
        /// </summary>
        /// <param name="connectionString">The Firebird connection string.</param>
        public FirebirdConnectionManager(string connectionString) : base(new DelegateConnectionFactory(l => new FbConnection(connectionString)))
        {
        }

        /// <inheritdoc/>
        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            // TODO: Possible Change - this is the PostGres version
            var scriptStatements =
                Regex.Split(scriptContents, "^\\s*;\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            return scriptStatements;
        }

        /// <inheritdoc/>
        protected override AllowedTransactionMode AllowedTransactionModes => AllowedTransactionMode.TransactionPerScript;
    }
}
