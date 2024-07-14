using ChatGPT.Data;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Tests
{
    public abstract class TestBase
    {
        protected MongoDbContext _dbContext { get; private set; }
        protected IConfiguration _configuration { get; private set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();  // Initialize with a new GUID
        public int Number { get; set; }
        public string? Environment { get; set; }
        public string? Result { get; set; }
        public DateTime Date { get; set; }
        public bool Remote { get; set; }

        protected void SetDbContext(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [SetUp]
        public void SetUp()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(System.IO.Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables(); // Add environment variables

            _configuration = builder.Build();

            Console.WriteLine("Configuration loaded successfully.");
            bool IsRemote = _configuration.GetValue<bool>("MongoSettings:IsRemote"); // Initialize remote from configuration
            Console.WriteLine($"IsRemote: {IsRemote}");

            Remote = IsRemote;
            Date = DateTime.Now;

            
            var connectionStringTemplate = GetMongoConnectionString(IsRemote);
            if (string.IsNullOrEmpty(connectionStringTemplate))
            {
                throw new InvalidOperationException($"MongoDB {IsRemote} connection string is missing or empty in appsettings.json");
            }

            string databaseName = GetDatabaseName("Qa");
            var password = _configuration["MongoSettings:Password"];

            string connectionString = connectionStringTemplate
                .Replace("<password>", password)
                .Replace("<DatabaseName>", databaseName);

            Console.WriteLine($" connectionString 2: {connectionString}");
            var dbContext = new MongoDbContext(connectionString, databaseName);
            SetDbContext(dbContext);

            Console.WriteLine("Get Last TestRun Number");
            int lastNumber = _dbContext.GetLastTestRunNumber();
            Number = lastNumber + 1;
            Console.WriteLine($"TestRun Number: {Number}");
        }

        protected string GetMongoConnectionString(bool IsRemote)
        {
            string? connectionString = IsRemote ?
            _configuration.GetConnectionString("Remote") :
            _configuration.GetConnectionString("Local");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string for '{(IsRemote ? "Local" : "Remote")}' is missing or empty in appsettings.json");
            }

            Console.WriteLine($"Using connection string: {connectionString}");


            return connectionString;
        }

        public void CleanUpTestRuns()
        {
            _dbContext.TestRuns.DeleteMany(Builders<TestRun>.Filter.Empty);
        }

        protected virtual string GetDatabaseName(string environment)
        {
            var databaseName = _configuration[$"TestSettings:Environments:{environment}:DatabaseName"];
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new Exception($"MongoDB {environment} database name is missing or empty in appsettings.json");
            }
            return databaseName;
        }
    }
}
