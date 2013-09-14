using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LinkLogger.Api;
using LinkLogger.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkLogger.Api.Tests
{
    [TestClass()]
    public class LinksControllerTests
    {
        [TestInitialize]
        public void Intialize()
        {
            EmptyDatabase();
        }

        [TestCleanup]
        public void Cleanup()
        {
            EmptyDatabase();
        }

        [TestMethod()]
        public async Task PostTest()
        {
            var target = new LinksController();
            var request = new LinkRequest()
                                  {
                                      Channel = "#foobar",
                                      Url = "http://www.sample.invalid",
                                      User = "Tom"
                                  };
            HttpResponseMessage response = await target.Post(request);
            int id = await response.Content.ReadAsAsync<int>();

            Link link;
            using (var ctx = new LinkLoggerContext())
            {
                link = await ctx.Links.SingleAsync();
            }

            Assert.AreEqual(request.Url, link.Url);
            Assert.AreEqual(request.User, link.Sender);
            Assert.AreEqual(request.Channel, link.Channel);
            Assert.IsTrue(link.RegisteredAt >= DateTime.Now.AddSeconds(-10));

            Assert.AreEqual(id, link.Id);

            Assert.IsTrue(id > 0);
        }

        private static void EmptyDatabase()
        {
            using (var ctx = new LinkLoggerContext())
            {
                var links = ctx.Links.ToList();
                if (!links.Any()) return;
                ctx.Links.RemoveRange(links);
                ctx.SaveChanges();
            }
        }
    }
}
