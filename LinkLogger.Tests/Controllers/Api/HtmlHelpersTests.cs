using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkLogger.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace LinkLogger.Controllers.Api.Tests
{
    [TestClass()]
    public class HtmlHelpersTests
    {
        [TestMethod()]
        public async Task FetchTitle_TextPage_Success()
        {
            string expected = "Jokerikone yskähteli MTV3:n ensimmäisessä lottoarvonnassa";
            var url = "http://www.hs.fi/kotimaa/Jokerikone+ysk%C3%A4hteli+MTV3n+ensimm%C3%A4isess%C3%A4+lottoarvonnassa/a1305727779361";
            string actual = await HtmlHelpers.FetchTitle(url);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public async Task FetchTitle_TextPage2_Success()
        {
            string expected = "Supersuosittu Robin paljasti Arto Nybergille: Näin pidän pääni kasassa";
            var url = "http://www.iltasanomat.fi/viihde/art-1288606574642.html";
            string actual = await HtmlHelpers.FetchTitle(url);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IsImage_TextPage_False()
        {
            var url = "http://www.hs.fi/kotimaa/Jokerikone+ysk%C3%A4hteli+MTV3n+ensimm%C3%A4isess%C3%A4+lottoarvonnassa/a1305727779361";
            bool actual = HtmlHelpers.IsImage(url);
            Assert.IsFalse(actual);
        }

        [TestMethod()]
        public void IsImage_Jpg_True()
        {
            var url = "http://www.sample.fi/foo/bar/baz.jpg";
            bool actual = HtmlHelpers.IsImage(url);
            Assert.IsTrue(actual);
        }

        [TestMethod()]
        public void IsImage_Png_True()
        {
            var url = "http://www.sample.fi/foo/bar/baz.png";
            bool actual = HtmlHelpers.IsImage(url);
            Assert.IsTrue(actual);
        }

        [TestMethod()]
        public void IsImage_Gif_True()
        {
            var url = "http://www.sample.fi/foo/bar/baz.gif";
            bool actual = HtmlHelpers.IsImage(url);
            Assert.IsTrue(actual);
        }
    }
}
