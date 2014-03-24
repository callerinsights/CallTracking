using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Massive;
using CallTracking.Web.Infrastructure.Logging;

namespace CallTracking.Web.Model
{
    public class Test : DynamicModel
    {
        ILogger _logger;

        public Test(ILogger logger) : base("MassiveConnection", "insights_test", "Id") 
        {
            _logger = logger;
        }

        //public void Call()
        //{
            
        //}
    }
}