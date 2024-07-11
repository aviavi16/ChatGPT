using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/*
 * This class will contain test methods to validate various API endpoints
 * provided by JsonPlaceholder
*/
namespace ChatGPT.Tests
{
    [TestFixture]
    public class JsonPlaceholderTest
    {
        //Intialize RestClient with the base URL
        private RestClient client; 

        [Test]
        public void TestGetPosts()
        {
            var requst = new RestRequest("/posts", Method.Get);

            var response = client.Execute(requst);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

    }
}
