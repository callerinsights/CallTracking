using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using createsend_dotnet;
using CallTracking.Web.Infrastructure.Logging;
using System.Configuration;

namespace CallTracking.Web.Infrastructure.Subscription
{
    public interface ISubscription
    {
        void Add(string email, string name, string referKey, string listID);

    }
    public class CampaignMonitorSubscription : ISubscription
    {
        ILogger _logger;
        AuthenticationDetails auth;

        public CampaignMonitorSubscription(ILogger logger)
        {
            _logger = logger;
            auth = new ApiKeyAuthenticationDetails(ConfigurationManager.AppSettings["api_key"].ToString());
        }

        public void Add(string email, string name, string referKey, string listID)
        {
            Subscriber s = new Subscriber(auth, listID);
            try
            {
                SubscriberCustomField custom = new SubscriberCustomField();
                custom.Key = "ReferKey";
                custom.Value = referKey;
                List<SubscriberCustomField> list = new List<SubscriberCustomField>();
                list.Add(custom);

                s.Add(email, name, list, true);
            }
            catch (CreatesendException ex)
            {
                ErrorResult error = (ErrorResult)ex.Data["ErrorResult"];
                _logger.LogError("Campaign Monitor Error: " + error.Code + " - " + error.Message);
            }
        }

    }
}