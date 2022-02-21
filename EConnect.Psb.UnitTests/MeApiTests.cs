using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace EConnect.Psb.UnitTests
{
    [TestClass]
    public class MeApiTests : PsbTestContext
    {
        [TestMethod]
        public async Task Me()
        {
            var res = await MeApi.Me();

            Assert.AreEqual("eConnectUser", res.Name);
        }


        [TestMethod]
        public async Task MeParties()
        {
            var res = (await MeApi.MeParties()).ToList();
            
            Assert.AreEqual(res[0].Name, "elien");
        }
    }
}