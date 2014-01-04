using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using gdRead.Data;
using gdRead.Data.Models;

namespace gdRead.Web.Controllers
{
    public class PostsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Post> Get(int id)
        {
            var ctx = new gdReadContext();
            return ctx.Posts.Where(x => x.Feed.Id == id);
        }

    }
}