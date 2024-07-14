using ChatGPT.Tests;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));
            }

            try
            {
                Console.WriteLine("Connecting to MongoDB...");
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
                Console.WriteLine($"Connected to MongoDB. Database: {_database} connectionString: {connectionString}");

                CreateIndexes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
                throw;
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            Console.WriteLine($"Getting collection: {collectionName}");
            return _database.GetCollection<T>(collectionName);
        }

        private void CreateIndexes()
        {
            try
            {
                // Define the index keys to be created, in this case, descending on Date
                var indexKeysDefinition = Builders<TestRun>.IndexKeys.Descending(tr => tr.Date);

                // Create an index model with the specified index keys
                var indexModel = new CreateIndexModel<TestRun>(indexKeysDefinition);

                // Create the index in the TestRuns collection
                TestRuns.Indexes.CreateOne(indexModel);

                Console.WriteLine("Index on 'Date' field created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create index on 'Date' field: {ex.Message}");
            }
        }

        public int GetLastTestRunNumber()
        {
            try
            {
                var lastTestRun = TestRuns
                    .Find(Builders<TestRun>.Filter.Empty)
                    .SortByDescending(tr => tr.Number)  // Sort by Number to get the latest test run
                    .Limit(1)
                    .FirstOrDefault();

                if (lastTestRun != null)
                {
                    Console.WriteLine($"Last TestRun found: {lastTestRun.Id} with Number: {lastTestRun.Number}");
                    return lastTestRun.Number;
                }
                else
                {
                    Console.WriteLine("No TestRuns found in the collection.");
                    return 0; // or any default value you prefer
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
                throw;
            }           
        }

        public IMongoCollection<TestRun> TestRuns => _database.GetCollection<TestRun>("TestRuns");
    }
}
