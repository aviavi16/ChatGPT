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
    [TestFixture]
    public class TestRun : TestBase
    {


        [Test]
        public void InsertTestRun()
        {
            var testRun = new TestRun
            {
                Id = Guid.NewGuid().ToString(),
                Number = this.Number,  // Assign the instance test run number
                Environment = "Qa",
                Date = this.Date,
                Remote = this.Remote,
                Result = "Success"
                
            };

            Console.WriteLine("Inserting TestRun into MongoDB...");
            Console.WriteLine($" tsssssssssssssssssssssss {testRun.Number}" );
            var collection = _dbContext.GetCollection<TestRun>("TestRuns"); // Access TestRuns collection
            try
            {
                bool IsRemote = _configuration.GetValue<bool>("MongoSettings:IsRemote", defaultValue: true);

                string? connectionStringTemplate = IsRemote ?
                    _configuration.GetConnectionString("Local") :
                    _configuration.GetConnectionString("Remote");
                Console.WriteLine($"Environment setting: {IsRemote}");

                collection.InsertOne(testRun);
                Console.WriteLine("TestRun inserted into MongoDB Atlas.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Rethrow the exception to fail the test
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting TestRun: {ex.Message}");
                throw;
            }
            var count = collection.CountDocuments(Builders<TestRun>.Filter.Empty);
            Console.WriteLine($"Total documents in TestRuns collection: {count}");

            Console.WriteLine("TestRun inserted into MongoDB.");
            Assert.Pass("TestRun inserted into MongoDB");
            //CleanUpTestRuns();
        }
    }
}
