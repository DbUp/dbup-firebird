using DbUp.Support;

namespace DbUp.Firebird
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class FirebirdObjectParser : SqlObjectParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdObjectParser"/> class.
        /// </summary>
        public FirebirdObjectParser() : base("\"", "\"")
        {
        }
    }
}
