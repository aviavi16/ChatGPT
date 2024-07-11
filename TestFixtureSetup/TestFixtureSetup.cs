using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.TestFixtureSetup
{
    [SetUpFixture]
    public class TestFixtureSetup
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Performe setup operations that apply to all test fixtures
            
            //client = new RestClient("h"); //TODO
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() { 
            //Perform teardown operations that apply to all test fixtures
        }
    }
}
