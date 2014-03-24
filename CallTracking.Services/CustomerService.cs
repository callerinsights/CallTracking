using CallTracking.Data;
using CallTracking.Data.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallTracking.Services
{
    public interface ICustomerService{
        string ResetPassword(string email);
    }

    public class CustomerService: ICustomerService
    {
        ICustomerRepository _customerRepository;
        ILogger _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public string ResetPassword(string email)
        {
            //Customer c = _customerRepository.GetByEmail(email);
            //if (c != null)
            //{
            //    string pass = Membership.GeneratePassword(8, 2);
            //    m.Password = EncodePassword(pass, m.PasswordFormat, m.PasswordSalt);
            //    _memberRepository.Save(m);
            //    return pass;
            //}
            return String.Empty;
        }
    }
}
