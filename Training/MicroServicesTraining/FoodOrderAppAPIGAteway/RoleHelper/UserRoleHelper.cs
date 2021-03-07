using FoodOrderAppAPIGAteway.Models;
using FoodOrderAppAPIGAteway.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderAppAPIGAteway.RoleHelper
{
    public class UserRoleHelper
    {
        public static string GetUserRole(ClaimsIdentity identity)
        {
            IEnumerable<Claim> claims = identity.Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}
