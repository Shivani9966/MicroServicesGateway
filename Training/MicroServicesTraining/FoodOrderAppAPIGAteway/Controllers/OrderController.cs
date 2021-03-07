using FoodOrderAppAPIGAteway.Constants;
using FoodOrderAppAPIGAteway.Models;
using FoodOrderAppAPIGAteway.RoleHelper;
using FoodOrderAppAPIGAteway.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderAppAPIGAteway.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        Httpclient httpclient = new Httpclient();
        private readonly IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        /// <summary>
        /// AssignDriver for Order
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AssignDriver")]
        public async Task<ActionResult<bool>> AssignDriver(AssignDriverRequestModel request)
        {
            if (GetUserRole() == UserRolesConstants.RestaurantManager)
            {
                Orders order = await httpclient.RunAsyncGet<int, Orders>(configuration.GetValue<string>(
                CommonConstants.GetOrderById), request.OrderId);
                Driver driver = await httpclient.RunAsyncGet<int, Driver>(configuration.GetValue<string>(
                    CommonConstants.GetDriverById), request.DriverId);
                order.DriverName = driver.FirstName + ' ' + driver.LastName;
                order.DriverId = request.DriverId;
                return Ok(await httpclient.RunAsyncPut<Orders, bool>(configuration.GetValue<string>(
                    CommonConstants.UpdateOrder), order));
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Order status change from notdelivered to delivered
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OrderDelivered")]
        public async Task<ActionResult<bool>> OrderDelivered(int orderId)
        {
            if (GetUserRole() == UserRolesConstants.Driver)
            {
                Orders order = await httpclient.RunAsyncGet<int, Orders>(configuration.GetValue<string>(
                CommonConstants.GetOrderById), orderId);
                if (order != null)
                {
                    order.IsOrderDelivered = true;
                    return Ok(await httpclient.RunAsyncPut<Orders, bool>(configuration.GetValue<string>(
                    CommonConstants.UpdateOrder), order));
                }
                return Ok(false);
            }
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
