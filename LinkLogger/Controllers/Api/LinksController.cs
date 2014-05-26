using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LinkLogger.DataAccess;
using LinkLogger.Hubs;
using LinkLogger.Models;

namespace LinkLogger.Controllers.Api
{
    /// <summary>
    /// LinksController is used as the main API endpoint for querying link data
    /// and posting info on new links.
    /// </summary>
    [RoleFilter("LinkViewer")]
    [RoutePrefix("api/links")]
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

        /// <summary>
        /// Get a specific link
        /// </summary>
        /// <param name="id">Link id</param>
        /// <returns></returns>
        [Route("{id}", Name = "GetLinkById")]
        public async Task<LinkModel> GetLink(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                Link link = await context.Links.FindAsync(id);
                return MapLinkToLinkModel(link);
            }
        }

        [Route("")]
        public async Task<IEnumerable<LinkModel>> GetLinks()
        {
            using (var context = new ApplicationDbContext())
            {
                var links = await context.Links.OrderByDescending(l => l.RegisteredAt).Take(20).ToArrayAsync();
                return links.Select(MapLinkToLinkModel);
            }
        }

        [Route("before/{id:int}")]
        public async Task<IEnumerable<LinkModel>> GetLinksOlderThan(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var links = await context.Links.Where(l => l.Id < id).OrderByDescending(l => l.RegisteredAt).Take(10).ToArrayAsync();
                return links.Select(MapLinkToLinkModel);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> PostLink(LinkModel model)
        {
            if (!Request.Headers.Contains("Linx-Access-Token"))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Access token missing.");
            }

            IEnumerable<string> accessTokenHeaderValues = Request.Headers.GetValues("Linx-Access-Token");
            string accessToken = accessTokenHeaderValues.FirstOrDefault();

            if (accessToken != _appSettings.PostLinkAccessToken)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid access token.");
            }
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(s => s.Value.Errors).Select(err => err.Exception.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                       {
                           Content =
                               new StringContent(
                               "Invalid model state: " +
                               string.Join(",", errors))
                       };
            }

            bool isImage = HtmlHelpers.IsImage(model.Url);
            
            if (string.IsNullOrEmpty(model.Title) && !isImage)
            {
                model.Title = await HtmlHelpers.FetchTitle(model.Url);
            }

            using (var context = new ApplicationDbContext())
            {
                var link = MapLinkModelToLink(model);
                context.Links.Add(link);
                int rowsAdded = await context.SaveChangesAsync();
                // TODO: log
                if (rowsAdded != 1) throw new HttpResponseException(HttpStatusCode.InternalServerError);

                Hub.Clients.All.addNewLink(MapLinkToLinkModel(link));
                model.Id = link.Id;
                model.PostedAt = link.RegisteredAt;
                var response = Request.CreateResponse(HttpStatusCode.Created);
                response.Headers.Location = new Uri(Url.Link("GetLinkById", new { id = model.Id }));
                return response;
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