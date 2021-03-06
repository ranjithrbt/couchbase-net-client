using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.IntegrationTests.Utils;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Couchbase.IntegrationTests
{
    [TestFixture]
    public class EnhancedErrorMessageTests
    {
        [Ignore("Only supported on CouchbaseMock")]
        public async Task Test_Enhanced_Error_Messages()
        {
            // boostrap client
            var config = TestConfiguration.GetConfiguration("basic");

            var serverUri = config.Servers.First();
            using (var client = new HttpClient())
            {
                using (var cluster = new Cluster(config))
                {
                    cluster.SetupEnhancedAuth();
                    var bucket = cluster.OpenBucket("default");

                    // make sure document doesn't exist
                    const string documentKey = "hello";
                    await bucket.RemoveAsync(documentKey);

                    // Get server index for the key
                    var vbucket = (bucket as CouchbaseBucket).GetKeyMapper().MapKey(documentKey) as IVBucket;
                    var serverIndex = vbucket.Primary;

                    // enable enhanced error messages
                    var response = await client.GetAsync(new Uri(serverUri,
                        string.Format("mock/set_enhanced_errors?enabled={0}&servers=[{1}]", TestConfiguration.Settings.EnhancedAuth, serverIndex)));
                    Assert.IsTrue(CheckContentIsValid(response));

                    // execute get operation - should contain enhanced error information
                    var result = bucket.Get<dynamic>(documentKey);
                    Assert.IsFalse(result.Success);

                    if (TestConfiguration.Settings.EnhancedAuth)
                    {
                        Assert.IsTrue(result.Message.IndexOf("context", StringComparison.OrdinalIgnoreCase) >= 0);
                        Assert.IsTrue(result.Message.IndexOf("ref #", StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    else
                    {
                        Assert.IsTrue(result.Message.IndexOf("context", StringComparison.OrdinalIgnoreCase) == -1);
                        Assert.IsTrue(result.Message.IndexOf("ref #", StringComparison.OrdinalIgnoreCase) == -1);
                    }
                }
            }
        }

        private static bool CheckContentIsValid(HttpResponseMessage message)
        {
            dynamic result;
            try
            {
                var json = message.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<dynamic>(json);
            }
            catch
            {
                return false;
            }

            if (result.status != "ok")
            {
                Assert.Fail(result.error.ToString());
            }

            return true;
        }
    }
}
