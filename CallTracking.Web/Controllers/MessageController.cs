using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CallTracking.Web.Controllers
{
    public class MessageController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Index()
        {
            //var product = products.FirstOrDefault((p) => p.Id == id);
            //if (product == null)
            //{
                return NotFound();
            //}
            //return Ok(product);
        }
    }
}
