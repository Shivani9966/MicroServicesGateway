using LoginAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI.Services
{
    public interface ILoginService
    {
        LoginModel AuthenticateUser(LoginModel login);
    }
}
