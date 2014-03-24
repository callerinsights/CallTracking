using CallTracking.Web.Infrastructure.Logging;
using CallTracking.Web.Infrastructure.Subscription;
using CallTracking.Web.Model;
using CallTracking.Web.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CallTracking.Web.Controllers
{
    public class HomeController : Controller
    {
        ILogger _logger;
        ISubscription _mailingList;
        IEmailService _emailService;
        public Customer _customers;
        
        public HomeController(ILogger logger, ISubscription mailingList, IEmailService emailService)
        {
            _logger = logger;
            _mailingList = mailingList;
            _emailService = emailService;
            _customers = new Customer(logger);
        }

        public ActionResult Index()
        {
            _logger.LogInfo(DateTime.Now.ToString());
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //
        // POST: /Home/Contact
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (TryValidateModel(model) && ModelState.IsValid)
                {
                    try
                    {
                        dynamic email = new Email("Forgotten");
                        email.To = model.Email;
                        email.NewPassword = "DOODLEPOPS";
                        _emailService.Send(email);

                        dynamic zemail = new Email("Contact");
                        zemail.To = model.Email;
                        _emailService.Send(zemail);


                        dynamic enquiry = new Email("Enquiry");
                        enquiry.Name = model.Name;
                        enquiry.Company = model.Company;
                        enquiry.Phone = model.Phone;
                        enquiry.enquiry = model.Enquiry;
                        enquiry.From = model.Email;
                        _emailService.Send(enquiry);

                        return View("Enquiry");                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error on Contact Page. Enquiry from " + model.Email);
                        _logger.LogError(ex);
                        return RedirectToAction("Contact", "Home");
                    }
                }
                return View("Contact", model);
            }

            return View(model);
        }
    }
}