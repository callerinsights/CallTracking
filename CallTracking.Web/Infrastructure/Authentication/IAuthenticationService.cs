using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalentSite_Preview.Web.Infrastructure.Authentication
{
    public interface IAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }
}
