using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using LinkLogger.DataAccess;
using LinkLogger.Hubs;
using LinkLogger.Models;

namespace LinkLogger.Controllers.Api
{
    [System.Web.Http.Authorize(Roles = "LinkViewer")]
    public class LinksController : ApiControllerWithHub<LinkHub>
    {
        private readonly IApplicationSettings _appSettings;

        public LinksController() : this(new WebConfigApplicationSettings())
        {
        }

        public LinksController(IApplicationSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<LinkModel> GetLink(int id)
        {
            using (var context = new LinkLoggerContext())
            {
                Link link = await context.Links.FindAsync(id);
                return MapLinkToLinkModel(link);
            }
        }

        public async Task<IEnumerable<LinkModel>> GetLinks()
        {
            using (var context = new LinkLoggerContext())
            {
                var links = await context.Links.OrderByDescending(l => l.RegisteredAt).Take(10).ToArrayAsync();
                return links.Select(MapLinkToLinkModel);
            }
        }

        [System.Web.Http.AllowAnonymous]
        public async Task<ActionResult> PostLink(LinkModel model)
        {
            if (!Request.Headers.Contains("Linx-Access-Token"))
            {
                return new HttpUnauthorizedResult("Access token missing.");
            }

            IEnumerable<string> accessTokenHeaderValues = Request.Headers.GetValues("Linx-Access-Token");
            string accessToken = accessTokenHeaderValues.FirstOrDefault();

            if (accessToken != _appSettings.PostLinkAccessToken)
            {
                return new HttpUnauthorizedResult("Invalid access token.");
            }
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(s => s.Value.Errors).Select(err => err.Exception.Message);
                var msg = new HttpResponseMessage(HttpStatusCode.BadRequest)
                          {
                              Content =
                                  new StringContent(
                                  "Invalid model state: " +
                                  string.Join(",", errors))
                          };
                throw new HttpResponseException(msg);
            }

            bool isImage = HtmlHelpers.IsImage(model.Url);
            
            if (string.IsNullOrEmpty(model.Title) && !isImage)
            {
                model.Title = await HtmlHelpers.FetchTitle(model.Url);
            }

            using (var context = new LinkLoggerContext())
            {
                var link = MapLinkModelToLink(model);
                context.Links.Add(link);
                int rowsAdded = await context.SaveChangesAsync();
                // TODO: log
                if (rowsAdded != 1) throw new HttpResponseException(HttpStatusCode.InternalServerError);

                Hub.Clients.All.addNewLink(MapLinkToLinkModel(link));
                model.Id = link.Id;
                return new JsonResult() { Data = model };
            }
        }

        private static Link MapLinkModelToLink(LinkModel model)
        {
            return new Link
                   {
                       Channel = model.Channel,
                       RegisteredAt = DateTime.Now,
                       User = model.User,
                       Url = model.Url,
                       Title = model.Title
                   };
        }

        private static LinkModel MapLinkToLinkModel(Link arg)
        {
            return new LinkModel()
                   {
                       Channel = arg.Channel,
                       Id = arg.Id,
                       PostedAt = arg.RegisteredAt,
                       User = arg.User,
                       Url = arg.Url,
                       Title = arg.Title
                   };
        }
    }
}