using CallTracking.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using CallTracking.Web.Infrastructure.Logging;
using CallTracking.Web.Infrastructure.Subscription;
using CallTracking.Web.Model;

namespace CallTracking.Web.Controllers
{
    public class PricingController : Controller
    {
        ILogger _logger;
        ISubscription _mailingList;
        public Customer _customers;

        public PricingController(ILogger logger, ISubscription mailingList)
        {
            _logger = logger;
            _mailingList = mailingList;
            _customers = new Customer(logger);
        }

        //
        // GET: /Pricing/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignUp(string referKey = null)
        {
            ViewBag.Campaign = Campaign;
            SignUpViewModel model = new SignUpViewModel();
            if (referKey != null)
            {
                model.ReferredBy = referKey;
                return View("SignUp", model);
            }
            return View("SignUp");
        }

        //
        // POST: /Pricing/SignUp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpViewModel model)
        {
            ViewBag.Campaign = Campaign;

            if (ModelState.IsValid)
            {
                if (TryValidateModel(model) && ModelState.IsValid)
                {
                    try
                    {
                        if (_customers.All(where: "Email = @0", args: model.Email).Count() == 0)
                        {
                            string referKey = ReferKey();
                            _customers.Insert(new
                            {
                                Email = model.Email,
                                IsSubscribed = true,
                                ReferKey = referKey,
                                ReferredBy = model.ReferredBy
                            });
                            Response.Cookies.Add(new HttpCookie("referkey", referKey));
                            _mailingList.Add(model.Email, null, referKey, System.Configuration.ConfigurationManager.AppSettings["teaser_mailing_api_key"]);

                            ViewBag.ReferKey = referKey;
                            return RedirectToAction("Success", "Pricing");
                        }
                        else
                        {
                            model.Message = "A user with that email address already exists. Please use the Forgotten Password link if you need to recover your password.";
                            _logger.LogDebug("A user with that email address already exists. Please use the Forgotten Password link if you need to recover your password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error on Index Post. Insert Member " + model.Email);
                        _logger.LogError(ex);
                        return RedirectToAction("Success", "Home");
                    }
                }
                return View("Index", model);
                //var user = new ApplicationUser() { UserName = model.UserName };
                //var result = await UserManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInAsync(user, isPersistent: false);
                //    return RedirectToAction("Index", "Home");
                //}
                //else
                //{
                //    AddErrors(result);
                //}
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        //
        // GET: /Pricing/Success
        public ActionResult Success()
        {
            return View("Success");
        }

        #region Private Properties

        private string Campaign
        {
            get
            {
                if (Request.QueryString["cpg"] != null)
                {
                    Campaign = Request.QueryString["cpg"].ToString();
                    return Request.QueryString["cpg"].ToString();
                }
                else if (Request.Cookies["Campaign"] != null)
                {
                    Campaign = Request.Cookies["Campaign"].Value;
                    return Request.Cookies["Campaign"].Value;
                }
                return String.Empty;
            }
            set
            {
                Response.Cookies.Add(new HttpCookie("Campaign", value));
            }
        }
        private string ReferKey()
        {
            Guid guid = Guid.NewGuid();
            string s = guid.ToString().Replace("-", "").Substring(0, 8);
            int result = 0;// Convert.ToInt32(_membership.Scalar("select count(*) from members where ReferKey = @0", args: s));
            if (result != 0)
            {
                _logger.LogWarning("ReferKey '" + s + "' came back with result of " + result);

                string t = "";
                while (t == "")
                    t = ReferKey();
                s = t;
            }
            return s;
        }

        #endregion
	}
}