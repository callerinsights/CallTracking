using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace CallTracking.Services.Authentication
{
    public interface IAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

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
