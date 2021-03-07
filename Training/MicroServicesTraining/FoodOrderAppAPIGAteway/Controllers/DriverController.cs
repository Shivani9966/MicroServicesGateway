using FoodOrderAppAPIGAteway.Constants;
using FoodOrderAppAPIGAteway.Models;
using FoodOrderAppAPIGAteway.RoleHelper;
using FoodOrderAppAPIGAteway.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderAppAPIGAteway.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        Httpclient httpclient = new Httpclient();
        private readonly IConfiguration configuration;

        public DriverController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        /// <summary>
        /// Get current(pending) orders for Driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCurrentOrders")]
        public async Task<ActionResult<List<Orders>>> GetAllCurrentOrders(int driverId)
        {
            if (GetUserRole() == UserRolesConstants.Driver)
            {
                List<Orders> orders = await httpclient.RunAsyncGet<int, List<Orders>>((configuration.GetValue<string>(
                CommonConstants.GetOrderByDriverId)), driverId);
                return Ok(orders.Where(x => x.IsOrderDelivered == false).ToList());
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Get Delivered orders for Driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDeliveredOrders")]
        public async Task<ActionResult<List<Orders>>> GetAllDeliveredOrders(int driverId)
        {
            if (GetUserRole() == UserRolesConstants.Driver)
            {
                List<Orders> orders = await httpclient.RunAsyncGet<int, List<Orders>>((configuration.GetValue<string>(
                CommonConstants.GetOrderByDriverId)), driverId);
                return Ok(orders.Where(x => x.IsOrderDelivered == true).ToList());
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Get all orders for Driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrders")]
        public async Task<ActionResult<List<Orders>>> GetAllOrders(int driverId)
        {
            if (GetUserRole() == UserRolesConstants.Driver)
                return Ok(await httpclient.RunAsyncGet<int, List<Orders>>((configuration.GetValue<string>(
                CommonConstants.GetOrderByDriverId)), driverId));
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        private string GetUserRole()
        {
            if (HttpContext != null)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    return UserRoleHelper.GetUserRole(identity);
                }
            }
            return string.Empty;
        }
    }
}
