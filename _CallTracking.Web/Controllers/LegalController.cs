using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CallTracking.Web.Controllers
{
    public class LegalController : Controller
    {
        //
        // GET: /Legal/
        public ActionResult tos()
        {
            return View("Index");
        }
        //
        // GET: /Legal/
        public ActionResult aup()
        {
            return View("Index");
        }
        //
        // GET: /Legal/
        public ActionResult privacy()
        {
            return View("Index");
        }
	}
}