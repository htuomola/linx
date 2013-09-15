using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LinkLogger.DataAccess;
using Microsoft.Ajax.Utilities;

namespace LinkLogger.Controllers.Api
{
    public class LinksController : ApiController
    {
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

        public async Task<string> PostLink(LinkModel request)
        {
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

            using (var context = new LinkLoggerContext())
            {
                var link = new Link
                           {
                               Channel = request.Channel,
                               RegisteredAt = DateTime.Now,
                               Sender = request.User,
                               Url = request.Url
                           };
                context.Links.Add(link);
                int rowsAdded = await context.SaveChangesAsync();
                // TODO: log
                if (rowsAdded != 1) throw new HttpResponseException(HttpStatusCode.InternalServerError);

                //var response = new LinkResponse()
                //               {
                //                   Id = link.Id
                //               };

                return Url.Link("DefaultApi", new {controller = "links", id = link.Id});
            }
        }

        private LinkModel MapLinkToLinkModel(Link arg)
        {
            return new LinkModel()
                   {
                       Channel = arg.Channel,
                       Id = arg.Id,
                       PostedAt = arg.RegisteredAt,
                       User = arg.Sender,
                       Url = arg.Url
                   };
        }
    }

    public class LinkModel
    {
        public int? Id { get; set; }
        
        [Required]
        public string Url { get; set; }
        
        [Required]
        public string User { get; set; }
        
        [Required]
        public string Channel { get; set; }
        
        public System.DateTime? PostedAt { get; set; }
    }
}