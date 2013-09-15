using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LinkLogger.DataAccess;

namespace LinkLogger.Controllers.Api
{
    public class LinksController : ApiController
    {
        public async Task<Link> Get(int id)
        {
            using (var context = new LinkLoggerContext())
            {
                Link link = await context.Links.FindAsync(id);
                return link;
            }
        }

        public async Task<HttpResponseMessage> Post(LinkRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(s => s.Value.Errors).Select(err => err.Exception.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid model state: "+string.Join(",", errors));
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
                if (rowsAdded != 1) new HttpResponseMessage(HttpStatusCode.InternalServerError);

                //var response = new LinkResponse()
                //               {
                //                   Id = link.Id
                //               };

                return Request.CreateResponse(HttpStatusCode.OK, link.Id);
            }
        }


    }

    public class LinkRequest
    {
        public string Url { get; set; }

        public string User { get; set; }

        public string Channel { get; set; }
    }
}