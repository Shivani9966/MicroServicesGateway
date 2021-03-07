using FoodOrderAppAPIGAteway.Constants;
using FoodOrderAppAPIGAteway.Models;
using FoodOrderAppAPIGAteway.RoleHelper;
using FoodOrderAppAPIGAteway.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderAppAPIGAteway.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        Httpclient httpclient = new Httpclient();
        private readonly IConfiguration configuration;

        public RestaurantController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        /// <summary>
        /// Get current orders for Restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllOrders")]
        public async Task<ActionResult<List<Orders>>> GetAllCurrentOrders(int restaurantId)
        {
            if (GetUserRole() == UserRolesConstants.RestaurantManager)
                return Ok(await httpclient.RunAsyncGet<int, List<Orders>>(configuration.GetValue<string>(
                CommonConstants.GetOrderByRestaurantId), restaurantId));
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Get monthly report for orders
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMonthlyOrderReport")]
        public async Task<ActionResult<List<Orders>>> GetMonthlyOrderReport(int restaurantId)
        {
            if (GetUserRole() == UserRolesConstants.RestaurantManager)
            {
                List<Orders> orders = await httpclient.RunAsyncGet<int, List<Orders>>(configuration.GetValue<string>(
                CommonConstants.GetOrderByRestaurantId), restaurantId);
                return Ok(orders.Where(x => x.IsOrderDelivered == true && (DateTime.Now - x.OrderDate.Value).TotalDays <= 30).ToList());
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Get 2 month report for orders filter by driver
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOrderReportDriver")]
        public async Task<ActionResult<List<Orders>>> GetOrderReportFilterByDriver(OrderReportRequestModel reportRequest)
        {
            if (GetUserRole() == UserRolesConstants.RestaurantManager)
            {
                List<Orders> orders = await httpclient.RunAsyncGet<int, List<Orders>>(configuration.GetValue<string>(
                CommonConstants.GetOrderByRestaurantId), reportRequest.RestaurantId);
                return Ok(orders.Where(x => x.DriverId == reportRequest.DriverId && x.IsOrderDelivered == true && (DateTime.Now - x.OrderDate.Value).TotalDays <= 61).ToList());
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
