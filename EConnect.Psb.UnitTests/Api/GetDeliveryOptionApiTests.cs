using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EConnect.Psb.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EConnect.Psb.UnitTests.Api
{
    [TestClass]
    public class GetDeliveryOptionApiTests
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetDeliveryOptionsTest(bool usePeppolTest)
        {
            var api = new PsbPeppolLookupApi(usePeppolTest);

            var deliveryOptions = await api.GetDeliveryOptions(new List<string>() { "0106:54441587" });
            
            Assert.IsNotNull(deliveryOptions);
            Assert.IsTrue(deliveryOptions.Any());
            Assert.AreEqual("NL:KVK:54441587", deliveryOptions.First().PartyId);
        }
    }
}
