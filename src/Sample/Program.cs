using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DbUp;

namespace FirebirdSampleApplication
{
    class Program
    {
        static int Main()
        {
            var config = GetConfig();
            string connectionString = config.GetConnectionString("SampleFirebird");

            // If you used `docker compose up` for creating a server and a database, the database already exists.
            // You can see that a new database can be created using EnsureDatabase.For.FirebirdDatabase(connectionString) by changing the Database parameter (the fdb filename)
            // in the connectionString in appsettings.json
            // You can also try to drop a database by using DropDatabase.For.FirebirdDatabase(connectionString);
            EnsureDatabase.For.FirebirdDatabase(connectionString);

            var upgrader =
                DeployChanges.To
                    .FirebirdDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .WithTransactionPerScript()
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

                WaitIfDebug();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            WaitIfDebug();
            return 0;
        }

        private static IConfiguration GetConfig()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            return config;
        }

        [Conditional("DEBUG")]
        public static void WaitIfDebug()
        {
            Console.ReadLine();
        }
    }
}
