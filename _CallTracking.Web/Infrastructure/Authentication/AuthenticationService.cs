using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Web.Security;

namespace TalentSite_Preview.Web.Infrastructure.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {        
        public void SignIn(string userName, bool createPersistentCookie)
        {
           FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {            
        	FormsAuthentication.SignOut();
        }
    }
}