using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using LinkLogger.DataAccess;
using LinkLogger.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkLogger.Controllers.Api.Tests
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
            var config = new HttpConfiguration();
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/links");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "links" } });
            
            var controller = new LinksController();
            controller.ControllerContext = new HttpControllerContext(config, routeData, httpRequest);
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            var request = new LinkModel()
                                  {
                                      Channel = "#foobar",
                                      Url = "http://mnd.fi/",
                                      User = "Tom"
                                  };
            string response = await controller.PostLink(request);
            Assert.IsNotNull(response);
            Assert.IsTrue(Uri.IsWellFormedUriString(response, UriKind.Absolute));
        }

        [TestMethod()]
        public async Task GetListOfLinks_NoLinks_Empty()
        {
            var controller = new LinksController();
            IEnumerable<LinkModel> links = await controller.GetLinks();
            Assert.IsFalse(links.Any());
        }

        [TestMethod()]
        public async Task GetListOfLinks_1Link()
        {
            var expected = new Link()
                           {
                               Channel = "#foobar",
                               Url = "http://www.sample.invalid",
                               User = "Tom",
                               RegisteredAt = DateTime.Now
                           };

            using (var ctx = new LinkLoggerContext())
            {
                ctx.Links.Add(expected);
                await ctx.SaveChangesAsync();
            }

            var controller = new LinksController();
            IEnumerable<LinkModel> links = await controller.GetLinks();
            
            Assert.AreEqual(1, links.Count());
            var link = links.First();

            Assert.IsNotNull(link);
            Assert.AreEqual(expected.Channel, link.Channel);
            // dates are not totally equal due to SQL storage
            Assert.IsTrue((expected.RegisteredAt - link.PostedAt).Value.TotalSeconds < 1);
            Assert.AreEqual(expected.User, link.User);
            Assert.AreEqual(expected.Id, link.Id);
            Assert.AreEqual(expected.Url, link.Url);
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
