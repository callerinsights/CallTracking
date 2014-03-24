using CallTracking.Web.Infrastructure.Logging;
using CallTracking.Web.Model;
using Plivo.API;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CallTracking.Web.Controllers
{
    public class AnswerController : ApiController
    {
        ILogger _logger;

        public AnswerController() { }

        public AnswerController(ILogger logger)
        {
            _logger = logger;

            string auth_id = ConfigurationManager.AppSettings["plivo_auth_id"];
            string auth_token = ConfigurationManager.AppSettings["plivo_auth_tokem"];
            //RestAPI plivo = new RestAPI(auth_id, auth_token);
           
        }

        // GET api/answer
        public string Get()
        {
            var tester = new Test(_logger);
            tester.Insert(new
            {
                Date = DateTime.Now,
                Reference = DateTime.Now.ToShortDateString()
            });


            Plivo.XML.Response resp = new Plivo.XML.Response();
            Plivo.XML.Dial dial = new Plivo.XML.Dial(new Dictionary<string, string>() { });
            try
            {
                _logger.LogInfo(DateTime.Now.ToString());

                //dial.AddNumber("6499636488", new Dictionary<string, string>());
                //resp.Add(dial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }


            return "Success";// resp.ToString();
        }

        // GET api/answer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/answer
        public void Post([FromBody]string value)
        {
        }

        // PUT api/answer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/answer/5
        public void Delete(int id)
        {
        }
    }
}
